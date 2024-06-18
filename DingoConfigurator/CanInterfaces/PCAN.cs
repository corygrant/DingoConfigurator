using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCAN;

namespace CanInterfaces
{
    // Type alias for a PCAN-Basic channel handle
    using TPCANHandle = System.UInt16;
    // Type alias for a CAN-FD bitrate string
    using TPCANBitrateFD = System.String;
    // Type alias for a microseconds timestamp
    using TPCANTimestampFD = System.UInt64;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    public class PCAN : ICanInterface
    {
        private int _rxTimeDelta;
        public int RxTimeDelta { get => _rxTimeDelta; }
        private Stopwatch _rxStopwatch;

        #region Defines
        /// <summary>
        /// Sets the PCANHandle (Hardware Channel)
        /// </summary>
        TPCANHandle PcanHandle = PCANBasic.PCAN_USBBUS1;
        /// <summary>
        /// Sets the desired connection mode (CAN = false / CAN-FD = true)
        /// </summary>
        const bool IsFD = false;
        /// <summary>
        /// Sets the bitrate for normal CAN devices
        /// </summary>
        TPCANBaudrate Bitrate = TPCANBaudrate.PCAN_BAUD_500K;
        /// <summary>
        /// Sets the bitrate for CAN FD devices. 
        /// Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
        ///   "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
        /// </summary>
        const TPCANBitrateFD BitrateFD = "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1";
        #endregion

        #region Members
        /// <summary>
        /// Shows if DLL was found
        /// </summary>
        private bool m_DLLFound;
        /// <summary>
        /// Thread for reading messages
        /// </summary>
        private Thread m_ReadThread;
        /// <summary>
        /// Shows if thread run
        /// </summary>
        private bool m_ThreadRun;
        #endregion

        public DataReceivedHandler DataReceived { get; set; }

        private void OnDataReceived(CanDataEventArgs e)
        {
            if(DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public bool Init(string port, CanInterfaceBaudRate baud)
        {
            // Checks if PCANBasic.dll is available, if not, the program terminates
            m_DLLFound = CheckForLibrary();
            if (!m_DLLFound)
            {
                MessageBox.Show("PCAN DLL Not Found", "PCAN Init", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            TPCANStatus stsResult;

            PcanHandle = ConvertHandle(port);
            Bitrate = ConvertBaudRate(baud);

            // Initialization of the selected channel
            stsResult = PCANBasic.Initialize(PcanHandle, Bitrate);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("Can not initialize. Please check the defines in the code.");
                MessageBox.Show(GetFormattedError(stsResult), "PCAN Init Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool Start()
        {
            if (!(PCANBasic.GetStatus(PcanHandle) == TPCANStatus.PCAN_ERROR_OK)) return false;
            m_ReadThread = new Thread(new ThreadStart(ReceiveThread));
            m_ThreadRun= true;
            m_ReadThread.Start();

            _rxStopwatch = Stopwatch.StartNew();

            return true;
        }

        public bool Stop()
        {
            if (!m_DLLFound) return false; 
            m_ThreadRun= false;
            m_ReadThread.Join();
            if(!(PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS) == TPCANStatus.PCAN_ERROR_OK)) return false;
            return true;
        }

        ~PCAN()
        {
            if (m_DLLFound)
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
        }

        public bool Write(CanInterfaceData canData)
        {
            if(!(PCANBasic.GetStatus(PcanHandle) == TPCANStatus.PCAN_ERROR_OK)) return false;
            if(!(canData.Payload.Length == 8)) return false;

            var msg = new TPCANMsg();
            msg.DATA = new byte[8];
            msg.DATA[0] = canData.Payload[0];
            msg.DATA[1] = canData.Payload[1];
            msg.DATA[2] = canData.Payload[2];
            msg.DATA[3] = canData.Payload[3];
            msg.DATA[4] = canData.Payload[4];
            msg.DATA[5] = canData.Payload[5];
            msg.DATA[6] = canData.Payload[6];
            msg.DATA[7] = canData.Payload[7];
            //msg.DATA = canData.Payload;
            msg.ID = Convert.ToUInt16(canData.Id);
            msg.LEN = Convert.ToByte(canData.Len);
            msg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;

            if(PCANBasic.Write(PcanHandle, ref msg) == TPCANStatus.PCAN_ERROR_OK) return true;

            return false;
        }

        private void ReceiveThread()
        {
            AutoResetEvent evtReceiveEvent = new AutoResetEvent(false);
            UInt32 iBuffer = Convert.ToUInt32(evtReceiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32());
            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if(stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                MessageBox.Show(GetFormattedError(stsResult), "PCAN Receive Thread Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (m_ThreadRun)
            {
                if(evtReceiveEvent.WaitOne(50))
                    ReadMessages();
                    
            }

            iBuffer = 0;

            stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, ref iBuffer, sizeof(UInt32));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                MessageBox.Show(GetFormattedError(stsResult), "PCAN Receive Thread Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            evtReceiveEvent.Dispose();
        }

        private void ReadMessages()
        {
            TPCANStatus stsResult;
            // We read at least one time the queue looking for messages. If a message is found, we look again trying to 
            // find more. If the queue is empty or an error occurr, we get out from the dowhile statement.
            do
            {
                stsResult = PCANBasic.Read(PcanHandle, out TPCANMsg CANMsg, out TPCANTimestamp CANTimeStamp);
                if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                {
                    _rxTimeDelta = Convert.ToInt32(_rxStopwatch.ElapsedMilliseconds);
                    _rxStopwatch.Restart();

                    CanInterfaceData data = new CanInterfaceData
                    {
                        Id = Convert.ToInt16(CANMsg.ID),
                        Len = CANMsg.LEN,
                        Payload = CANMsg.DATA
                    };
                    OnDataReceived(new CanDataEventArgs(data));
                }

                if (stsResult != TPCANStatus.PCAN_ERROR_OK && stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                {
                    MessageBox.Show(GetFormattedError(stsResult), "PCAN Read Messages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } while ((!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
        }

        /// <summary>
        /// Checks for availability of the PCANBasic library
        /// </summary>
        /// <returns>If the library was found or not</returns>
        private bool CheckForLibrary()
        {
            // Check for dll file
            try
            {
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
                return true;
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Unable to find the library: PCANBasic.dll !");
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }

            return false;
        }

        private string GetFormattedError(TPCANStatus error)
        {
            // Creates a buffer big enough for a error-text
            var strTemp = new StringBuilder(256);
            // Gets the text using the GetErrorText API function. If the function success, the translated error is returned. 
            // If it fails, a text describing the current error is returned.
            if (PCANBasic.GetErrorText(error, 0x09, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);

            return strTemp.ToString();
        }

        private TPCANHandle ConvertHandle(string name)
        {
            switch(name)
            {
                case "USBBUS1":
                    return PCANBasic.PCAN_USBBUS1;

                case "USBBUS2":
                    return PCANBasic.PCAN_USBBUS2;

                case "USBBUS3":
                    return PCANBasic.PCAN_USBBUS3;

                case "USBBUS4":
                    return PCANBasic.PCAN_USBBUS4;

                case "USBBUS5":
                    return PCANBasic.PCAN_USBBUS5;

                case "USBBUS6":
                    return PCANBasic.PCAN_USBBUS6;

                case "USBBUS7":
                    return PCANBasic.PCAN_USBBUS7;

                case "USBBUS8":
                    return PCANBasic.PCAN_USBBUS8;

                case "USBBUS9":
                    return PCANBasic.PCAN_USBBUS9;

                case "USBBUS10":
                    return PCANBasic.PCAN_USBBUS10;

                case "USBBUS11":
                    return PCANBasic.PCAN_USBBUS11;

                case "USBBUS12":
                    return PCANBasic.PCAN_USBBUS12;

                case "USBBUS13":
                    return PCANBasic.PCAN_USBBUS13;

                case "USBBUS14":
                    return PCANBasic.PCAN_USBBUS14;

                case "USBBUS15":
                    return PCANBasic.PCAN_USBBUS15;

                case "USBBUS16":
                    return PCANBasic.PCAN_USBBUS16;

                default:
                    return PCANBasic.PCAN_USBBUS1;
            }
        }
        private TPCANBaudrate ConvertBaudRate(CanInterfaceBaudRate baud)
        {
            switch (baud)
            {
                case CanInterfaceBaudRate.BAUD_1M:
                    return TPCANBaudrate.PCAN_BAUD_1M;

                case CanInterfaceBaudRate.BAUD_500K:
                    return TPCANBaudrate.PCAN_BAUD_500K;

                case CanInterfaceBaudRate.BAUD_250K:
                    return TPCANBaudrate.PCAN_BAUD_250K;

                default:
                    return TPCANBaudrate.PCAN_BAUD_500K;
            }
        }
    }
}

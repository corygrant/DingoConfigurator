using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Peak.Can.Basic;
using Peak.Can.Basic.BackwardCompatibility;

namespace CanInterfaces
{
    public class PCAN : ICanInterface
    {
        private int _rxTimeDelta;
        public int RxTimeDelta { get => _rxTimeDelta; }
        private Stopwatch _rxStopwatch;

        public DataReceivedHandler DataReceived { get; set; }

        private Worker _worker;

        private void OnDataReceived(CanDataEventArgs e)
        {
            if(DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public bool Init(string port, CanInterfaceBaudRate baud)
        {
            Bitrate br = ConvertBaudRate(baud);

            _worker = new Worker(PcanChannel.Usb01, br);

            return true;
        }

        void ICanInterface.Disconnect()
        {
            Stop();
        }

        public bool Start()
        {
            if (_worker == null) return false;

            _worker.MessageAvailable += OnMessageAvailable;
            try {                 
                _worker.Start(true);
            }
            catch (PcanBasicException e)
            {
                return false;
            }

            _rxStopwatch = Stopwatch.StartNew();

            return true;
        }

        public bool Stop()
        {
            if (_worker == null) return false;
            _worker.MessageAvailable -= OnMessageAvailable;
            _worker.Stop();
            return true;
        }

        public bool Write(CanInterfaceData canData)
        {
            if (_worker == null) return false;
            if (!(canData.Payload.Length == 8)) return false;
            if(!(_worker.Active)) return false;

            var msg = new PcanMessage((uint)canData.Id, MessageType.Standard, (byte)canData.Len, canData.Payload);

            PcanStatus result;
            if(!_worker.Transmit(msg, out result))
            {
                //Check result var
                return false;
            }

            return true;
        }

        private void OnMessageAvailable(object sender, MessageAvailableEventArgs e)
        {
            if (_worker == null) return;
            PcanMessage msg;
            if(_worker.Dequeue(e.QueueIndex, out msg, out ulong ts))
            {
                _rxTimeDelta = Convert.ToInt32(_rxStopwatch.ElapsedMilliseconds);
                _rxStopwatch.Restart();

                CanInterfaceData data = new CanInterfaceData
                {
                    Id = Convert.ToInt16(msg.ID),
                    Len = msg.Length,
                    Payload = msg.Data
                };
                OnDataReceived(new CanDataEventArgs(data));
            }

        }

        private Peak.Can.Basic.Bitrate ConvertBaudRate(CanInterfaceBaudRate baud)
        {
            switch (baud)
            {
                case CanInterfaceBaudRate.BAUD_1000K:
                    return Bitrate.Pcan1000;

                case CanInterfaceBaudRate.BAUD_500K:
                    return Bitrate.Pcan500;

                case CanInterfaceBaudRate.BAUD_250K:
                    return Bitrate.Pcan250;

                case CanInterfaceBaudRate.BAUD_125K:
                    return Bitrate.Pcan125;

                default:
                    return Bitrate.Pcan500;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public class USB : ICanInterface
    {
        private SerialPort _serial;

        private int _rxTimeDelta;
        public int RxTimeDelta { get => _rxTimeDelta; }
        private Stopwatch _rxStopwatch;
        private bool _disposed = false;

        public DataReceivedHandler DataReceived { get; set; }

        private void OnDataReceived(CanDataEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public bool Init(string port, CanInterfaceBaudRate baud)
        {
            try
            {
                _serial = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                _serial.Handshake = Handshake.None;
                _serial.NewLine = "\r";
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var ser = (SerialPort)sender;
            if (ser == null) return;
            if (!ser.IsOpen) return;

            foreach (var raw in ser.ReadExisting().Split('\r'))
            {

                if (raw.Length >= 5) //'t' msg is always at least 5 bytes long (t + ID ID ID + DLC)
                {
                    if (raw.Substring(0, 1) != "t") return;

                    _rxTimeDelta = Convert.ToInt32(_rxStopwatch.ElapsedMilliseconds);
                    _rxStopwatch.Restart();

                    var id = int.Parse(raw.Substring(1, 3), System.Globalization.NumberStyles.HexNumber);
                    var len = int.Parse(raw.Substring(4, 1), System.Globalization.NumberStyles.HexNumber);

                    //Msg comes in as a hex string
                    //For example, an ID of 2008(0x7D8) will be sent as "t7D8...."
                    //The string needs to be parsed into an int using int.Parse
                    //The payload bytes are split across 2 bytes (a nibble each)
                    //For example, a payload byte of 28 (0001 1100) would be split into "1C"
                    byte[] payload;
                    if (len > 0)
                    {
                        payload = new byte[len];
                        for (int i = 0; i < payload.Length; i++)
                        {
                            int highNibble = int.Parse(raw.Substring(i * 2 + 5, 1), System.Globalization.NumberStyles.HexNumber);
                            int lowNibble = int.Parse(raw.Substring(i * 2 + 6, 1), System.Globalization.NumberStyles.HexNumber);
                            payload[i] = (byte)(((highNibble & 0x0F) << 4) + (lowNibble & 0x0F));
                        }
                    }
                    else
                    {
                        //Length was 0, create empty data
                        payload = new byte[8];
                    }

                    CanInterfaceData data = new CanInterfaceData
                    {
                        Id = id,
                        Len = len,
                        Payload = payload
                    };

                    OnDataReceived(new CanDataEventArgs(data));
                }
            }
        }

        public bool Start()
        {
            _serial.DataReceived += _serial_DataReceived;
            try 
            {
                _serial.Open();

                _rxStopwatch = Stopwatch.StartNew();
            }
            catch(Exception e)
            {
                _serial.DataReceived -= _serial_DataReceived;
                return false;
            }

            return _serial.IsOpen;
        }

        public bool Stop()
        {
            _serial.DataReceived -= _serial_DataReceived;
            if (_serial == null) return false;
            if (!_serial.IsOpen) return false;

            _serial.Close();

            return true;
        }

        public bool Write(CanInterfaceData canData)
        {
            if (!_serial.IsOpen)
                return false;
            if (!(canData.Len > 0))
                return false;

            try
            {
                byte[] data = new byte[canData.Len];
                for(int i = 0; i < data.Length; i++)
                {
                    data[i] = canData.Payload[i];
                }

                _serial.Write(data, 0, canData.Len);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_serial != null)
                    {
                        _serial.Close();
                        _serial.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public class USB : ICanInterface
    {
        private SerialPort _serial;

        public DataReceivedHandler DataReceived { get; set; }
        private void OnDataReceived(CanDataEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        ~USB()
        {
            _serial.DataReceived -= _serial_DataReceived;
            if (_serial == null) return;
            if (!_serial.IsOpen) return;

            _serial.Close();
            _serial.Dispose();
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
            string raw = ser.ReadLine(); //Use ReadLine to make sure each msg is read individually (must add NewLine="\r" before starting RX above)

            if (raw.Length == 21) //'t' msg is always 21 chars long
            {
                byte[] payload = new byte[8];

                CanInterfaceData data = new CanInterfaceData
                {
                    Id = 1111,
                    Len = 8,
                    Payload = payload
                };

                OnDataReceived(new CanDataEventArgs(data));
            }
        }

        public bool Start()
        {
            _serial.DataReceived += _serial_DataReceived;
            try 
            {
                _serial.Open();
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
            if (!(canData.Payload.Length == 8))
                return false;

            try
            {
                byte[] data = new byte[21];

                _serial.Write(data, 0, 21);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}

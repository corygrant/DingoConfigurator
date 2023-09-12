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
            byte[] raw = new byte[12];
            ser.Read(raw, 0, 12);
            Console.WriteLine(raw);
            /*
            string raw = ser.ReadLine(); //Use ReadLine to make sure each msg is read individually (must add NewLine="\r" before starting RX above)
            byte[] bytes = Encoding.UTF8.GetBytes(raw);
            if (raw.Length == 12) //'t' msg is always 12 chars long
            {
                byte[] payload = new byte[8];

                payload[0] = (byte)int.Parse(raw.Substring(4, 1), System.Globalization.NumberStyles.HexNumber);
                payload[1] = (byte)int.Parse(raw.Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
                payload[2] = (byte)int.Parse(raw.Substring(6, 1), System.Globalization.NumberStyles.HexNumber);
                payload[3] = (byte)int.Parse(raw.Substring(7, 1), System.Globalization.NumberStyles.HexNumber);
                payload[4] = (byte)int.Parse(raw.Substring(8, 1), System.Globalization.NumberStyles.HexNumber);
                payload[5] = (byte)int.Parse(raw.Substring(9, 1), System.Globalization.NumberStyles.HexNumber);
                payload[6] = (byte)int.Parse(raw.Substring(10, 1), System.Globalization.NumberStyles.HexNumber);
                payload[7] = (byte)int.Parse(raw.Substring(11, 1), System.Globalization.NumberStyles.HexNumber);

                //payload[0] = bytes[4];
                //payload[1] = bytes[5];
                //payload[2] = bytes[6];
                //payload[3] = bytes[7];
                //payload[4] = bytes[8];
                //payload[5] = bytes[9];
                //payload[6] = bytes[10];
                //payload[7] = bytes[11];

                CanInterfaceData data = new CanInterfaceData
                {
                    Id = bytes[2] + (bytes[1] << 8),
                    Len = bytes[3],
                    Payload = payload
                };

                OnDataReceived(new CanDataEventArgs(data));
            }
            */
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

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public enum USB2CAN_Bitrate
    {
        BITRATE_10K = 0,
        BITRATE_20K,
        BITRATE_50K,
        BITRATE_100K,
        BITRATE_125K,
        BITRATE_250K,
        BITRATE_500K,
        BITRATE_750K,
        BITRATE_1000K,
        BITRATE_INVALID
    }

    public class USB2CAN : ICanInterface
    {
        private SerialPort _serial;

        private USB2CAN_Bitrate bitrate = USB2CAN_Bitrate.BITRATE_500K;

        public DataReceivedHandler DataReceived { get; set; }

        private int _rxTimeDelta;
        public int RxTimeDelta { get => _rxTimeDelta;}
        private Stopwatch _rxStopwatch;
        private bool _disposed = false;

        private void OnDataReceived(CanDataEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public bool Init(string port, CanInterfaceBaudRate baud)
        {
            bitrate = ConvertBitrate(baud);

            try
            {
                _serial = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                _serial.Handshake = Handshake.None;
                _serial.NewLine = "\r";
                _serial.DataReceived += _serial_DataReceived;
                _serial.Open();

                _rxStopwatch = Stopwatch.StartNew();
            }
            catch (Exception e)
            {
                _serial.DataReceived -= _serial_DataReceived;
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
                    if ((len > 0) && (raw.Length >= 5 + len * 2))
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
            if (!_serial.IsOpen) return false;

            //byte[] data = new byte[8];
            string sData = "";
            try
            {
                //data[0] = (byte)'C';
                sData = "C\r";
                _serial.Write(Encoding.ASCII.GetBytes(sData), 0, Encoding.ASCII.GetByteCount(sData));

                //Set bitrate
                //data[0] = (byte)'S';
                //data[1] = Convert.ToByte(bitrate);
                sData = "S" + (int)bitrate + "\r";
                _serial.Write(Encoding.ASCII.GetBytes(sData), 0, Encoding.ASCII.GetByteCount(sData));

                //Open slcan
                //data[0] = (byte)'O';
                sData = "O\r";
                //_serial.Write(data, 0, 1);
                _serial.Write(Encoding.ASCII.GetBytes(sData), 0, Encoding.ASCII.GetByteCount(sData));

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;

        }

        public bool Stop()
        {
            if(_serial == null) return false;
            if (!_serial.IsOpen) return false;

            //byte[] data = new byte[8];
            //data[0] = (byte)'C';
            //_serial.Write(data, 0, 1);
            string sData = "";
            sData = "C\r";
            _serial.Write(Encoding.ASCII.GetBytes(sData), 0, Encoding.ASCII.GetByteCount(sData));

            _serial.DataReceived -= _serial_DataReceived;

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
                byte[] data = new byte[22];
                data[0] = (byte)'t';
                data[1] = (byte)((canData.Id & 0xF00) >> 8);
                data[2] = (byte)((canData.Id & 0xF0) >> 4);
                data[3] = (byte)(canData.Id & 0xF);
                data[4] = (byte)canData.Len;

                int lastByte = 0;

                for (int i = 0; i < canData.Len; i++)
                {
                    data[5 + (i * 2)] = Convert.ToByte((canData.Payload[i] & 0xF0) >> 4);
                    data[6 + (i * 2)] = Convert.ToByte(canData.Payload[i] & 0xF);
                    lastByte = 6 + (i * 2);
                }

                data[lastByte + 1] = Convert.ToByte('\r');

                for(int i = 1; i < lastByte + 1; i++)
                {
                    if (data[i] < 0xA)
                        data[i] += 0x30;
                    else
                        data[i] += 0x37;
                }

                _serial.Write(data, 0, lastByte + 2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private USB2CAN_Bitrate ConvertBitrate(CanInterfaceBaudRate baud)
        {
            switch (baud)
            {
                case CanInterfaceBaudRate.BAUD_1000K:
                    return USB2CAN_Bitrate.BITRATE_1000K;

                case CanInterfaceBaudRate.BAUD_500K:
                    return USB2CAN_Bitrate.BITRATE_500K;

                case CanInterfaceBaudRate.BAUD_250K:
                    return USB2CAN_Bitrate.BITRATE_250K;

                case CanInterfaceBaudRate.BAUD_125K:
                    return USB2CAN_Bitrate.BITRATE_125K;

                default:
                    return USB2CAN_Bitrate.BITRATE_500K;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_serial != null)
                    {
                        lock (_serial)
                        {
                            if (_serial.IsOpen)
                            {
                                Stop();
                                _serial.DataReceived -= _serial_DataReceived;
                                _serial.Close();
                            }
                            _serial.Dispose();
                            _serial = null;
                        }
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

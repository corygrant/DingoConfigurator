using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InnoMakerUsb2CanLib;
using LibUsbDotNet.Main;
using LibUsbDotNet;

namespace CanInterfaces
{
    public class InnoMaker : ICanInterface
    {
        private List<InnoMakerDevice> InnoMakerDevices = new List<InnoMakerDevice>();
        private uint pid = 24687u;

        private uint vid = 7504u;
        public uint Can_Eff_Flag = 2147483648u;

        public uint Can_Rtr_Flag = 1073741824u;

        public uint Can_Err_Flag = 536870912u;

        public uint Can_Sff_Mask = 127u;

        public uint Can_Eff_Mask = 536870911u;

        public uint Can_Err_Mask = 536870911u;

        public uint Can_Id_Mask = 536870911u;

        public uint Can_Err_Ctrl = 4u;

        public uint Can_Err_BusOff = 64u;

        public uint Can_Err_Restarted = 256u;

        public uint Can_Err_Ctrl_UnSpec = 0u;

        public uint Can_Err_Ctrl_Rx_Overflow = 1u;

        public uint Can_Err_Ctrl_Tx_Overflow = 2u;

        public uint Can_Err_Ctrl_Rx_Warning = 4u;

        public uint Can_Err_Ctrl_Tx_Warning = 8u;

        public uint Can_Err_Ctrl_Rx_Passive = 16u;

        public uint Can_Err_Ctrl_Tx_Passive = 32u;

        private bool _disposed = false;

        public DataReceivedHandler DataReceived { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int RxTimeDelta => throw new NotImplementedException();

        public bool Init(string port, CanInterfaceBaudRate baud)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        public bool Write(CanInterfaceData canData)
        {
            throw new NotImplementedException();
        }

        public bool scanInnoMakerDevices()
        {
            foreach (InnoMakerDevice innoMakerDevice2 in InnoMakerDevices)
            {
                if (innoMakerDevice2.InnoMakerDev != null)
                {
                    innoMakerDevice2.InnoMakerDev.Close();
                    innoMakerDevice2.InnoMakerDev = null;
                }
            }

            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            InnoMakerDevices.Clear();
            foreach (UsbRegistry item in allDevices)
            {
                if (item.Pid == pid && item.Vid == vid)
                {
                    InnoMakerDevice innoMakerDevice = new InnoMakerDevice();
                    if (item.DevicePath.Split('#').Length > 2)
                    {
                        innoMakerDevice.deviceId = item.DevicePath.Split('#')[2];
                        innoMakerDevice.deviceName = "CAN " + allDevices.IndexOf(item);
                    }
                    else
                    {
                        innoMakerDevice.deviceId = item.DevicePath;
                        innoMakerDevice.deviceName = "CAN " + allDevices.IndexOf(item);
                    }

                    innoMakerDevice.isOpen = false;
                    innoMakerDevice.usbReg = item;
                    InnoMakerDevices.Add(innoMakerDevice);
                }
            }

            return true;
        }

        public int getInnoMakerDeviceCount()
        {
            return InnoMakerDevices.Count;
        }

        public bool openInnoMakerDevice(InnoMakerDevice device)
        {
            if (device.InnoMakerDev == null)
            {
                device.usbReg.Open(out device.InnoMakerDev);
                device.isOpen = true;
                if (device.InnoMakerDev == null)
                {
                    return false;
                }

                return true;
            }

            if (device.InnoMakerDev.Open())
            {
                device.isOpen = true;
                return true;
            }

            return false;
        }

        public bool closeInnoMakerDevice(InnoMakerDevice device)
        {
            if (device == null)
            {
                return true;
            }

            if (device.InnoMakerDev == null)
            {
                device.isOpen = false;
                return true;
            }

            if (device.InnoMakerDev.Close())
            {
                device.isOpen = false;
                return true;
            }

            return false;
        }

        public InnoMakerDevice getInnoMakerDevice(int devIndex)
        {
            if (devIndex < InnoMakerDevices.Count)
            {
                return InnoMakerDevices[devIndex];
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    
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

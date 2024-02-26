using CanDevices.DingoPdm;
using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices
{
    public interface ICanDevice
    {
        string Name { get; set; }
        int BaseId { get; set; }
        bool IsConnected { get;}
        DateTime LastRxTime { get;}
        void UpdateIsConnected();
        bool Read(int id, byte[] data, ref List<CanDeviceResponse> queue);
        void Clear();
        bool IsPriorityMsg(int id);
        bool InIdRange(int id);
        List<CanDeviceResponse> GetUploadMessages();
        List<CanDeviceResponse> GetDownloadMessages();
        List<CanDeviceResponse> GetUpdateMessages(int newId);
        CanDeviceResponse GetBurnMessage();
    }
}

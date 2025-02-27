using CanInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices
{
    public interface ICanDevice
    {
        string Name { get; set; }
        int BaseId { get; set; }

        [JsonIgnore]
        bool IsConnected { get;}

        [JsonIgnore]
        DateTime LastRxTime { get;}
        void UpdateIsConnected();
        bool Read(int id, byte[] data, ref ConcurrentDictionary<(int BaseId, int Prefix, int Index), CanDeviceResponse> queue);
        void Clear();
        bool IsPriorityMsg(int id);
        bool InIdRange(int id);
        List<CanDeviceResponse> GetUploadMessages();
        List<CanDeviceResponse> GetDownloadMessages();
        List<CanDeviceResponse> GetUpdateMessages(int newId);
        CanDeviceResponse GetBurnMessage();
        CanDeviceResponse GetSleepMessage();
        CanDeviceResponse GetVersionMessage();

    }
}

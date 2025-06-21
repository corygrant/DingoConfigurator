using System;
using System.Collections.Generic;
using CanDevices.DingoPdm;

namespace CanDevices.SoftButtonBox
{
    public interface IKeypadEmulator
    {
        KeypadModel Model { get; }
        int BaseCanId { get; set; }
        bool[] ButtonStates { get; }
        int[] DialValues { get; }
        int NumButtons { get; }
        int NumDials { get; }
        bool ColorsEnabled { get; }
        
        List<CanDeviceResponse> GenerateButtonStateMessages();
        List<CanDeviceResponse> GenerateDialStateMessages();
        List<CanDeviceResponse> GenerateHeartbeatMessage();
        bool InIdRange(int id);
        bool ProcessIncomingMessage(int canId, byte[] data);
        bool IsLedMessage(int canId);
        void SetButtonState(int buttonIndex, bool pressed);
        void SetDialValue(int dialIndex, int value);
        void Reset();
        string GetLedColorName(int buttonIndex);
        string GetLedBlinkColorName(int buttonIndex);
    }
}
using System;
using System.Collections.Generic;
using CanDevices.DingoPdm;

namespace CanDevices.SoftButtonBox
{
    public class GrayhillKeypadEmulator : KeypadEmulatorBase
    {
        private const int BUTTON_STATE_OFFSET = 0x180;
        private const int SET_LED_OFFSET = 0x200;
        private const int LED_BRIGHTNESS_OFFSET = 0x400;
        private const int HEARTBEAT_OFFSET = 0x700;

        private bool[] _ledStates;
        private byte _brightness = 255;

        public GrayhillKeypadEmulator(KeypadModel model, int baseCanId) : base(model, baseCanId)
        {
            _ledStates = new bool[NumButtons];
        }

        protected override void InitializeMessageHandlers()
        {
            MessageHandlers = new Dictionary<int, Func<int, byte[], bool>>
            {
                { SET_LED_OFFSET, ProcessSetLedMessage },
                { LED_BRIGHTNESS_OFFSET, ProcessLedBrightnessMessage }
            };
        }

        public override List<CanDeviceResponse> GenerateButtonStateMessages()
        {
            var messages = new List<CanDeviceResponse>();
            
            byte[] data = new byte[8];
            
            for (int i = 0; i < NumButtons && i < 64; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                
                if (ButtonStates[i])
                {
                    data[byteIndex] |= (byte)(1 << bitIndex);
                }
            }
            
            messages.Add(CreateCanMessage(BaseCanId + BUTTON_STATE_OFFSET, data));
            return messages;
        }

        public override List<CanDeviceResponse> GenerateDialStateMessages()
        {
            return new List<CanDeviceResponse>();
        }

        public override List<CanDeviceResponse> GenerateHeartbeatMessage()
        {
            var messages = new List<CanDeviceResponse>();
            byte[] data = { 0x05 };
            messages.Add(CreateCanMessage(BaseCanId + HEARTBEAT_OFFSET, data));
            return messages;
        }

        private bool ProcessSetLedMessage(int canId, byte[] data)
        {
            if (data.Length > 0)
            {
                for (int i = 0; i < NumButtons && i < data.Length * 8; i++)
                {
                    int byteIndex = i / 8;
                    int bitIndex = i % 8;
                    
                    if (byteIndex < data.Length)
                    {
                        _ledStates[i] = (data[byteIndex] & (1 << bitIndex)) != 0;
                    }
                }
                return true;
            }
            return false;
        }

        private bool ProcessLedBrightnessMessage(int canId, byte[] data)
        {
            if (data.Length >= 1)
            {
                _brightness = data[0];
                return true;
            }
            return false;
        }

        public bool GetLedState(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < NumButtons)
            {
                return _ledStates[buttonIndex];
            }
            return false;
        }

        public byte GetBrightness()
        {
            return _brightness;
        }

        public override bool IsLedMessage(int canId)
        {
            int messageOffset = canId - BaseCanId;
            return messageOffset == SET_LED_OFFSET || 
                   messageOffset == LED_BRIGHTNESS_OFFSET;
        }

        public override string GetLedColorName(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < NumButtons)
            {
                return _ledStates[buttonIndex] ? "White" : "Off";
            }
            return "Off";
        }
    }
}
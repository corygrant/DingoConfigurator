using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CanDevices.DingoPdm;

namespace CanDevices.SoftButtonBox
{
    public class BlinkKeypadEmulator : KeypadEmulatorBase
    {
        private const int BUTTON_STATE_OFFSET = 0x180;
        private const int SET_LED_OFFSET = 0x200;
        private const int DIAL_STATE_A_OFFSET = 0x280;
        private const int SET_LED_BLINK_OFFSET = 0x300;
        private const int DIAL_STATE_B_OFFSET = 0x380;
        private const int LED_BRIGHTNESS_OFFSET = 0x400;
        private const int ANALOG_INPUT_OFFSET = 0x480;
        private const int BACKLIGHT_OFFSET = 0x500;
        private const int HEARTBEAT_OFFSET = 0x700;

        private const int MAX_NUM_BUTTONS = 15;

        private BlinkMarineButtonColor[] _ledStates;
        private BlinkMarineButtonColor[] _ledBlinkStates;
        private byte _buttonBrightness;
        private byte _backlightBrightness;
        private BlinkMarineBacklightColor _backlightColor;
        private bool[,] _dialLedStates;
        private bool[] _centralLedState;

        public BlinkKeypadEmulator(KeypadModel model, int baseCanId) : base(model, baseCanId)
        {
            _ledStates = new BlinkMarineButtonColor[NumButtons];
            _ledBlinkStates = new BlinkMarineButtonColor[NumButtons];

            if(Model == KeypadModel.Blink13Key_2Dial)
            {
                _dialLedStates = new bool[2,16];
            }

            if (Model == KeypadModel.BlinkRacepad)
            {
                _dialLedStates = new bool[4,8];
                _centralLedState = new bool[12];
            }
        }


        protected override void InitializeMessageHandlers()
        {
            MessageHandlers = new Dictionary<int, Func<int, byte[], bool>>
            {
                { SET_LED_OFFSET, ProcessSetLedMessage },
                { SET_LED_BLINK_OFFSET, ProcessSetLedBlinkMessage },
                { LED_BRIGHTNESS_OFFSET, ProcessLedBrightnessMessage },
                { BACKLIGHT_OFFSET, ProcessBacklightMessage }
            };
        }

        public override List<CanDeviceResponse> GenerateButtonStateMessages()
        {
            var messages = new List<CanDeviceResponse>();
            
            byte[] data = new byte[8];
            
            for (int i = 0; i < NumButtons && i < MAX_NUM_BUTTONS; i++)
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
            var messages = new List<CanDeviceResponse>();
            
            if (NumDials > 0)
            {
                byte[] dataA = new byte[8];
                byte[] dataB = new byte[8];
                
                for (int i = 0; i < NumDials && i < 4; i++)
                {
                    int value = DialValues[i];
                    dataA[i * 2] = (byte)(value & 0xFF);
                    dataA[i * 2 + 1] = (byte)((value >> 8) & 0xFF);
                    
                    dataB[i * 2] = (byte)((value >> 16) & 0xFF);
                    dataB[i * 2 + 1] = (byte)((value >> 24) & 0xFF);
                }
                
                messages.Add(CreateCanMessage(BaseCanId + DIAL_STATE_A_OFFSET, dataA));
                messages.Add(CreateCanMessage(BaseCanId + DIAL_STATE_B_OFFSET, dataB));
            }
            
            return messages;
        }

        public override List<CanDeviceResponse> GenerateHeartbeatMessage()
        {
            var messages = new List<CanDeviceResponse>();
            byte[] data = { 0x05 };
            messages.Add(CreateCanMessage(BaseCanId + HEARTBEAT_OFFSET, data));
            return messages;
        }

        private bool DecodeStackedLedFormat(byte[] data, out BlinkMarineButtonColor[] colors)
        {
            colors = new BlinkMarineButtonColor[NumButtons];

            int[] rawLed = new int[NumButtons];

            int bitIndex = 0;

            ulong message = BitConverter.ToUInt64(data, 0);

            // Extract red bits for all buttons first
            for (int i = 0; i < NumButtons; i++)
            {
                if (((message >> bitIndex++) & 1) == 1)
                    rawLed[i] |= 0x01;
            }

            // Then extract green bits for all buttons
            for (int i = 0; i < NumButtons; i++)
            {
                if (((message >> bitIndex++) & 1) == 1)
                    rawLed[i] |= 0x02;
            }

            // Finally extract blue bits for all buttons
            for (int i = 0; i < NumButtons; i++)
            {
                if (((message >> bitIndex++) & 1) == 1)
                    rawLed[i] |= 0x04;
            }

            for (int i = 0; i < NumButtons; i++)
            {
                //if (((message >> bitIndex++) & 1) == 1)
                    colors[i] = (BlinkMarineButtonColor)rawLed[i];
            }

            return true;

        }

        private bool DecodePaddedLedFormat(byte[] data, out BlinkMarineButtonColor[] colors)
        {
            colors = new BlinkMarineButtonColor[NumButtons];

            ulong message = BitConverter.ToUInt64(data, 0);

            int bytesPerColor = (NumButtons + 7) / 8; // Ceiling division

            int[] rawLed = new int[NumButtons];

            // Extract red bits for all buttons
            for (int i = 0; i < NumButtons; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                int bitPosition = (byteIndex * 8) + bitIndex;
                if (((message >> bitPosition) & 1) == 1)
                    rawLed[i] |= 0x01;
            }

            // Extract green bits for all buttons
            for (int i = 0; i < NumButtons; i++)
            {
                int byteIndex = bytesPerColor + (i / 8);
                int bitIndex = i % 8;
                int bitPosition = (byteIndex * 8) + bitIndex;
                if (((message >> bitPosition) & 1) == 1)
                    rawLed[i] |= 0x02;
            }

            // Extract blue bits for all buttons
            for (int i = 0; i < NumButtons; i++)
            {
                int byteIndex = 2 * bytesPerColor + (i / 8);
                int bitIndex = i % 8;
                int bitPosition = (byteIndex * 8) + bitIndex;
                if (((message >> bitPosition) & 1) == 1)
                    rawLed[i] |= 0x04;
            }

            for (int i = 0; i < NumButtons; i++)
            {
                colors[i] = (BlinkMarineButtonColor)rawLed[i];
            }

            return true;
        }

        private bool ProcessSetLedMessage(int canId, byte[] data)
        {
            switch (Model)
            {
                case KeypadModel.Blink2Key:
                case KeypadModel.Blink4Key:
                case KeypadModel.Blink5Key:
                case KeypadModel.Blink6Key:
                case KeypadModel.Blink8Key:
                case KeypadModel.Blink10Key:
                    return DecodePaddedLedFormat(data, out _ledStates);
                case KeypadModel.Blink12Key:
                    return DecodeStackedLedFormat(data, out _ledStates);
                case KeypadModel.Blink15Key:
                case KeypadModel.Blink13Key_2Dial:
                case KeypadModel.BlinkRacepad:
                    return DecodePaddedLedFormat(data, out _ledStates);
            }

            return false;
        }

        private bool ProcessSetLedBlinkMessage(int canId, byte[] data)
        {
            switch (Model)
            {
                case KeypadModel.Blink2Key:
                case KeypadModel.Blink4Key:
                case KeypadModel.Blink5Key:
                case KeypadModel.Blink6Key:
                case KeypadModel.Blink8Key:
                case KeypadModel.Blink10Key:
                    return DecodePaddedLedFormat(data, out _ledBlinkStates);
                case KeypadModel.Blink12Key:
                    return DecodeStackedLedFormat(data, out _ledBlinkStates);
                case KeypadModel.Blink15Key:
                case KeypadModel.Blink13Key_2Dial:
                case KeypadModel.BlinkRacepad:
                    return DecodePaddedLedFormat(data, out _ledBlinkStates);
            }

            return false;
        }

        private bool ProcessLedBrightnessMessage(int canId, byte[] data)
        {
            if (NumDials == 0)
            {
                _buttonBrightness = data[0];
                return true;
            }

            if (Model == KeypadModel.Blink13Key_2Dial)
            {
                for (int i = 0; i < NumDials; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        _dialLedStates[i, j] = (data[j / 8] & (1 << (j % 8))) != 0;
                    }
                }
            }

            if (Model == KeypadModel.BlinkRacepad)
            {
                for (int i = 0; i < NumDials; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        _dialLedStates[i, j] = (data[j / 8] & (1 << (j % 8))) != 0;
                    }
                }

                for (int j = 0; j < 12; j++)
                {
                    _centralLedState[j] = (data[(j / 8) + 4] & (1 << (j % 8))) != 0;
                }
            }

            return true;
        }

        private bool ProcessBacklightMessage(int canId, byte[] data)
        {
            if (data.Length != 8)
                return false;

            _backlightBrightness = data[0];
            _backlightColor = (BlinkMarineBacklightColor)data[1];

            return true;
        }

        public byte GetLedState(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < NumButtons)
            {
                return Convert.ToByte(_ledStates[buttonIndex]);
            }
            return 0;
        }

        public byte GetLedBlinkState(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < NumButtons)
            {
                return Convert.ToByte(_ledBlinkStates[buttonIndex]);
            }
            return 0;
        }

        public byte GetBacklightBrightness()
        {
            return _backlightBrightness;
        }

        public byte GetButtonBrightness()
        {
            return _buttonBrightness;
        }

        public bool GetDialLed(int dialIndex, int ledIndex)
        {
            if (Model == KeypadModel.Blink13Key_2Dial)
            {
                if (dialIndex >= 0 && dialIndex < NumDials && ledIndex >= 0 && ledIndex < 16)
                {
                    return _dialLedStates[dialIndex, ledIndex];
                }
            }
            else if (Model == KeypadModel.BlinkRacepad)
            {
                if (dialIndex >= 0 && dialIndex < NumDials && ledIndex >= 0 && ledIndex < 8)
                {
                    return _dialLedStates[dialIndex, ledIndex];
                }
            }
            return false;
        }

        public bool GetCentralLedState(int ledIndex)
        {
            if (Model == KeypadModel.BlinkRacepad && ledIndex >= 0 && ledIndex < 12)
            {
                return _centralLedState[ledIndex];
            }
            return false;
        }

        public byte GetBacklightColor()
        {
            return Convert.ToByte(_backlightColor);
        }

        public override bool IsLedMessage(int canId)
        {
            int messageOffset = canId - BaseCanId;
            return messageOffset == SET_LED_OFFSET || 
                   messageOffset == SET_LED_BLINK_OFFSET || 
                   messageOffset == LED_BRIGHTNESS_OFFSET ||
                   messageOffset == BACKLIGHT_OFFSET;
        }

        public override string GetLedColorName(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < NumButtons)
            {
                return _ledStates[buttonIndex].ToString();
            }
            return "Off";
        }
    }
}
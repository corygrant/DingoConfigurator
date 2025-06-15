using System;
using System.Collections.Generic;
using System.Linq;
using CanDevices.DingoPdm;
using CanInterfaces;

namespace CanDevices.SoftButtonBox
{
    public abstract class KeypadEmulatorBase : IKeypadEmulator
    {
        public KeypadModel Model { get; protected set; }
        public int BaseCanId { get; set; }
        public bool[] ButtonStates { get; protected set; }
        public int[] DialValues { get; protected set; }
        public int NumButtons { get; protected set; }
        public int NumDials { get; protected set; }
        public bool ColorsEnabled { get; protected set; }

        protected Dictionary<int, Func<int, byte[], bool>> MessageHandlers { get; set; }

        protected KeypadEmulatorBase(KeypadModel model, int baseCanId)
        {
            Model = model;
            BaseCanId = baseCanId;
            InitializeModelProperties();
            InitializeStates();
            InitializeMessageHandlers();
        }

        protected virtual void InitializeModelProperties()
        {
            switch (Model)
            {
                case KeypadModel.Blink2Key:
                    NumButtons = 2; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink4Key:
                    NumButtons = 4; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink5Key:
                    NumButtons = 5; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink6Key:
                    NumButtons = 6; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink8Key:
                    NumButtons = 8; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink10Key:
                    NumButtons = 10; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink12Key:
                    NumButtons = 12; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink15Key:
                    NumButtons = 15; NumDials = 0; ColorsEnabled = true; break;
                case KeypadModel.Blink13Key_2Dial:
                    NumButtons = 13; NumDials = 2; ColorsEnabled = true; break;
                case KeypadModel.BlinkRacepad:
                    NumButtons = 20; NumDials = 4; ColorsEnabled = true; break;
                case KeypadModel.Grayhill6Key:
                    NumButtons = 6; NumDials = 0; ColorsEnabled = false; break;
                case KeypadModel.Grayhill8Key:
                    NumButtons = 8; NumDials = 0; ColorsEnabled = false; break;
                case KeypadModel.Grayhill15Key:
                    NumButtons = 15; NumDials = 0; ColorsEnabled = false; break;
                case KeypadModel.Grayhill20Key:
                    NumButtons = 20; NumDials = 0; ColorsEnabled = false; break;
                default:
                    NumButtons = 0; NumDials = 0; ColorsEnabled = false; break;
            }
        }

        protected virtual bool InIdRange(int id)
        {
            int messageOffset = id - BaseCanId;
            return MessageHandlers.ContainsKey(messageOffset);
        }

        protected virtual void InitializeStates()
        {
            ButtonStates = new bool[Math.Max(NumButtons, 1)];
            DialValues = new int[Math.Max(NumDials, 1)];
        }

        protected abstract void InitializeMessageHandlers();

        public virtual bool ProcessIncomingMessage(int canId, byte[] data)
        {
            int messageOffset = canId - BaseCanId;
            if (MessageHandlers.ContainsKey(messageOffset))
            {
                return MessageHandlers[messageOffset](canId, data);
            }
            return false;
        }

        public abstract List<CanDeviceResponse> GenerateButtonStateMessages();
        public abstract List<CanDeviceResponse> GenerateDialStateMessages();
        public abstract List<CanDeviceResponse> GenerateHeartbeatMessage();

        protected CanDeviceResponse CreateCanMessage(int canId, byte[] data)
        {
            return new CanDeviceResponse
            {
                Prefix = 0,
                Index = 0,
                Data = new CanInterfaceData
                {
                    Id = canId,
                    Len = data.Length,
                    Payload = data
                },
                MsgDescription = $"SBB, {Model}"
            };
        }
    }
}
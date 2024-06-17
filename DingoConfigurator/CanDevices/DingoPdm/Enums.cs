using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{

    public enum DeviceState
    {
        PowerOn,
        Starting,
        Run,
        Sleep,
        Wakeup,
        OverTemp,
        Error
    }

    public enum MessagePrefix
    {
        Burn = 'B',
        Sleep = 'Q',
        Version = 'V',
        CAN = 'C',
        Input = 'I',
        Output = 'O',
        VirtualInput = 'U',
        Flasher = 'H',
        Wiper = 'W',
        WiperSpeed = 'P',
        WiperDelay = 'Y',
        StarterDisable = 'D',
        CANInput = 'N',
        Info = 'F',
        Warning = 'R',
        Error = 'E'
    }

    public enum MessageSrc
{
        StatePowerOn = 1,
        StateStarting,
        StateRun,
        StateOvertemp,
        StateError,
        StateSleep,
        StateWake,
        OverCurrent,
        BatteryVoltage,
        CAN,
        USB,
        OverTemp,
        Config,
        FRAM,
        ADC,
        I2C,
        TempSensor,
        USBConnected
    }

    public enum OutState
    {
        Off,
        On,
        Overcurrent,
        Fault
    }

    public enum WiperMode
    {
        DigIn,
        IntIn,
        MixIn
    }

    public enum WiperState
    {
        Parked,
        Parking,
        SlowOn,
        FastOn,
        InterPause,
        InterOn,
        Wash,
        Swipe
    }

    public enum WiperSpeed
    {
        Park,
        Slow,
        Fast,
        Inter1,
        Inter2,
        Inter3,
        Inter4,
        Inter5,
        Inter6
    }

    public enum VarMap
    {
        None = 0,
        DigIn1,
        DigIn2,
        CANIn1,
        CANIn2,
        CANIn3,
        CANIn4,
        CANIn5,
        CANIn6,
        CANIn7,
        CANIn8,
        CANIn9,
        CANIn10,
        CANIn11,
        CANIn12,
        CANIn13,
        CANIn14,
        CANIn15,
        CANIn16,
        CANIn17,
        CANIn18,
        CANIn19,
        CANIn20,
        CANIn21,
        CANIn22,
        CANIn23,
        CANIn24,
        CANIn25,
        CANIn26,
        CANIn27,
        CANIn28,
        CANIn29,
        CANIn30,
        CANIn31,
        CANIn32,
        VirtIn1,
        VirtIn2,
        VirtIn3,
        VirtIn4,
        VirtIn5,
        VirtIn6,
        VirtIn7,
        VirtIn8,
        VirtIn9,
        VirtIn10,
        VirtIn11,
        VirtIn12,
        VirtIn13,
        VirtIn14,
        VirtIn15,
        VirtIn16,
        Output1,
        Output2,
        Output3,
        Output4,
        Output5,
        Output6,
        Output7,
        Output8,
        WiperSlow,
        WiperFast,
        AlwaysOn
    }


    public enum CanSpeed
    {
        Bitrate_10K = 0,
        Bitrate_20K = 1,
        Bitrate_50K = 2,
        Bitrate_100K = 3,
        Bitrate_125K = 4,
        Bitrate_250K = 5,
        Bitrate_500K = 6,
        Bitrate_750K = 7,
        Bitrate_1000K = 8
    }

    public enum InputMode
    {
        Num,
        Momentary,
        Latching
    }
    
    public enum InputPull
    {
        NoPull,
        PullUp,
        PullDown
    }

    public enum Conditional
    {
        And,
        Or,
        Nor
    }

    public enum Operator
    {
        Equal,
        GreaterThan,
        LessThan,
        BitwiseAnd,
        BitwiseNand
    }

    public enum ResetMode
    {
        None,
        Count,
        Endless
    }
}

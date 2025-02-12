﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{

    public enum DeviceState
    {
        Run,
        Sleep,
        OverTemp,
        Error
    }

    public enum MessagePrefix
    {
        Null = 0,
        Can = 1,
        Inputs = 5,
        //InputsName = 6, //Future use
        Outputs = 10,
        //OutputsName = 11, //Future use
        VirtualInputs = 15,
        //VirtualInputsName = 16, //Future use
        Wiper = 20,
        WiperSpeed = 21,
        WiperDelays = 22,
        Flashers = 25,
        //FlashersName = 26, //Future use
        StarterDisable = 30,
        CanInputs = 35,
        CanInputsId = 36,
        //CanInputsName = 37, //Future use
        Version = 120,
        Sleep = 121,
        Bootloader = 125,
        BurnSettings = 127
    }

    public enum MessageType
    {
        Info = 'F',
        Warning = 'R',
        Error = 'E',
        Wake = '!'
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
        Flasher1,
        Flasher2,
        Flasher3,
        Flasher4,
        AlwaysOn
    }


    public enum CanSpeed
    {
        BAUD_1000K = 0,
        BAUD_500K = 1,
        BAUD_250K = 2,
        BAUD_125K = 3
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

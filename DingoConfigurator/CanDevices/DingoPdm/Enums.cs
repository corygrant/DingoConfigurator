using System;
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
        OutputsPwm = 11,
        //OutputsName = 12, //Future use
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
		Counter = 40,
		//CountersName = 41, //Future use
		Conditions = 45,
		//ConditionsName = 46, //Future use
        Keypad = 50,
        KeypadLed = 51,
        KeypadButton = 52,
        KeypadButtonLed = 53,
        KeypadDial = 54,
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
        CANInVal1,
        CANInVal2,
        CANInVal3,
        CANInVal4,
        CANInVal5,
        CANInVal6,
        CANInVal7,
        CANInVal8,
        CANInVal9,
        CANInVal10,
        CANInVal11,
        CANInVal12,
        CANInVal13,
        CANInVal14,
        CANInVal15,
        CANInVal16,
        CANInVal17,
        CANInVal18,
        CANInVal19,
        CANInVal20,
        CANInVal21,
        CANInVal22,
        CANInVal23,
        CANInVal24,
        CANInVal25,
        CANInVal26,
        CANInVal27,
        CANInVal28,
        CANInVal29,
        CANInVal30,
        CANInVal31,
        CANInVal32,
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
        Output1_On,
        Output1_OC,
        Output1_Fault,
        Output2_On,
        Output2_OC,
        Output2_Fault,
        Output3_On,
        Output3_OC,
        Output3_Fault,
        Output4_On,
        Output4_OC,
        Output4_Fault,
        Output5_On,
        Output5_OC,
        Output5_Fault,
        Output6_On,
        Output6_OC,
        Output6_Fault,
        Output7_On,
        Output7_OC,
        Output7_Fault,
        Output8_On,
        Output8_OC,
        Output8_Fault,
        WiperSlow,
        WiperFast,
        WiperPark,
        WiperInter,
        WiperWash,
        WiperSwipe,
        Flasher1,
        Flasher2,
        Flasher3,
        Flasher4,
        Counter1,
        Counter2,
        Counter3,
        Counter4,
        Condition1,
		Condition2,
		Condition3,
		Condition4,
		Condition5,
		Condition6,
		Condition7,
		Condition8,
		Condition9,
		Condition10,
		Condition11,
		Condition12,
		Condition13,
		Condition14,
		Condition15,
		Condition16,
		Condition17,
		Condition18,
		Condition19,
		Condition20,
		Condition21,
		Condition22,
		Condition23,
		Condition24,
		Condition25,
		Condition26,
		Condition27,
		Condition28,
		Condition29,
		Condition30,
		Condition31,
		Condition32,
        Keypad1Btn1,
        Keypad1Btn2, 
        Keypad1Btn3,
        Keypad1Btn4, 
        Keypad1Btn5, 
        Keypad1Btn6,
        Keypad1Btn7, 
        Keypad1Btn8,
        Keypad1Btn9,
        Keypad1Btn10,
        Keypad1Btn11,
        Keypad1Btn12,
        Keypad1Btn13,
        Keypad1Btn14,
        Keypad1Btn15,
        Keypad1Btn16,
        Keypad1Btn17,
        Keypad1Btn18,
        Keypad1Btn19,
        Keypad1Btn20,
        Keypad2Btn1,
        Keypad2Btn2,
        Keypad2Btn3,
        Keypad2Btn4,
        Keypad2Btn5,
        Keypad2Btn6,
        Keypad2Btn7,
        Keypad2Btn8,
        Keypad2Btn9,
        Keypad2Btn10,
        Keypad2Btn11,
        Keypad2Btn12,
        Keypad2Btn13,
        Keypad2Btn14,
        Keypad2Btn15,
        Keypad2Btn16,
        Keypad2Btn17,
        Keypad2Btn18,
        Keypad2Btn19,
        Keypad2Btn20,
        Keypad1Dial1,
        Keypad1Dial2,
        Keypad1Dial3,
        Keypad1Dial4,
        Keypad2Dial1,
        Keypad2Dial2,
        Keypad2Dial3,
        Keypad2Dial4,
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
        Momentary,
        Latching
    }
    
    public enum InputPull
    {
        NoPull,
        PullUp,
        PullDown
    }

    public enum  InputEdge
    {
        Rising,
        Falling,
        Both
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
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual,
		BitwiseAnd,
		BitwiseNand
	}

    public enum ResetMode
    {
        None,
        Count,
        Endless
    }

    public enum KeypadModel
    {
        Blink2Key,
        Blink4Key,
        Blink5Key,
        Blink6Key,
        Blink8Key,
        Blink10Key,
        Blink12Key,
        Blink15Key,
        Blink13Key_2Dial,
        BlinkRacepad,
        Grayhill6Key,
        Grayhill8Key,
        Grayhill15Key,
        Grayhill20Key
    }

    public enum BlinkMarineButtonColor
    {
        Off,
        Red,
        Green,
        Orange,
        Blue,
        Violet,
        Cyan,
        White
    }

    public enum BlinkMarineBacklightColor
    {
        Off,
        Red,
        Green,
        Blue,
        Yellow,
        Cyan,
        Violet,
        White,
        Amber,
        YellowGreen
    }
}

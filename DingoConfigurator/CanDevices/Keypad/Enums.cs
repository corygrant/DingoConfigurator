using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.Keypad
{
    public enum KeypadBrand
    {
        Blink,
        Grayhill
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

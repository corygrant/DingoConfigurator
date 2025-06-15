using System;
using CanDevices.DingoPdm;

namespace CanDevices.SoftButtonBox
{
    public static class KeypadEmulatorFactory
    {
        public static IKeypadEmulator CreateEmulator(KeypadModel model, int baseCanId)
        {
            switch (model)
            {
                case KeypadModel.Blink2Key:
                case KeypadModel.Blink4Key:
                case KeypadModel.Blink5Key:
                case KeypadModel.Blink6Key:
                case KeypadModel.Blink8Key:
                case KeypadModel.Blink10Key:
                case KeypadModel.Blink12Key:
                case KeypadModel.Blink15Key:
                case KeypadModel.Blink13Key_2Dial:
                case KeypadModel.BlinkRacepad:
                    return new BlinkKeypadEmulator(model, baseCanId);

                case KeypadModel.Grayhill6Key:
                case KeypadModel.Grayhill8Key:
                case KeypadModel.Grayhill15Key:
                case KeypadModel.Grayhill20Key:
                    return new GrayhillKeypadEmulator(model, baseCanId);

                default:
                    throw new ArgumentException($"Unsupported keypad model: {model}");
            }
        }

        public static bool IsBlinkModel(KeypadModel model)
        {
            return model.ToString().StartsWith("Blink");
        }

        public static bool IsGrayhillModel(KeypadModel model)
        {
            return model.ToString().StartsWith("Grayhill");
        }

        public static bool HasDials(KeypadModel model)
        {
            return model == KeypadModel.Blink13Key_2Dial || model == KeypadModel.BlinkRacepad;
        }
    }
}
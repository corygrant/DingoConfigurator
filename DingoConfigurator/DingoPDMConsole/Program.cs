using CanInterfaces;
using CanDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingoPDMConsole
{
    enum CAN_Interface
    {
        USB2CAN,
        PCAN
    }

    internal class Program
    {
        private static CAN_Interface CAN_INTERFACE = CAN_Interface.PCAN;
        private static  ICanInterface can;

        private static DingoPdmCan _pdm;
        private static CanBoardCan _canBoard;
        
        static void Main(string[] args)
        {
            _pdm = new DingoPdmCan(2000);
            _canBoard = new CanBoardCan(1600);

            switch (CAN_INTERFACE)
            {
                case CAN_Interface.USB2CAN:
                    can = new CanInterfaces.USB2CAN();
                    break;

                case CAN_Interface.PCAN:
                    can = new CanInterfaces.PCAN();
                    break;

                default:
                    return;
            }

            can.Init();
            can.DataReceived += CanDataReceived;
            can.Start();

            Console.WriteLine("Press any key to close");
            Console.ReadKey();

            can.Stop();
        }

        static void CanDataReceived(object sender, CanDataEventArgs e)
        {
            _pdm.Read(e.canData.Id, e.canData.Payload);
            if(_canBoard.Read(e.canData.Id, e.canData.Payload))
            {
                /*
                Console.Write("Rotary: ");
                for (int i = 0; i < _canBoard.RotarySwitchPos.Count; i++)
                {
                    Console.Write($"{_canBoard.RotarySwitchPos.ElementAt(i)} ");
                }
                Console.WriteLine();

                Console.Write("Digital: ");
                for (int i = 0; i < _canBoard.DigitalIn.Count; i++)
                {
                    Console.Write($"{_canBoard.DigitalIn.ElementAt(i)} ");
                }
                Console.WriteLine();

                Console.Write("Analog: ");
                for (int i = 0; i < _canBoard.AnalogInMV.Count; i++)
                {
                    Console.Write($"{_canBoard.AnalogInMV.ElementAt(i)} ");
                }
                Console.WriteLine();
                */
            }
            Console.WriteLine($"HB: {_canBoard.Heartbeat} {_canBoard.IsConnected} {_canBoard.LastRxTime}");

            /*
            Console.Write("Inputs: ");
            for (int i = 0; i < _pdm.DigitalInputs.Count; i++)
            {
                Console.Write($"{_pdm.DigitalInputs.ElementAt(i)} ");
            }
            Console.WriteLine();

            Console.WriteLine($"{_pdm.DeviceState} {_pdm.TotalCurrent} {_pdm.BatteryVoltage} {_pdm.BoardTempF}");

            Console.Write("Current: ");
            for (int i = 0; i < _pdm.OutputCurrent.Count; i++)
            {
                Console.Write($"{_pdm.OutputCurrent.ElementAt(i)} ");
            }
            Console.WriteLine();

            Console.Write("Outputs: ");
            for (int i = 0; i < _pdm.OutputState.Count; i++)
            {
                Console.Write($"{_pdm.OutputState.ElementAt(i)} ");
            }
            Console.WriteLine();

            Console.Write("Limit: ");
            for (int i = 0; i < _pdm.OutputCurrentLimit.Count; i++)
            {
                Console.Write($"{_pdm.OutputCurrentLimit.ElementAt(i)} ");
            }
            Console.WriteLine();

            Console.Write("Reset: ");
            for (int i = 0; i < _pdm.OutputResetCount.Count; i++)
            {
                Console.Write($"{_pdm.OutputResetCount.ElementAt(i)} ");
            }
            Console.WriteLine();
            */



        }
    }
}

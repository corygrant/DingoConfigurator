using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanInterfaces
{
    public delegate void DataReceivedHandler(object sender, CanDataEventArgs e);
    public interface ICanInterface
    {
        bool Init(string port, CanInterfaceBaudRate baud);
        bool Start();
        bool Stop();
        bool Write(CanInterfaceData canData);
        DataReceivedHandler DataReceived { get; set; }
    }
}

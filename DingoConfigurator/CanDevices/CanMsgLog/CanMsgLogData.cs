using CanInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanDevices.CanMsgLog
{
    public class CanMsgLogData : NotifyPropertyChangedBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                if(value != _id)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private string _idString;
        public string IdString
        {
            get => _idString;
            set
            {
                if (value != _idString)
                {
                    _idString = value;
                    OnPropertyChanged(nameof(IdString));
                }
            }
        }

        private int _len;
        public int Len 
        {
            get => _len;
            set
            {
                if(_len != value)
                {
                    _len = value;
                    OnPropertyChanged(nameof(Len));
                }

            }
        }

        private byte[] _payload;
        public byte[] Payload
        {
            get => _payload;
            set
            {
                if( value != _payload)
                {
                    _payload = value;
                    OnPropertyChanged(nameof(Payload));
                }

            }
        }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                if( _count != value)
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        private string _payloadString;
        public string PayloadString
        {
            get => _payloadString;
            set
            {
                if( value != _payloadString)
                {
                    _payloadString = value;
                    OnPropertyChanged(nameof(PayloadString));
                }
            }
        }

        private string _note;
        public string Note
        {
            get => _note;
            set
            {
                if (value != _note)
                {
                    _note = value;
                    OnPropertyChanged(nameof(Note));
                }
            }
        }
    }
}

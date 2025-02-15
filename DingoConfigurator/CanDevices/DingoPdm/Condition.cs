using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{
    public class Condition : NotifyPropertyChangedBase
	{
		private string _name;
		[JsonPropertyName("name")]
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		private int _number;
		[JsonPropertyName("number")]
		public int Number
		{
			get => _number;
			set
			{
				if (_number != value)
				{
					_number = value;
					OnPropertyChanged(nameof(Number));
				}
			}
		}

		private int _value;
		[JsonIgnore]
		public int Value
		{
			get => _value;
			set
			{
				if (_value != value)
				{
					_value = value;
					OnPropertyChanged(nameof(Value));
				}
			}
		}

		private bool _enabled;
		[JsonPropertyName("enabled")]
		public bool Enabled
		{
			get => _enabled;
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnPropertyChanged(nameof(Enabled));
				}
			}
		}

		private VarMap _input;
		[JsonPropertyName("input")]
		public VarMap Input
		{
			get => _input;
			set
			{
				if (_input != value)
				{
					_input = value;
					OnPropertyChanged(nameof(Input));
				}
			}
		}

		private Operator _operator;
		[JsonPropertyName("operator")]
		public Operator Operator
		{
			get => _operator;
			set
			{
				if (_operator != value)
				{
					_operator = value;
					OnPropertyChanged(nameof(Operator));
				}
			}
		}

		private int _arg;
		[JsonPropertyName("arg")]
		public int Arg
		{
			get => _arg;
			set
			{
				if (_arg != value)
				{
					_arg = value;
					OnPropertyChanged(nameof(Arg));
				}
			}
		}

		public static byte[] Request(int index)
		{
			byte[] data = new byte[8];
			data[0] = Convert.ToByte(MessagePrefix.Conditions);
			data[1] = Convert.ToByte(index);
			return data;
		}

		public bool Receive(byte[] data)
		{
			if (data.Length != 6) return false;

			Enabled = Convert.ToBoolean(data[2] & 0x01);
			Operator = (Operator)(data[2] >> 4);
			Input = (VarMap)(data[3]);
			Arg = (data[4] << 8) + data[5];

			return true;
		}

		public byte[] Write()
		{
			byte[] data = new byte[8];
			data[0] = Convert.ToByte(MessagePrefix.Conditions);
			data[1] = Convert.ToByte(Number - 1);
			data[2] = Convert.ToByte((Convert.ToByte(Operator) << 4) +
					  (Convert.ToByte(Enabled)));
			data[3] = Convert.ToByte(Input);
			data[4] = Convert.ToByte(Arg >> 8);
			data[5] = Convert.ToByte(Arg & 0xFF);

			return data;
		}
	}
}

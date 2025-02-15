using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanDevices.DingoPdm
{
    public class Counter : NotifyPropertyChangedBase
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

		private VarMap _incInput;
		[JsonPropertyName("incInput")]
		public VarMap IncInput
		{
			get => _incInput;
			set
			{
				if (_incInput != value)
				{
					_incInput = value;
					OnPropertyChanged(nameof(IncInput));
				}
			}
		}

		private VarMap _decInput;
		[JsonPropertyName("decInput")]
		public VarMap DecInput
		{
			get => _decInput;
			set
			{
				if (_decInput != value)
				{
					_decInput = value;
					OnPropertyChanged(nameof(DecInput));
				}
			}
		}

		private VarMap _resetInput;
		[JsonPropertyName("resetInput")]
		public VarMap ResetInput
		{
			get => _resetInput;
			set
			{
				if (_resetInput != value)
				{
					_resetInput = value;
					OnPropertyChanged(nameof(ResetInput));
				}
			}
		}

		private int _nMinCount;
		[JsonPropertyName("minCount")]
		public int MinCount
		{
			get => _nMinCount;
			set
			{
				if (_nMinCount != value)
				{
					_nMinCount = value;
					OnPropertyChanged(nameof(MinCount));
				}
			}
		}

		private int _nMaxCount;
		[JsonPropertyName("maxCount")]
		public int MaxCount
		{
			get => _nMaxCount;
			set
			{
				if (_nMaxCount != value)
				{
					_nMaxCount = value;
					OnPropertyChanged(nameof(MaxCount));
				}
			}
		}

		private InputEdge incEdge;
		[JsonPropertyName("incEdge")]
		public InputEdge IncEdge
		{
			get => incEdge;
			set
			{
				if (incEdge != value)
				{
					incEdge = value;
					OnPropertyChanged(nameof(IncEdge));
				}
			}
		}

		private InputEdge decEdge;
		[JsonPropertyName("decEdge")]
		public InputEdge DecEdge
		{
			get => decEdge;
			set
			{
				if (decEdge != value)
				{
					decEdge = value;
					OnPropertyChanged(nameof(DecEdge));
				}
			}
		}

		private InputEdge resetEdge;
		[JsonPropertyName("resetEdge")]
		public InputEdge ResetEdge
		{
			get => resetEdge;
			set
			{
				if (resetEdge != value)
				{
					resetEdge = value;
					OnPropertyChanged(nameof(ResetEdge));
				}
			}
		}

		private bool _wrapAround;
		[JsonPropertyName("wrapAround")]
		public bool WrapAround
		{
			get => _wrapAround;
			set
			{
				if (_wrapAround != value)
				{
					_wrapAround = value;
					OnPropertyChanged(nameof(WrapAround));
				}
			}
		}

		public static byte[] Request(int index)
		{
			byte[] data = new byte[8];
			data[0] = Convert.ToByte(MessagePrefix.Counter);
			data[1] = Convert.ToByte(index);
			return data;
		}

		public bool Receive(byte[] data)
		{
			if (data.Length != 8) return false;

			Enabled = Convert.ToBoolean(data[2] & 0x01);
			WrapAround = Convert.ToBoolean((data[2] & 0x02) >> 1);
			IncInput = (VarMap)data[3];
			DecInput = (VarMap)data[4];
			ResetInput = (VarMap)data[5];
			MinCount = (data[6] & 0x0F);
			MaxCount = ((data[6] & 0xF0) >> 4);
			IncEdge = (InputEdge)(data[7] & 0x03);
			DecEdge = (InputEdge)((data[7] & 0x0C) >> 2);
			ResetEdge = (InputEdge)((data[7] & 0x30) >> 4);

			return true;
		}

		public byte[] Write()
		{
			byte[] data = new byte[8];
			data[0] = Convert.ToByte(MessagePrefix.Counter);
			data[1] = Convert.ToByte(Number - 1);
			data[2] = Convert.ToByte(Convert.ToByte(Enabled) +
	 				  (Convert.ToByte(WrapAround) << 1));
			data[3] = Convert.ToByte(IncInput);
			data[4] = Convert.ToByte(DecInput);
			data[5] = Convert.ToByte(ResetInput);
			data[6] = Convert.ToByte((MinCount & 0x0F) +
					  ((MaxCount & 0x0F) << 4));
			data[7] = Convert.ToByte(((int)IncEdge & 0x03) +
					  (((int)DecEdge & 0x03) << 2) +
					  (((int)ResetEdge & 0x03) << 4));

			return data;
		}
	}
}

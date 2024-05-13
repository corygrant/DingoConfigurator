﻿using System.Text.Json.Serialization;

namespace CanDevices.DingoPdm
{
    public class Input : NotifyPropertyChangedBase
    {
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

        private bool _state;
        [JsonIgnore]
        public bool State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private bool _invertInput;
        [JsonPropertyName("invertInput")]
        public bool InvertInput
        {
            get => _invertInput;
            set
            {
                if (_invertInput != value)
                {
                    _invertInput = value;
                    OnPropertyChanged(nameof(InvertInput));
                }
            }
        }

        private InputMode _mode;
        [JsonPropertyName("mode")]
        public InputMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged(nameof(Mode));
                }
            }
        }

        private int _debounceTime;
        [JsonPropertyName("debounceTime")]
        public int DebounceTime
        {
            get => _debounceTime;
            set
            {
                if (_debounceTime != value)
                {
                    _debounceTime = value;
                    OnPropertyChanged(nameof(DebounceTime));
                }
            }
        }

        private InputPull _inputPull;
        [JsonPropertyName("pull")]
        public InputPull Pull
        {
            get => _inputPull;
            set
            {
                if (_inputPull != value)
                {
                    _inputPull = value;
                    OnPropertyChanged(nameof(InputPull));
                }
            }
        }
    }
}

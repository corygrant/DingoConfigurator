namespace CanDevices.DingoPdm
{
    public class DingoPdmInput : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmInput).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool State { get; set; }
        public bool InvertInput { get; set; }
        public InputMode Mode { get; set; }
        public int DebounceTime { get; set; }
    }
}

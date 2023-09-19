namespace CanDevices.DingoPdm
{
    public class DingoPdmFlasher : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmFlasher).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public string Name { get; set; }
        public int Number { get; set; }
        public int Value { get; set; }
        public bool Enabled { get; set; }
        public bool Single { get; set; }
        public VarMap Input { get; set; }
        public VarMap Output { get; set; }
        public int OnTime { get; set; }
        public int OffTime { get; set; }
    }
}

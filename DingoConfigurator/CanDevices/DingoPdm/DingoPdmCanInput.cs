namespace CanDevices.DingoPdm
{
    public class DingoPdmCanInput : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmCanInput).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public string Name { get; set; }
        public int Number { get; set; }
        public int Value { get; set; }
        public bool Enabled { get; set; }
        public int Id { get; set; }
        public int LowByte { get; set; }
        public int HighByte { get; set; }
        public Operator Operator { get; set; }
        public int OnVal { get; set; }
        public InputMode Mode { get; set; }
    }
}

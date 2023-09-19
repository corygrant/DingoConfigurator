namespace CanDevices.DingoPdm
{
    public class DingoPdmStarterDisable : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmStarterDisable).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public bool Enabled { get; set; }
        public VarMap Input { get; set; }
        public bool Output1 { get; set; }
        public bool Output2 { get; set; }
        public bool Output3 { get; set; }
        public bool Output4 { get; set; }
        public bool Output5 { get; set; }
        public bool Output6 { get; set; }
        public bool Output7 { get; set; }
        public bool Output8 { get; set; }
    }
}

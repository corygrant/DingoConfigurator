namespace CanDevices.DingoPdm
{
    public class DingoPdmVirtualInput : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmVirtualInput).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public string Name { get; set; }
        public int Number { get; set; }
        public int Value { get; set; }
        public bool Enabled { get; set; }
        public bool Not0 { get; set; }
        public VarMap Var0 { get; set; }
        public Conditional Cond0 { get; set; }
        public bool Not1 { get; set; }
        public VarMap Var1 { get; set; }
        public Conditional Cond1 { get; set; }
        public bool Not2 { get; set; }
        public VarMap Var2 { get; set; }
        public InputMode Mode { get; set; }
    }
}

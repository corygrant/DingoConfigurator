namespace CanDevices.DingoPdm
{
    public class DingoPdmOutput : NotifyPropertyChangedBase
    {

        public void UpdateView()
        {
            foreach(var prop in typeof(DingoPdmOutput).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public double Current { get; set; }
        public OutState State { get; set; }
        public double CurrentLimit { get; set; }
        public int ResetCount { get; set; }
        public int ResetCountLimit { get; set; }
        public ResetMode ResetMode { get; set; }
        public int ResetTime { get; set; }
        public double InrushCurrentLimit { get; set; }
        public int InrushTime { get; set; }
        public VarMap Input { get; set; }

        public DingoPdmOutput()
        {
            Enabled = false;
            Number = 0;
            State = OutState.Off;
            Current = 0;
            CurrentLimit = 0;
            ResetCount = 0;
        }
    }
}

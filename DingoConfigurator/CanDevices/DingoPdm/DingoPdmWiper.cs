namespace CanDevices.DingoPdm
{
    public class DingoPdmWiper : NotifyPropertyChangedBase
    {
        public void UpdateView()
        {
            foreach (var prop in typeof(DingoPdmWiper).GetProperties())
            {
                OnPropertyChanged(prop.Name);
            }
        }

        public DingoPdmWiper()
        {
            SpeedMap = new int[8];
            IntermitTime = new int[6];
        }

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool SlowState { get; set; }
        public bool FastState { get; set; }
        public WiperMode Mode { get; set; }
        public WiperState State { get; set; }
        public WiperSpeed Speed { get; set; }
        public VarMap SlowInput { get; set; }
        public VarMap FastInput { get; set; }
        public VarMap InterInput { get; set; }
        public VarMap OnInput { get; set; }
        public VarMap SpeedInput { get; set; }
        public VarMap ParkInput { get; set; }
        public bool ParkStopLevel { get; set; }
        public VarMap SwipeInput { get; set; }
        public VarMap WashInput { get; set; }
        public int WashWipeCycles { get; set; }
        public int[] SpeedMap { get; set; }
        public int[] IntermitTime { get; set; }
    }
}

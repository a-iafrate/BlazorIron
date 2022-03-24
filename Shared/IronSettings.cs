namespace BlazorIron.Shared
{
    public class IronSettings
    {
        public const string Position = "IronSettings";
        public int AngleOpenMotor1 { get; set; }
        public int AngleOpenMotor2 { get; set; }
        public int AngleCloseMotor1 { get; set; }
        public int AngleCloseMotor2 { get; set; }
        public int MinimumPulse { get; set; }
        public int MaximumPulse { get; set; }
        public int MotorSpeed { get; set; }
        public int EyeSwitchLed { get; set; }
        public int MotorSleep { get; set; }
    }
}
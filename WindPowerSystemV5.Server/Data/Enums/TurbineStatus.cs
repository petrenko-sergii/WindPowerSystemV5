namespace WindPowerSystemV5.Server.Data.Enums;

public enum TurbineStatus
{
    Unknown = 0,
    Installed,
    Run,
    InService,
    CutOut,
    ReadyNoWind,
    Failed,
    Disabled,
}

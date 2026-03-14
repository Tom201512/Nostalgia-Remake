namespace ReelSpinGame_Reel
{
    // リール関連のenum

    // リール識別用ID
    public enum ReelID
    {
        ReelLeft,
        ReelMiddle,
        ReelRight,
    };

    // リール位置識別
    public enum ReelPosID
    {
        Lower2nd = -1,
        Lower = 0,
        Center,
        Upper,
        Upper2nd,
    }

    // リールの状態
    public enum ReelStatus
    {
        Stopped,
        Spinning,
        ReceiveStop,
        Stopping,
    }

    // 図柄
    public enum ReelSymbols
    {
        RedSeven,
        BlueSeven,
        BAR,
        Cherry,
        Melon,
        Bell,
        Replay,
    }
}
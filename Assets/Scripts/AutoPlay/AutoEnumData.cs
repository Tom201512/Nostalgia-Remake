using System;

namespace ReelSpinGame_AutoPlay
{
    // オート関連のEnum
    public enum StopOrderID { First, Second, Third }                    // 停止順の識別化
    public enum StopOrderOptionName { LMR, LRM, MLR, MRL, RLM, RML }    // 停止順番のオプション名(左:L, 中:M, 右:R)
    public enum AutoSpeedName { Normal, Fast, Quick }                   // オート速度

    // 一定条件
    [Flags]
    public enum SpecificConditionFlag
    {
        None = 0,
        WinningPattern = 1 << 0,
        BIG = 1 << 1,
        REG = 1 << 2,
        EndBonus = 1 << 3,
    }

    // 回数条件
    public enum SpinTimeConditionName
    {
        None = 0,
        Spin1000G,
        Spin3000G,
        Spin5000G,
        Spin10000G,
    }
}
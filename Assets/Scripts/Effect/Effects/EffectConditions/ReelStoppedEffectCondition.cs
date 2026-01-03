using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // リール停止時エフェクトの条件
    public class ReelStoppedEffectCondition
    {
        public int StoppedReelCount { get; set; }           // リールの停止数
        public BigType RiichiBigType { get; set; }          // テンパイしたBIGの種類
        public BonusStatus BonusStatus { get; set; }        // ボーナス中の状態 

        public ReelStoppedEffectCondition()
        {
            StoppedReelCount = 0;
            RiichiBigType = BigType.None;
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}
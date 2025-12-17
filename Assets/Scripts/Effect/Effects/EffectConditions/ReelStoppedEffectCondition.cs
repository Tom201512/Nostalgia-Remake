using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data.Condition
{
    // リール停止時エフェクトの条件
    public class ReelStoppedEffectCondition
    {
        // var
        public int StoppedReelCount { get; set; }           // リールの停止数
        public BigColor RiichiBigColor { get; set; }        // テンパイしたBIG色
        public BonusStatus BonusStatus { get; set; }        // ボーナス中の状態 

        public ReelStoppedEffectCondition()
        {
            StoppedReelCount = 0;
            RiichiBigColor = BigColor.None;
            BonusStatus = BonusStatus.BonusNone;
        }
    }
}
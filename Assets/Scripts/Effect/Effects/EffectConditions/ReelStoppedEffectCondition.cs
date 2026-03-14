using ReelSpinGame_Bonus;

namespace ReelSpinGame_Effect.Data.Condition
{
    // リール停止時エフェクトの条件
    public class ReelStoppedEffectCondition
    {
        public int StoppedReelCount { get; set; }           // リールの停止数
        public BonusModel.BigType RiichiBigType { get; set; }          // テンパイしたBIGの種類
        public BonusModel.BonusStatus BonusStatus { get; set; }        // ボーナス中の状態 

        public ReelStoppedEffectCondition()
        {
            StoppedReelCount = 0;
            RiichiBigType = BonusModel.BigType.None;
            BonusStatus = BonusModel.BonusStatus.BonusNone;
        }

        // 条件作成
        public static ReelStoppedEffectCondition MakeReelStoppedEffectCondition(int stoppedReelCount, BonusModel.BigType riichiBigType, BonusModel.BonusStatus bonusStatus)
        {
            return new ReelStoppedEffectCondition
            {
                StoppedReelCount = stoppedReelCount,
                RiichiBigType = riichiBigType,
                BonusStatus = bonusStatus,
            };
        }
    }
}
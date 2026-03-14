using ReelSpinGame_Bonus;
using ReelSpinGame_Flag;

namespace ReelSpinGame_Reel
{
    // リールテーブルのメイン条件
    public class ReelMainCondition
    {
        public FlagModel.FlagID Flag { get; private set; }            // フラグ
        public BonusModel.BonusTypeID Bonus { get; private set; }      // 成立ボーナス
        public int Bet { get; private set; }                // ベット枚数
        public int Random { get; private set; }             // ランダム数値

        public ReelMainCondition()
        {
            Flag = FlagModel.FlagID.FlagNone;
            Bonus = BonusModel.BonusTypeID.BonusNone;
            Bet = 0;
            Random = 0;
        }

        public static ReelMainCondition MakeReelMainCondition(FlagModel.FlagID flag, BonusModel.BonusTypeID bonusTypeID, int betAmount, int randomValue)
        {
            return new ReelMainCondition
            {
                Flag = flag,
                Bonus = bonusTypeID,
                Bet = betAmount,
                Random = randomValue
            };
        }
    }
}
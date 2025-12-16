using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // オートAI条件のパラメータークラス
    public class AutoAIConditionClass
    {
        // var
        public FlagID Flag { get; set; }                    // フラグ 
        public ReelID FirstPush { get; set; }               // 第一停止リール
        public BonusTypeID HoldingBonus { get; set; }       // ストック中のボーナス
        public int BigChanceGames { get; set; }             // BIG時のゲーム数
        public int RemainingJacIn {  get; set; }            // 残りJAC-IN
        public int BetAmount {  get; set; }                 // ベット枚数

        public AutoAIConditionClass()
        {
            Flag = FlagID.FlagNone;
            FirstPush = ReelID.ReelLeft;
            HoldingBonus = BonusTypeID.BonusNone;
            BigChanceGames = 0;
            RemainingJacIn = 0;
            BetAmount = 0;
        }
    }
}
using ReelSpinGame_AutoPlay;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Save.Database.Option
{
    // オート設定
    public class AutoOptionData
    {
        public AutoSpeedName CurrentSpeed { get; set; }                    // オート速度
        public StopOrderOptionName CurrentStopOrder { get; set; }         // オート時の押し順
        public BigColor BigLineUpSymbol { get; set; }                     // 揃えるBIG図柄
        public bool HasTechnicalPlay { get; set; }                        // 技術介入をするか
        public byte EndConditionFlag { get; set; }                 // 終了条件のフラグ
        public SpinTimeConditionName SpinConditionID { get; set; }        // 回転条件 

        public AutoOptionData()
        {
            CurrentSpeed = AutoSpeedName.Normal;
            CurrentStopOrder = StopOrderOptionName.LMR;
            HasTechnicalPlay = true;
            BigLineUpSymbol = BigColor.None;
            EndConditionFlag = 0;
            SpinConditionID = SpinTimeConditionName.None;
        }

        // 数値初期化
        public void InitializeData()
        {
            CurrentSpeed = AutoSpeedName.Normal;
            CurrentStopOrder = StopOrderOptionName.LMR;
            HasTechnicalPlay = true;
            BigLineUpSymbol = BigColor.None;
            EndConditionFlag = 0;
            SpinConditionID = SpinTimeConditionName.None;
        }
    }
}
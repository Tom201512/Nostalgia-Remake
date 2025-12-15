using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Save.Database.Option
{
    // オート設定
    public class AutoOptionData
    {
        public AutoPlaySpeed AutoSpeedID { get; private set; }                    // オート速度
        public AutoStopOrderOptions AutoStopOrdersID { get; private set; }               // オート時の押し順
        public BigColor BigColorLineUpID { get; private set; }               // 揃えるBIG色
        public bool HasTechnicalPlay { get; private set; }              // 技術介入をするか
        public byte SpecificConditionBinary { get; private set; }       // 一定条件のバイナリ値
        public AutoSpinTimeConditionID SpinConditionID { get; private set; }                // 回転条件 

        public AutoOptionData()
        {
            AutoSpeedID = (int)AutoPlaySpeed.Normal;
            AutoStopOrdersID = (int)AutoStopOrderOptions.LMR;
            HasTechnicalPlay = true;
            BigColorLineUpID = (int)BigColor.None;
            SpecificConditionBinary = 0;
            SpinConditionID = (int)AutoSpinTimeConditionID.None;
        }

        // 数値設定
        public void SetAutoSpeed(AutoPlaySpeed autoSpeed) => AutoSpeedID = autoSpeed;
        public void SetAutoStopOrder(AutoStopOrderOptions stopOrder) => AutoStopOrdersID = stopOrder;
        public void SetBigColor(BigColor bigColor) => BigColorLineUpID = bigColor;
        public void SetTechnicalPlay(bool hasTechnical) => HasTechnicalPlay = hasTechnical;
        public void SetSpecificCondition(byte conditionBinary) => SpecificConditionBinary = conditionBinary;
        public void SetSpinCondition(AutoSpinTimeConditionID spinCondition) => SpinConditionID = spinCondition;

        // 数値初期化
        public void InitializeData()
        {
            AutoSpeedID = AutoPlaySpeed.Normal;
            AutoStopOrdersID = AutoStopOrderOptions.LMR;
            HasTechnicalPlay = true;
            BigColorLineUpID = BigColor.None;
            SpecificConditionBinary = 0;
            SpinConditionID = AutoSpinTimeConditionID.None;
        }
    }
}
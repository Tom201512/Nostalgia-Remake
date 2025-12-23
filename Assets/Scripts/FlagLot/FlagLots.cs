using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Lots.Flag
{
    // フラグ抽選
    public class FlagLots : MonoBehaviour
    {
        [SerializeField] FlagDatabase flagDatabase;         // フラグデータベース

        public bool UseForceFlag { get; private set; }      // 強制役を使用するか
        public FlagID ForceFlagID { get; private set; }     // 使用する強制役フラグ

        private FlagLotsModel flagLotsModel;                // フラグ抽選中のデータ
        private FlagCounter flagCounter;                    // フラグカウンタ

        private void Awake()
        {
            flagLotsModel = new FlagLotsModel();
            flagCounter = new FlagCounter();
        }

        // 各数値を得る
        public FlagID GetCurrentFlag() => flagLotsModel.CurrentFlag;             // 現在のフラグ
        public FlagLotTable GetCurrentTable() => flagLotsModel.CurrentTable;     // 現在のテーブル
        public int GetCounter() => flagCounter.Counter;

        public void SetCounterValue(int value) => flagCounter.SetCounter(value);        // 小役カウンタ数値セット
        public void ChangeTable(FlagLotTable mode) => flagLotsModel.CurrentTable = mode;     // テーブル変更

        // 強制フラグの設定
        public void SetForceFlag(FlagID forceFlagID)
        {
            ForceFlagID = forceFlagID;
            UseForceFlag = true;
        }

        // フラグ抽選をする
        public void StartFlagLots(int setting, int betAmount, BonusTypeID holdingBonusID)
        {
            if (UseForceFlag)
            {
                // 強制役を発動させる。その後は強制役を切る
                flagLotsModel.CurrentFlag = ForceFlagID;
                UseForceFlag = false;
            }
            else
            {
                flagLotsModel.GetFlagLots(flagCounter.Counter, setting, betAmount, flagDatabase);
            }

            // 何らかのボーナスが成立中にBIGまたはREGフラグが引かれた場合ははずれに置き換える
            if (holdingBonusID != BonusTypeID.BonusNone &&
               (flagLotsModel.CurrentFlag == FlagID.FlagBig || flagLotsModel.CurrentFlag == FlagID.FlagReg))
            {
                flagLotsModel.CurrentFlag = FlagID.FlagNone;
            }
        }

        // 小役カウンタ増加
        public void IncreaseCounter(int payoutAmount) => flagCounter.IncreaseCounter(payoutAmount);

        // 小役カウンタ減少
        public void DecreaseCounter(int setting, int lastBetAmount) => flagCounter.DecreaseCounter(setting, lastBetAmount);

        // カウンタリセット
        public void ResetCounter() => flagCounter.SetCounter(0);
    }
}

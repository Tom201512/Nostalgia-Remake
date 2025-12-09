using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // フラグ抽選
        // var
        // フラグ抽選中のデータ
        private FlagBehaviour data;
        // フラグデータベース
        [SerializeField] FlagDatabase flagDatabase;

        public bool UseForceFlag { get; private set; } // 強制役を使用するか
        public FlagID ForceFlagID { get; private set; } // 使用する強制役フラグ

        // func
        private void Awake()
        {
            data = new FlagBehaviour();
        }

        // 各数値を得る
        // 現在のフラグ
        public FlagID GetCurrentFlag() => data.CurrentFlag;
        // 現在のテーブル
        public FlagLotMode GetCurrentTable() => data.CurrentTable;
        // カウンタ
        public int GetCounter() => data.FlagCounter.Counter;

        // 数値変更
        public void SetCounterValue(int value) => data.FlagCounter.SetCounter(value);

        // テーブル変更
        public void ChangeTable(FlagLotMode mode) => data.CurrentTable = mode;

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
                data.CurrentFlag = ForceFlagID;
                UseForceFlag = false;
            }
            else
            {
                data.GetFlagLots(setting, betAmount, flagDatabase);
            }

            // 何らかのボーナスが成立中にBIGまたはREGフラグが引かれた場合ははずれに置き換える
            if (holdingBonusID != BonusTypeID.BonusNone &&
               (data.CurrentFlag == FlagID.FlagBig || data.CurrentFlag == FlagID.FlagReg))
            {
                data.CurrentFlag = FlagID.FlagNone;
            }
        }

        // 小役カウンタ増加
        public void IncreaseCounter(int payoutAmount) => data.FlagCounter.IncreaseCounter(payoutAmount);

        // 小役カウンタ減少
        public void DecreaseCounter(int setting, int lastBetAmount) =>data.FlagCounter.DecreaseCounter(setting, lastBetAmount);

        // カウンタリセット
        public void ResetCounter() => data.FlagCounter.ResetCounter();
    }
}

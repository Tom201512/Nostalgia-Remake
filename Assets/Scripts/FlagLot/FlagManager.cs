using ReelSpinGame_Bonus;
using ReelSpinGame_Datas;
using UnityEngine;

namespace ReelSpinGame_Flag.Flag
{
    // フラグ抽選
    public class FlagManager : MonoBehaviour
    {
        [SerializeField] FlagDatabase flagDatabase;     // フラグデータベース

        public bool UseForceFlag { get; private set; }                  // 強制役を使用するか
        public FlagModel.FlagID ForceFlagID { get; private set; }       // 使用する強制役フラグ

        // 現在の台設定
        public int CurrentSlotSetting { get => model.CurrentSlotSetting; }

        // ランダム設定を使用しているか                                                                               
        public bool IsUsingRandomSetting 
        { 
            get => model.IsRandomSlotSetting; 
            set => model.IsRandomSlotSetting = value;
        } 

        private FlagModel model;                            // フラグ抽選中のデータ
        private FlagCounter flagCounter;                    // フラグカウンタ

        private void Awake()
        {
            model = new FlagModel();
            flagCounter = new FlagCounter();
        }

        // 各数値を得る
        public FlagModel.FlagID GetCurrentFlag() => model.CurrentFlag;             // 現在のフラグ
        public FlagModel.FlagLotTable GetCurrentTable() => model.CurrentTable;     // 現在のテーブル
        public int GetCounter() => flagCounter.Counter;

        // フラグ設定を割り当てる
        public void SetSlotSetting(int setting)
        {
            // 0ならランダムを選ぶ
            if (setting == FlagModel.RandomSettingValue)
            {
                model.SetRandomSlotSetting();
            }
            else
            {
                model.CurrentSlotSetting = setting;
            }
        }

        // 小役カウンタ数値セット
        public void SetCounterValue(int value) => flagCounter.SetCounter(value);
        // テーブル変更
        public void ChangeTable(FlagModel.FlagLotTable mode) => model.CurrentTable = mode;

        // 強制フラグの設定
        public void SetForceFlag(FlagModel.FlagID forceFlagID)
        {
            ForceFlagID = forceFlagID;
            UseForceFlag = true;
        }

        // フラグ抽選をする
        public void StartFlagLots(int betAmount, BonusModel.BonusTypeID holdingBonusID)
        {
            if (UseForceFlag)
            {
                // 強制役を発動させる。その後は強制役を切る
                model.CurrentFlag = ForceFlagID;
                UseForceFlag = false;
            }
            else
            {
                model.GetFlagLots(flagCounter.Counter, betAmount, flagDatabase);
            }
        }

        // 小役カウンタ増加
        public void IncreaseCounter(int payoutAmount) => flagCounter.IncreaseCounter(payoutAmount);

        // 小役カウンタ減少
        public void DecreaseCounter(int lastBetAmount) => flagCounter.DecreaseCounter(model.CurrentSlotSetting, lastBetAmount);

        // カウンタリセット
        public void ResetCounter() => flagCounter.SetCounter(0);
    }
}
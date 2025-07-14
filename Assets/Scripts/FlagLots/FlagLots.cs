using ReelSpinGame_Datas;
using UnityEngine;
using static ReelSpinGame_Lots.FlagBehaviour;

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

        // デバッグ用(強制役)
        [SerializeField] private bool useInstant;
        [SerializeField] private bool useInfinityInstant;
        [SerializeField] private FlagId instantFlagID;

        // func
        void Awake()
        {
            data = new FlagBehaviour();
        }

        // 各数値を得る
        // 現在のフラグ
        public FlagId GetCurrentFlag() => data.CurrentFlag;
        // 現在のテーブル
        public FlagLotMode GetCurrentTable() => data.CurrentTable;
        // カウンタ
        public int GetCounter() => data.FlagCounter.Counter;

        // 数値変更
        // テーブル変更
        public void ChangeTable(FlagLotMode mode) => data.CurrentTable = mode;

        // フラグ抽選をする
        public void StartFlagLots(int setting, int betAmounts)
        {
            if (useInstant)
            {
                // 強制役を発動させる。その後は強制役を切る
                data.CurrentFlag = instantFlagID;

                // デバッグ用
                if(!useInfinityInstant)
                {
                    useInstant = false;
                }
            }
            else
            {
                data.GetFlagLots(setting, betAmounts, useInstant, instantFlagID, flagDatabase);
            }
        }

        // 小役カウンタ増加
        public void IncreaseCounter(int payoutAmounts) => data.FlagCounter.IncreaseCounter(payoutAmounts);

        // 小役カウンタ減少
        public void DecreaseCounter(int setting, int lastBetAmounts) =>data.FlagCounter.DecreaseCounter(setting, lastBetAmounts);

        // カウンタリセット
        public void ResetCounter() => data.FlagCounter.ResetCounter();
    }
}

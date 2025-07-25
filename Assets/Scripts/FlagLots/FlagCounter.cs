using UnityEngine;

namespace ReelSpinGame_Lots.FlagCounter
{
    public class FlagCounter
    {
        // 小役カウンタ

        // const
        //小役カウンタ減少値
        const int CounterDecrease = 256;
        //小役カウンタ増加値
        const int CounterIncreaseDefault = 100;
        // 設定5時
        const int CounterIncrease5 = 102;
        // 設定6時
        const int CounterIncrease6 = 104;

        // var
        // 小役カウンタ
        public int Counter { get; private set; }

        // コンストラクタ
        public FlagCounter()
        {
            Counter = 0;
        }

        // func
        // カウンタ値のセット
        public void SetCounter(int value) => Counter = value;

        // 小役カウンタの増加
        public void IncreaseCounter(int payoutAmounts)
        {
            Counter += CounterDecrease * payoutAmounts;
            //Debug.Log("Increased Counter Current:" + Counter);
        }

        // 小役カウンタの減少
        public void DecreaseCounter(int settingNum, int betAmounts)
        {
            // 設定5
            if(settingNum == 5)
            {
                Counter -= betAmounts * CounterIncrease5;
            }

            // 設定6
            else if(settingNum == 6)
            {
                Counter -= betAmounts * CounterIncrease6;
            }

            // それ以外
            else
            {
                Counter -= betAmounts * CounterIncreaseDefault;
            }

            //Debug.Log("Decreased Counter Current:" + Counter);
        }

        // 小役カウンタリセット
        public void ResetCounter()
        {
            //Debug.Log("Reset counter");
            Counter = 0;
        }
    }
}

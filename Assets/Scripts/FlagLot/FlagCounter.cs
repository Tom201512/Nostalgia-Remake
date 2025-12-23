namespace ReelSpinGame_Lots
{
    // 小役カウンタ
    public class FlagCounter
    {
        const int CounterDecrease = 256;            // 小役カウンタ減少値
        const int CounterIncreaseDefault = 100;     // 小役カウンタ増加値(設定1~4)
        const int CounterIncrease5 = 102;           // 小役カウンタ増加値(設定5)
        const int CounterIncrease6 = 104;           // 小役カウンタ増加値(設定6)

        public int Counter { get; private set; }    // 現在の小役カウンタ

        public FlagCounter()
        {
            Counter = 0;
        }

        // func
        // カウンタ値のセット
        public void SetCounter(int value) => Counter = value;

        // 小役カウンタの増加
        public void IncreaseCounter(int payoutAmount)
        {
            Counter += CounterDecrease * payoutAmount;
        }

        // 小役カウンタの減少
        public void DecreaseCounter(int settingNum, int betAmount)
        {
            // 設定5
            if(settingNum == 5)
            {
                Counter -= betAmount * CounterIncrease5;
            }

            // 設定6
            else if(settingNum == 6)
            {
                Counter -= betAmount * CounterIncrease6;
            }

            // それ以外
            else
            {
                Counter -= betAmount * CounterIncreaseDefault;
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

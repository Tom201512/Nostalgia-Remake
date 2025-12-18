namespace ReelSpinGame_Lamps
{
    // セグメントランプのユーティリティクラス
    public static class SegmentLampUtil
    {
        // 桁数を計算する
        public static int GetDigitCount(int value)
        {
            int sum = 0;
            int digitsCount = 0;

            // 0の場合は1桁を返す
            if (value == 0)
            {
                return 1;
            }
            // 指定桁数まで数字を出す
            while (value != 0)
            {
                sum = (value % 10);
                value = (value / 10);
                digitsCount += 1;
            }

            return digitsCount;
        }

        // 指定した桁にある数字を求める
        public static int GetDigitValue(int value, int digit)
        {
            int sum = 0;
            // 指定桁数まで数字を出す
            for (int i = 0; i < digit; i++)
            {
                sum = (value % 10);
                value = (value / 10);
            }

            return sum;
        }
    }
}
using ReelSpinGame_Reel.Spin;
using System;

namespace ReelSpinGame_Reel.Util
{
    // リール図柄の計算
    public static class ReelSymbolPosCalc
    {
        // リール位置をオーバーフローしない数値で返す
        public static int OffsetReelPos(int reelPos, int offset)
        {
            if (reelPos + offset < 0)
            {
                return ReelSpinModel.MaxReelArray + reelPos + (int)offset;
            }

            else if (reelPos + (int)offset > ReelSpinModel.MaxReelArray - 1)
            {
                return reelPos + (int)offset - ReelSpinModel.MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            return reelPos + (int)offset;
        }

        // リール配列の番号を図柄へ変更
        public static ReelSymbols ReturnSymbol(int symbolID) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), symbolID);
        // リール位置を配列要素に置き換える
        public static int GetReelArrayIndex(ReelPosID posID) => (int)posID + (int)ReelPosID.Lower2nd * -1;
    }
}
using ReelSpinGame_Reels.Spin;
using System;

namespace ReelSpinGame_Reels.Util
{
    // リール図柄の計算
    public static class ReelSymbolPosCalc
    {
        // リール位置をオーバーフローしない数値で返す
        public static int OffsetReelPos(int reelPos, int offset)
        {
            if (reelPos + offset < 0)
            {
                return ReelSpinModel.MaxReelArray + reelPos + offset;
            }

            else if (reelPos + offset > ReelSpinModel.MaxReelArray - 1)
            {
                return reelPos + offset - ReelSpinModel.MaxReelArray;
            }
            // オーバーフローがないならそのまま返す
            return reelPos + offset;
        }

        // リール配列の番号を図柄へ変更
        public static ReelSymbols ReturnSymbol(byte symbolID) => (ReelSymbols)Enum.ToObject(typeof(ReelSymbols), symbolID);
        // リール位置を配列要素に置き換える
        public static int GetReelArrayIndex(int posID) => posID + (int)ReelPosID.Lower2nd * -1;
    }
}
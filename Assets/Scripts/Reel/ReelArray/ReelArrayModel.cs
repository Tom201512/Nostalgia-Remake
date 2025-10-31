using System;
using UnityEngine;

namespace ReelSpinGame_Reels.Array
{
    public class ReelArrayModel
    {
        // リール配列のモデル

        // const
        // リール配列数
        public const int MaxReelArray = 21;
        // 図柄
        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }
        // リール位置識別用
        public enum ReelPosID { Lower2nd = -1, Lower, Center, Upper, Upper2nd }

        // var
        // リール配列
        public byte[] ReelArray { get; set; }

        // 現在の位置(下段基準)
        public int CurrentLower { get; set; }
        // 最後に止めた位置(下段基準)
        public int LastPushedLowerPos { get;  set; }
        // 将来的に止まる位置(下段基準)
        public int WillStopLowerPos { get; set; }
    }
}
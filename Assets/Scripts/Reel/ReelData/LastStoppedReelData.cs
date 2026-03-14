using System.Collections.Generic;

namespace ReelSpinGame_Reel
{
    // 最後に止めたリールのデータ
    public class LastStoppedReelData
    {
        public List<int> LastPos { get; private set; }                      // 最後に止めた位置
        public List<int> LastStoppedOrder { get; private set; }                // 最後に止めたときの押し順
        public List<int> LastReelDelay { get; private set; }                // 最後に止めたときのスベリコマ数
        public List<List<ReelSymbols>> LastSymbols { get; private set; }    // 最後に止まった出目

        public LastStoppedReelData()
        {
            LastPos = new List<int>();
            LastReelDelay = new List<int>();
            LastStoppedOrder = new List<int>();
            LastSymbols = new List<List<ReelSymbols>>();
        }
    }
}
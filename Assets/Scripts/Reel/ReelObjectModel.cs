using System;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_Reels
{
    public class ReelObjectModel
    {
        // リールオブジェクトモデル

        // const 
        // 最高ディレイ(スベリコマ数 4)
        public const int MaxDelay = 4;

        // var

        // リール識別ID
        public int ReelID { get; set; }
        // 現在の位置(下段基準)
        public int CurrentLower { get; set; }
        // 最後に止めた位置(下段基準)
        public int LastPushedLowerPos { get; set; }
        // 将来的に止まる位置(下段基準)
        public int WillStopLowerPos { get; set; }
        // 最後に止めた時のスベリコマ数
        public int LastStoppedDelay { get; set; }

        // コンストラクタ
        public ReelObjectModel(int reelID, int lowerPos) 
        {
            CurrentLower = 0;
            LastPushedLowerPos = 0;
            WillStopLowerPos = 0;
            LastStoppedDelay = 0;

            ReelID = reelID;
            // 位置設定
            // もし位置が0~20でなければ例外を出す
            if (lowerPos < 0 ||  lowerPos > MaxReelArray - 1)
            {
                throw new Exception("Invalid Position num. Must be within 0 ~ " + (MaxReelArray - 1));
            }
        }
    }
}

using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 1枚掛け時BB7狙いのAI
    public class AutoOneBetBB7BigAI : IAutoBehaviorBase
    {
        public AutoOneBetBB7BigAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // BARを狙う候補位置(左のみ。すべて上段を狙う)
            int[] bb7PosL = new int[] { 6, 14 };

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = bb7PosL[Random.Range(0, bb7PosL.Length)];
            // 中リールは10番、 右リールは19番を狙う
            stopPos[(int)ReelID.ReelMiddle] = 9;
            stopPos[(int)ReelID.ReelRight] = 18;

            return stopPos;
        }
    }
}
using ReelSpinGame_Reels;
using UnityEngine;

namespace ReelSpinGame_AutoPlay.AI
{
    // 通常時3枚掛けBB7時のAI
    public class AutoNormalBB7BigAI : IAutoBehaviorBase
    {
        public AutoNormalBB7BigAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // BARを狙う候補位置(左のみ。すべて上段を狙う)
            int[] bb7PosL = new int[] { 5, 16 };

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = bb7PosL[Random.Range(0, bb7PosL.Length)];
            // 中リールは10番、 右リールは19番を狙う
            stopPos[(int)ReelID.ReelMiddle] = 9;
            stopPos[(int)ReelID.ReelRight] = 18;

            return stopPos;
        }
    }
}
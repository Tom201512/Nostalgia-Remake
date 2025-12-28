using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // スイカ狙いのAI
    public class AutoMelonAI : IAutoBehaviorBase
    {
        public AutoMelonAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // スイカを狙う候補位置(全て上段を狙う)
            int[] melonPosL = new int[] { 1, 8, 11, 14, 20 };
            int[] melonPosM = new int[] { 2, 14 };
            int[] melonPosR = new int[] { 0, 4, 12, 16 };

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = melonPosL[Random.Range(0, melonPosL.Length)];
            stopPos[(int)ReelID.ReelMiddle] = melonPosM[Random.Range(0, melonPosM.Length)];
            stopPos[(int)ReelID.ReelRight] = melonPosR[Random.Range(0, melonPosR.Length)];

            return stopPos;
        }
    }
}
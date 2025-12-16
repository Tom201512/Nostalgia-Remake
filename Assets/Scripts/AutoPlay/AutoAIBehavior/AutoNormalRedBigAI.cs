using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // 通常時3枚掛け赤7BIG時のAI
    public class AutoNormalRedBigAI : IAutoBehaviorBase
    {
        public AutoNormalRedBigAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // 赤7を狙う候補位置(左のみ。全て上段を狙う)
            int[] redPosL = new int[] { 10, 18 };

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = redPosL[Random.Range(0, redPosL.Length)];
            // 中、右リールは19番を狙う
            stopPos[(int)ReelID.ReelMiddle] = 18;
            stopPos[(int)ReelID.ReelRight] = 18;

            return stopPos;
        }
    }
}

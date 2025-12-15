using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 通常時3枚掛け青7BIG時のAI
    public class AutoNormalBlueBigAI : IAutoBehaviorBase
    {
        public AutoNormalBlueBigAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // 青7を狙う候補位置(中のみ。全て上段を狙う)
            int[] bluePosM = new int[] { 6, 12 };

            // 候補に入れた数字からランダムで狙う
            // 左は4番
            stopPos[(int)ReelID.ReelLeft] = 3;
            // 中リールは候補から狙う
            stopPos[(int)ReelID.ReelMiddle] = bluePosM[Random.Range(0, bluePosM.Length)];
            // 右は10番を狙う
            stopPos[(int)ReelID.ReelRight] = 10;

            return stopPos;
        }
    }
}
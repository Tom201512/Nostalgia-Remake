using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 通常時3枚掛けボーナスゲーム図柄時のAI
    public class AutoNormalBonusGameAI : IAutoBehaviorBase
    {
        public AutoNormalBonusGameAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // BARを狙う候補位置(左、右のみ。全て上段を狙う)
            int[] barPosL = new int[] { 5, 16 };
            int[] barPosR = new int[] { 2, 14 };

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = barPosL[Random.Range(0, barPosL.Length)];
            // 中リールは10番を狙う
            stopPos[(int)ReelID.ReelMiddle] = 9;
            stopPos[(int)ReelID.ReelRight] = barPosR[Random.Range(0, barPosR.Length)];

            return stopPos;
        }
    }
}

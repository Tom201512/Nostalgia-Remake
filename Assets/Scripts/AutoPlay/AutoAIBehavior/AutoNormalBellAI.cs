using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 通常時のベル狙いAI
    public class AutoNormalBellAI : IAutoBehaviorBase
    {
        public AutoNormalBellAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // ベルを狙う候補(中リールのみ)
            int[] bellPosM = new int[] { 4, 11, 16, 20 };

            // 左は取りこぼさないのでランダム
            stopPos[(int)ReelID.ReelLeft] = Random.Range(0, MaxReelArray);
            // 中は取りこぼし位置があるので候補から狙う
            stopPos[(int)ReelID.ReelMiddle] = bellPosM[Random.Range(0, bellPosM.Length)];
            // 右は取りこぼさないのでランダム
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}

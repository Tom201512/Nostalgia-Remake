using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // 変則押し時のベル狙いAI
    public class AutoNonLeftFirstBellAI : IAutoBehaviorBase
    {
        public AutoNonLeftFirstBellAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // ベルを狙う候補(左、中)
            int[] bellPosL = new int[] { 0, 11 };
            int[] bellPosM = new int[] { 4, 11, 16, 20 };

            // 左は下段受けが狙えるように候補から狙う
            stopPos[(int)ReelID.ReelLeft] = bellPosL[Random.Range(0, bellPosL.Length)];
            // 中は取りこぼし位置があるので候補から狙う
            stopPos[(int)ReelID.ReelMiddle] = bellPosM[Random.Range(0, bellPosM.Length)];
            // 右は取りこぼさないのでランダム
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}

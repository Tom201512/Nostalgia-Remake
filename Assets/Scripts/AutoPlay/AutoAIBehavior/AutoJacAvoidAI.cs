using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 変則押し時でJACを外すAI
    public class AutoJacAvoidAI : IAutoBehaviorBase
    {
        public AutoJacAvoidAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };
            // ハズシ位置はすべてビタで止める
            int[] jacAvoidPosL = new int[] { 10, 16 };

            stopPos[(int)ReelID.ReelLeft] = jacAvoidPosL[Random.Range(0, jacAvoidPosL.Length)];
            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}

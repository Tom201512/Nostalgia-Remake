using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // “K“–‰Ÿ‚µ
    public class AutoRandomAI : IAutoBehaviorBase
    {
        public AutoRandomAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            stopPos[(int)ReelID.ReelLeft] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}

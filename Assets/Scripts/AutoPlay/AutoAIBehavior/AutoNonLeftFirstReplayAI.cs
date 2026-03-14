using ReelSpinGame_Reel;
using UnityEngine;
using static ReelSpinGame_Reel.Spin.ReelSpinModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // 変則押し時にリプレイを狙うAI
    public class AutoNonLeftFirstReplayAI : IAutoBehaviorBase
    {
        public AutoNonLeftFirstReplayAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };
            // 左のリプレイは1番または5番で固定
            int[] replayPosL = new int[] { 0, 4 };

            stopPos[(int)ReelID.ReelLeft] = replayPosL[Random.Range(0, replayPosL.Length)];
            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}

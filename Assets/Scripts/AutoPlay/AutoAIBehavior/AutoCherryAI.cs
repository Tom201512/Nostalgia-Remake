using ReelSpinGame_Reels;
using UnityEngine;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_AutoPlay.AI
{
    // É`ÉFÉäÅ[Çë_Ç§êßå‰
    public class AutoCherryAI : IAutoBehaviorBase
    {
        public AutoCherryAI() { }

        public int[] SendStopPosData()
        {
            int[] stopPos = new int[] { 0, 0, 0 };
            // ç∂ÇÕ5î‘, 17î‘Ç≈å≈íË
            int[] cherryPosL = new int[] { 4, 16 };

            stopPos[(int)ReelID.ReelLeft] = cherryPosL[Random.Range(0, cherryPosL.Length)];
            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
    }
}
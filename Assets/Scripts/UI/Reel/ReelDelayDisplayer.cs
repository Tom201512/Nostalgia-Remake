using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // リールスベリコマ数表示
    public class ReelDelayDisplayer : MonoBehaviour
    {
        [SerializeField] List<DelayDisplayer> delayDisplayers;      // スベリコマの表示機能

        // スベリコマ数リセット
        public void ResetAllDelay()
        {
            foreach (DelayDisplayer delay in delayDisplayers)
            {
                delay.ResetDelay();
            }
        }

        // 指定リールのスベリコマをセット
        public void SetDelay(ReelID reelID, int delay) => delayDisplayers[(int)reelID].SetDelay(delay);
    }
}

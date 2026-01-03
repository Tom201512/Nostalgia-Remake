using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // リール結果表示
    public class ReelDisplayUI : MonoBehaviour
    {
        [SerializeField] List<ReelDisplayer> ReelDisplayers;        // リールディスプレイ

        // リール図柄を表示させる
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            ReelDisplayers[(int)ReelID.ReelLeft].DisplayReelSymbols(leftLower);
            ReelDisplayers[(int)ReelID.ReelMiddle].DisplayReelSymbols(middleLower);
            ReelDisplayers[(int)ReelID.ReelRight].DisplayReelSymbols(rightLower);
        }

        // リール停止位置を表示させる
        public void DisplayPos(int leftLower, int middleLower, int rightLower)
        {
            ReelDisplayers[(int)ReelID.ReelLeft].DisplayPos(leftLower);
            ReelDisplayers[(int)ReelID.ReelMiddle].DisplayPos(middleLower);
            ReelDisplayers[(int)ReelID.ReelRight].DisplayPos(rightLower);
        }

        // 押し順の表示
        public void DisplayOrder(int leftOrder, int middleOrder, int rightOrder)
        {
            ReelDisplayers[(int)ReelID.ReelLeft].DisplayOrder(leftOrder);
            ReelDisplayers[(int)ReelID.ReelMiddle].DisplayOrder(middleOrder);
            ReelDisplayers[(int)ReelID.ReelRight].DisplayOrder(rightOrder);
        }

        // リールスベリコマ
        public void DisplayDelay(int leftDelay, int middleDelay, int rightDelay)
        {
            ReelDisplayers[(int)ReelID.ReelLeft].DisplayDelay(leftDelay);
            ReelDisplayers[(int)ReelID.ReelMiddle].DisplayDelay(middleDelay);
            ReelDisplayers[(int)ReelID.ReelRight].DisplayDelay(rightDelay);
        }
    }
}
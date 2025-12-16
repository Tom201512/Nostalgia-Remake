using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // リール結果表示

        // var
        // リールオブジェクト
        [SerializeField] List<ReelObjectPresenter> ReelObjects;
        // リールディスプレイ
        [SerializeField] List<ReelDisplayer> ReelDisplayers;

        // リール図柄を表示させる
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            ReelDisplayers[(int)ReelID.ReelLeft].DisplayReelSymbols(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            ReelDisplayers[(int)ReelID.ReelMiddle].DisplayReelSymbols(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            ReelDisplayers[(int)ReelID.ReelRight].DisplayReelSymbols(rightLower, ReelObjects[(int)ReelID.ReelRight]);
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
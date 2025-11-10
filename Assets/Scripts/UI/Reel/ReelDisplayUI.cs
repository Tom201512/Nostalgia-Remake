using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Array;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // リール結果表示

        // var
        // リールディスプレイ
        [SerializeField] List<ReelDisplayer> reelDisplayers;

        // リールデータを得る
        public List<ReelObjectPresenter> ReelObjects { get; private set; }

        // リールのセット
        public void SetReels(List<ReelObjectPresenter> reelObjects) => ReelObjects = reelObjects;

        // リール図柄を表示させる
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayReelSymbols(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayReelSymbols(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            reelDisplayers[(int)ReelID.ReelRight].DisplayReelSymbols(rightLower, ReelObjects[(int)ReelID.ReelRight]);
        }

        // リール停止位置を表示させる
        public void DisplayPos(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayPos(leftLower);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayPos(middleLower);
            reelDisplayers[(int)ReelID.ReelRight].DisplayPos(rightLower);
        }

        // 押し順の表示
        public void DisplayOrder(int leftOrder, int middleOrder, int rightOrder)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayOrder(leftOrder);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayOrder(middleOrder);
            reelDisplayers[(int)ReelID.ReelRight].DisplayOrder(rightOrder);
        }

        // リールスベリコマ
        public void DisplayDelay(int leftDelay, int middleDelay, int rightDelay)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayDelay(leftDelay);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayDelay(middleDelay);
            reelDisplayers[(int)ReelID.ReelRight].DisplayDelay(rightDelay);
        }
    }
}
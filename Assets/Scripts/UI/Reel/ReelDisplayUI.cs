using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // リール結果表示

        // var
        // リールディスプレイ
        [SerializeField] List<ReelDisplayer> reelDisplayers;

        // リールオブジェクト
        public List<ReelObject> ReelObjects { get; private set; }

        // リールのセット
        public void SetReels(List<ReelObject> reelObjects) => ReelObjects = reelObjects;

        // 指定位置のリール図柄を表示させる
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayReel(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayReel(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            reelDisplayers[(int)ReelID.ReelRight].DisplayReel(rightLower, ReelObjects[(int)ReelID.ReelRight]);
        }
    }
}
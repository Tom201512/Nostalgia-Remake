using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Spin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // ミニリール
    public class MiniReelDisplayer : MonoBehaviour
    {
        [SerializeField] List<ReelObjectPresenter> reelObjects;        // 監査対象のリールオブジェクト
        [SerializeField] List<ReelCursor> currentCursors;           // 現在位置のカーソル

        public bool IsActivating { get; set; }              // ミニリールが稼働中か

        void Update()
        {
            if(IsActivating)
            {
                for (int i = 0; i < reelObjects.Count; i++)
                {
                    float TweenValue = 0;
                    // 回転速度がマイナスであれば計算を逆にする
                    if (Math.Abs(reelObjects[i].RotateSpeed) == -1)
                    {
                        TweenValue = reelObjects[i].CurrentDegree / ReelSpinModel.ChangeAngle;
                    }
                    else if (Math.Abs(reelObjects[i].RotateSpeed) == 1)
                    {
                        TweenValue = (360 - reelObjects[i].CurrentDegree) / ReelSpinModel.ChangeAngle;
                    }

                    currentCursors[i].SetPosition(reelObjects[i].GetReelPos((int)ReelPosID.Lower), TweenValue);
                }
            }
        }

        // ミニリール位置設定
        public void SetMiniReelPos(List<int> reelPos)
        {
            for(int i = 0; i < currentCursors.Count; i++)
            {
                currentCursors[i].SetPosition(reelPos[i], 0);
            }
        }
    }
}

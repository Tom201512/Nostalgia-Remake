using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Spin;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // ミニリール
    public class MiniReelDisplayer : MonoBehaviour
    {
        [SerializeField] List<ReelObjectPresenter> reelObjects;         // 監査対象のリールオブジェクト
        [SerializeField] List<ReelCursor> stoppedCursors;               // 停止位置のカーソル
        [SerializeField] List<ReelCursor> delayCursors;                 // スベリコマ位置のカーソル
        [SerializeField] List<ReelCursor> markerCursors;                // マーカー用のカーソル

        public bool IsActivating { get; set; }              // ミニリールが稼働中か

        void Update()
        {
            if (IsActivating)
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

                    if (reelObjects[i].ReelStatus == ReelStatus.Spinning)
                    {
                        stoppedCursors[i].SetPosition(reelObjects[i].GetReelPos((int)ReelPosID.Lower), TweenValue);
                    }
                    delayCursors[i].SetPosition(reelObjects[i].GetReelPos((int)ReelPosID.Lower), TweenValue);
                }
            }
        }

        // ミニリール位置カーソル設定
        public void SetMiniReelPos(List<int> reelPos)
        {
            for (int i = 0; i < delayCursors.Count; i++)
            {
                stoppedCursors[i].SetPosition(reelPos[i], 0);
            }
        }

        // マーカー位置カーソル設定
        public void SetMarkerCursorPos(List<int> cursorPos)
        {
            for (int i = 0; i < markerCursors.Count; i++)
            {
                markerCursors[i].SetPosition(cursorPos[i], 0);
            }
        }

        // 停止カーソル設定
        public void SetStopCursor(ReelID reelID, int stoppedPos)
        {
            stoppedCursors[(int)reelID].SetPosition(stoppedPos, 0);
            delayCursors[(int)reelID].gameObject.SetActive(true);
        }

        // スベリコマ位置カーソルリセット
        public void ResetDelayCursor()
        {
            foreach (ReelCursor cursor in delayCursors)
            {
                cursor.SetPosition(-1, 0);
                cursor.gameObject.SetActive(false);
            }
        }
    }
}


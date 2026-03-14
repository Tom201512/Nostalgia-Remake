using ReelSpinGame_Bonus;
using ReelSpinGame_Reel.Symbol;
using ReelSpinGame_Reel.Table;
using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.Common;
using UnityEngine;

namespace ReelSpinGame_Reel
{
    // リールオブジェクトごとの処理
    public class ReelGroupAccessor : MonoBehaviour
    {
        [SerializeField] private List<ReelObject> reelObjects;       // リールオブジェクト

        public event Action<ReelID> AnyReelStopped;     // 何かしらのリールが止まったときのイベント

        private void Awake()
        {
            foreach(ReelObject reelObject in reelObjects)
            {
                reelObject.ReelStopped += OnReelStopped;
            }
        }

        // 各情報を得る
        public int[] GetArrayContents(ReelID reelID) => reelObjects[(int)reelID].ReelArray;                            // 図柄配列
        public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos(ReelPosID.Lower);            // 現在位置
        public int GetReelPushedPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos(ReelPosID.Center);            // 押した位置
        public int GetLastPushedLowerPos(ReelID reelID) => reelObjects[(int)reelID].LastPushedPos;                      // 止めた位置
        public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].WillStopLowerPos;                      // 停止予定位置
        public int GetLastDelay(ReelID reelID) => reelObjects[(int)reelID].LastDelay;                                   // スベリコマ数
        public ReelStatus GetReelStatus(ReelID reelID) => reelObjects[(int)reelID].ReelStatus;                          // リール状態
        public float GetReelSpeed(ReelID reelID) => reelObjects[(int)reelID].RotateSpeed;                               // リール速度
        public float GetReelDegree(ReelID reelID) => reelObjects[(int)reelID].CurrentDegree;                            // 回転角

        // 停止予定位置から指定位置の図柄を得る
        public ReelSymbols GetSymbolFromWillStop(ReelID reelID, ReelPosID reelPosID) => reelObjects[(int)reelID].GetSymbolFromWillStop(reelPosID);

        public ReelDelayTableFile GetReelTableData(ReelID reelID) => reelObjects[(int)reelID].ReelDelayTableFile;            // スベリコマテーブル

        // 各情報の設定
        public void SetCurrentLower(ReelID reelID, int currentLower) => reelObjects[(int)reelID].ChangeCurrentLower(currentLower);          // 現在の下段位置
        public void SetMarker(ReelID reelID, int markerPos) => reelObjects[(int)reelID].SetMarker(markerPos);                               // マーカー位置設定
        public void SetReelBlur(ReelID reelID, bool hasBlur) => reelObjects[(int)reelID].HasBlur = hasBlur;                                 // リールブラー設定

        // リールを回転させる
        public void StartReel(float maxSpeed, bool isFastAuto, BonusModel.BonusStatus currentBonusStatus)
        {
            foreach(ReelID reelID in Enum.GetValues(typeof(ReelID)))
            {
                reelObjects[(int)reelID].StartReel(maxSpeed, isFastAuto);
                // JACGAME中のリール計算処理をするか
                reelObjects[(int)reelID].HasJacModeLight = currentBonusStatus == BonusModel.BonusStatus.BonusJACGames;
                reelObjects[(int)reelID].HasBlur = true;
            }
        }

        // 指定リールの停止
        public void StopSelectedReel(ReelID reelID, int pushCount, int delay) => reelObjects[(int)reelID].StopReel(reelObjects[(int)reelID].GetReelPos(ReelPosID.Center), pushCount, delay);

        // 指定リールを指定位置で押し強制停止させる
        public void StopReelForce(ReelID reelID, int pushedPos, int pushCount, int delay) => reelObjects[(int)reelID].StopReelFast(pushedPos, pushCount, delay);

        // 最後に止めたリールの情報を記録
        public LastStoppedReelData RecordLastStoppedData()
        {
            LastStoppedReelData lastStoppedReelData = new LastStoppedReelData();
            // リール図柄を作成する
            foreach (ReelID reelID in Enum.GetValues (typeof(ReelID)))
            {
                lastStoppedReelData.LastPos.Add(reelObjects[(int)reelID].GetReelPos(ReelPosID.Lower));
                lastStoppedReelData.LastStoppedOrder.Add(reelObjects[(int)reelID].LastStoppedOrder);
                lastStoppedReelData.LastReelDelay.Add(reelObjects[(int)reelID].LastDelay);
                // データ作成
                lastStoppedReelData.LastSymbols.Add(new List<ReelSymbols>());

                // 各位置の図柄を得る(枠下から枠上)
                foreach (ReelPosID reelPos in SymbolManager.ReelPosOrder)
                {
                    lastStoppedReelData.LastSymbols[(int)reelID].Add(reelObjects[(int)reelID].GetReelSymbol(reelPos));
                }
            }

            return lastStoppedReelData;
        }

        // 指定リールの高速停止
        private void OnReelStopped(ReelID reelID)
        {
            AnyReelStopped?.Invoke(reelID);
        }
    }
}
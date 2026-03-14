using ReelSpinGame_Bonus;
using ReelSpinGame_Reel.Symbol;
using ReelSpinGame_Reel.Table;
using ReelSpinGame_UI.Reel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Reel
{
    // リールマネージャー
    public class ReelManager : MonoBehaviour
    {
        public const int ReelAmount = 3;    // リール数

        [SerializeField] ReelGroupAccessor reelGroup;           // リールオブジェクトのグループ
        [SerializeField] ReelSpinManager reelSpinManager;       // リール回転
        [SerializeField] ReelTableManager reelTableManager;     // スベリコマ管理
        [SerializeField] ReelSymbolCounter reelSymbolCounter;   // 図柄数カウント
        [SerializeField] MiniReelDisplayer miniReel;            // ミニリール機能
        [SerializeField] ReelDelayDisplayer delayDisplay;       // スベリコマ表示

        public bool IsReelWorking { get => reelSpinManager.IsReelWorking; }             // リールが動作中か
        public bool IsReelFinished { get => reelSpinManager.IsReelFinished; }           // リールが完全に停止したか
        public bool CanStopReels { get => reelSpinManager.CanStopReels; }               // 停止できる状態か
        public bool HasForceStop { get => reelSpinManager.HasForceStop; }               // 強制停止が有効か
        public bool IsFirstReelPushed { get => reelSpinManager.IsFirstReelPushed; }     // 第一停止をしたか
        public ReelID FirstPushReel { get => reelSpinManager.FirstPushReel; }           // 第一停止したリールのID
        public int StoppedReelCount { get => reelSpinManager.StoppedReelCount; }        // 停止したリール数
        public int RandomValue { get => reelSpinManager.RandomValue; }                  // 得たランダム数値

        // 最後に止めたリールデータ
        public LastStoppedReelData LastStoppedReelData { get => reelSpinManager.GetLastStoppedReelData(); }

        // リールが停止したときのイベント
        public event Action<ReelID> ReelStoppedEvent;

        private void Awake()
        {
            reelSpinManager.ReelStoppedEvent += OnReelStopped;
        }

        private void Start()
        {
            miniReel.gameObject.SetActive(false);
            delayDisplay.ResetAllDelay();
            delayDisplay.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            reelSpinManager.ReelStoppedEvent -= OnReelStopped;
        }

        // リールごとの情報を得る
        public int[] GetArrayContents(ReelID reelID) => reelGroup.GetArrayContents(reelID);            // 図柄配列
        public int GetCurrentReelPos(ReelID reelID) => reelGroup.GetCurrentReelPos(reelID);             // 現在位置(下段)
        public int GetReelPushedPos(ReelID reelID) => reelGroup.GetReelPushedPos(reelID);               // 押した位置(中段の位置)
        public int GetLastPushedLowerPos(ReelID reelID) => reelGroup.GetLastPushedLowerPos(reelID);     // 最後に押した下段位置
        public int GetWillStopReelPos(ReelID reelID) => reelGroup.GetWillStopReelPos(reelID);           // 停止予定位置
        public int GetLastDelay(ReelID reelID) => reelGroup.GetLastDelay(reelID);                       // スベリコマ(ディレイ)数
        public ReelStatus GetReelStatus(ReelID reelID) => reelGroup.GetReelStatus(reelID);              // 状態
        public float GetReelSpeed(ReelID reelID) => reelGroup.GetReelSpeed(reelID);                     // 速度
        public float GetReelDegree(ReelID reelID) => reelGroup.GetReelDegree(reelID);                   // 回転角

        // 揃っているBIG図柄の数を得る
        public BonusModel.BigType GetBigLinedUpCount(int betAmount, int checkAmount) => reelSymbolCounter.GetBigLinedUpCount(betAmount, checkAmount);

        // 強制フラグのランダム数値設定
        public void SetForceRandomValue(int value) => reelSpinManager.SetForceRandomValue(value);

        // リール位置設定
        public void SetReelPos(List<int> lastReelPos)
        {
            reelSpinManager.SetReelPos(lastReelPos);
            miniReel.SetMiniReelPos(lastReelPos);
        }

        // ミニリール表示設定
        public void SetMiniReelVisible(bool value)
        {
            miniReel.gameObject.SetActive(value);
            miniReel.IsActivating = value;
        }

        // スベリコマ表示設定
        public void SetReelDelayVisible(bool value) => delayDisplay.gameObject.SetActive(value);

        // マーカー位置設定
        public void SetReelMarkers(List<int> markerPos)
        {
            reelSpinManager.SetReelMarkers(markerPos);
            miniReel.SetMarkerCursorPos(markerPos);
        }

        // リール回転
        public void StartReels(BonusModel.BonusStatus currentBonusStatus, bool usingFastAuto)
        {
            reelSpinManager.StartReels(currentBonusStatus, usingFastAuto);
            miniReel.ResetDelayCursor();
            delayDisplay.ResetAllDelay();
        }

        // リールの停止(通常)
        public void StopSelectedReel(ReelID reelID, ReelMainCondition condition)
        {
            int delay = GetDelay(reelID, reelGroup.GetReelPushedPos(reelID), condition);
            reelSpinManager.StopSelectedReel(reelID, delay);
        }

        // 指定箇所でのリール高速停止
        public void StopSelectedReelFast(ReelID reelID, ReelMainCondition condition, int pushedPos)
        {
            int delay = GetDelay(reelID, pushedPos, condition);
            reelSpinManager.StopSelectedReelFast(reelID, delay, pushedPos);
        }

        // スベリコマを得る
        private int GetDelay(ReelID reelID, int pushedPos, ReelMainCondition condition) => reelTableManager.GetDelay(reelID, 
            reelSpinManager.StoppedReelCount, pushedPos, condition);

        // リールが停止したときのイベント
        private void OnReelStopped(ReelID reelID)
        {
            delayDisplay.SetDelay(reelID, reelGroup.GetLastDelay(reelID));
            miniReel.SetStopCursor(reelID, reelGroup.GetLastPushedLowerPos(reelID));
            ReelStoppedEvent?.Invoke(reelID);
        }
    }
}
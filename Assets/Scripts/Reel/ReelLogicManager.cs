using ReelSpinGame_Lots;
using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Reels
{
    // リールのロジック管理マネージャー
    public class ReelLogicManager : MonoBehaviour
    {
        public const int ReelAmount = 3;    // リール数

        ReelSpinManager spinManager;        // リール回転
        ReelSymbolCounter symbolCounter;    // 図柄カウント

        // プロパティ


        // リールが停止したときのイベント
        public delegate void SomeReelStopped();
        public event SomeReelStopped SomeReelStoppedEvent;

        void Awake()
        {
            spinManager = GetComponent<ReelSpinManager>();
            symbolCounter = GetComponent<ReelSymbolCounter>();

            // イベント登録
            spinManager.HasSomeReelStopped += OnSomeReelStopped;
        }

        void OnDestroy()
        {
            spinManager.HasSomeReelStopped -= OnSomeReelStopped;
        }

        // 数値を得る

        // マネージャー
        public bool GetIsReelWorking() => spinManager.IsReelWorking;           // リールが動作中か
        public bool GetIsReelFinished() => spinManager.IsReelFinished;         // リールの動作が終了したか
        public bool GetCanStopReels() => spinManager.CanStopReels;             // 停止できる状態か
        public bool HasForceStop() => spinManager.HasForceStop;                // オートストップ状態
        public bool GetIsFirstReelPushed() => spinManager.IsFirstReelPushed;   // 第一停止をしたか
        public ReelID GetFirstPushReel() => spinManager.FirstPushReel;         // 第一停止したリールのID
        public int GetStoppedCount() => spinManager.StoppedReelCount;          // 停止したリール数
        public int GetRandomValue() => spinManager.RandomValue;                // 得たランダム数値

        // 最後に止めたリールデータを返す
        public LastStoppedReelData GetLastStoppedReelData() => spinManager.GetLastStoppedReelData();

        // リールごとの情報を得る
        public byte[] GetArrayContents(ReelID reelID) => spinManager.GetArrayContents(reelID);            // 図柄配列
        public int GetCurrentReelPos(ReelID reelID) => spinManager.GetCurrentReelPos(reelID);             // 現在位置(下段)
        public int GetReelPushedPos(ReelID reelID) => spinManager.GetReelPushedPos(reelID);               // 押した位置(中段の位置)
        public int GetLastPushedLowerPos(ReelID reelID) => spinManager.GetLastPushedLowerPos(reelID);     // 最後に押した下段位置
        public int GetWillStopReelPos(ReelID reelID) => spinManager.GetWillStopReelPos(reelID);           // 停止予定位置
        public int GetLastDelay(ReelID reelID) => spinManager.GetLastDelay(reelID);                       // スベリコマ(ディレイ)数
        public ReelStatus GetReelStatus(ReelID reelID) => spinManager.GetReelStatus(reelID);              // 状態
        public float GetReelSpeed(ReelID reelID) => spinManager.GetReelSpeed(reelID);                     // 速度
        public float GetReelDegree(ReelID reelID) => spinManager.GetReelDegree(reelID);                   // 回転角
        public int GetUsedReelTID(ReelID reelID) => spinManager.GetUsedReelTID(reelID);                   // 使用したリールテーブルID
        public int GetUsedReelCID(ReelID reelID) => spinManager.GetUsedReelTID(reelID);                   // 使用した組み合わせID

        // 揃っているBIG図柄の数
        public BigType GetBigLinedUpCount(int betAmount, int checkAmount) => symbolCounter.GetBigLinedUpCount(betAmount, checkAmount);

        // 強制フラグのランダム数値設定
        public void SetForceRandomValue(int value) => spinManager.SetForceRandomValue(value);

        // リール位置設定
        public void SetReelPos(List<int> lastReelPos) => spinManager.SetReelPos(lastReelPos);

        // リールの回転
        public void StartReels(BonusStatus currentBonusStatus, bool usingFastAuto) => spinManager.StartReels(currentBonusStatus, usingFastAuto);

        // リールの停止(通常)
        public void StopSelectedReel(ReelID reelID, FlagID flag, BonusTypeID holdingBonus, int betAmount)
        {
            ReelMainCondition condition = new ReelMainCondition();
            condition.Flag = flag;
            condition.Bonus = holdingBonus;
            condition.Bet = betAmount;
            condition.Random = spinManager.RandomValue;

            spinManager.StopSelectedReel(reelID, condition);
        }

        // リール停止(高速)
        public void StopSelectedReelFast(ReelID reelID, FlagID flag, BonusTypeID holdingBonus, int betAmount, int pushedPos)
        {
            ReelMainCondition condition = new ReelMainCondition();
            condition.Flag = flag;
            condition.Bonus = holdingBonus;
            condition.Bet = betAmount;
            condition.Random = spinManager.RandomValue;

            spinManager.StopSelectedReelFast(reelID, condition, pushedPos);
        }

        // リールが停止したときのイベント
        void OnSomeReelStopped()
        {
            SomeReelStoppedEvent?.Invoke();
        }
    }
}

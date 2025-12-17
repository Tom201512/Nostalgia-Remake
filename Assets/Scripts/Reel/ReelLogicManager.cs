using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels
{
    // リールのロジック管理マネージャー
    public class ReelLogicManager : MonoBehaviour
    {
        // const
        public const int ReelAmount = 3;                // リール数

        // var
        ReelSpinManager spinManager;        // リール回転
        ReelSymbolCounter symbolCounter;        // 図柄カウント

        // リールが停止したときのイベント
        public delegate void SomeReelStopped();
        public event SomeReelStopped SomeReelStoppedEvent;

        // func
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

        // func
        // 数値を得る

        // マネージャー
        public bool GetIsReelWorking() => spinManager.GetIsReelWorking();           // リールが動作中か
        public bool GetIsReelFinished() => spinManager.GetIsReelFinished();         // リールの動作が終了したか
        public bool GetCanStopReels() => spinManager.GetCanStopReels();             // 停止できる状態か
        public bool HasForceStop() => spinManager.GetHasForceStop();                // オートストップ状態
        public bool GetIsFirstReelPushed() => spinManager.GetIsFirstReelPushed();   // 第一停止をしたか
        public ReelID GetFirstPushReel() => spinManager.GetFirstPushReel();         // 第一停止したリールのID
        public int GetStoppedCount() => spinManager.GetStoppedCount();          // 停止したリール数
        public int GetRandomValue() => spinManager.GetRandomValue();                // 得たランダム数値

        // リール
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
        public BigColor GetBigLinedUpCount(int betAmount, int checkAmount) => symbolCounter.GetBigLinedUpCount(betAmount, checkAmount);

        // 最後に止めたリールデータを返す
        public LastStoppedReelData GetLastStoppedReelData() => spinManager.GetLastStoppedReelData();

        // 強制フラグのランダム数値設定
        public void SetForceRandomValue(int value) => spinManager.SetForceRandomValue(value);

        // リール位置設定
        public void SetReelPos(List<int> lastReelPos) => spinManager.SetReelPos(lastReelPos);

        // リールの回転
        public void StartReels(BonusStatus currentBonusStatus, bool usingFastAuto) => spinManager.StartReels(currentBonusStatus, usingFastAuto);

        // リールの停止(通常)
        public void StopSelectedReel(ReelID reelID, int bet, FlagID flagID, BonusTypeID bonusID)
        {
            spinManager.StopSelectedReel(reelID, bet, flagID, bonusID);
        }

        // リール停止(高速)
        public void StopSelectedReelFast(ReelID reelID, int bet, FlagID flagID, BonusTypeID bonusID, int pushedPos)
        {
            spinManager.StopSelectedReelFast(reelID, bet, flagID, bonusID, pushedPos);
        }

        // リールが停止したときのイベント
        void OnSomeReelStopped()
        {
            SomeReelStoppedEvent?.Invoke();
        }
    }
}

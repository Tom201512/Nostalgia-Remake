using ReelSpinGame_Datas;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Payout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerModel;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // const
    // リール速度が一定になってから停止できるようになるまでの秒数
    public const float ReelWaitTime = 0.5f;
    // 操作が何もない状態で自動停止させる時間(秒)
    public const float ReelAutoStopTime = 60.0f;

    // var
    // リールマネージャーモデル
    private ReelManagerModel reelManagerModel;
    // 払い出し確認機能
    private PayoutChecker payoutChecker;
    // リールオブジェクトプレゼンター
    [SerializeField] private List<ReelObjectPresenter> reelObjects;
    // 強制ランダム数値
    [SerializeField] private bool instantRandomMode;
    // 強制ランダム数値を常に有効
    [SerializeField] private bool infinityRandomMode;
    // 強制時のランダム数値
    [Range(1,6),SerializeField] private int instantRandomValue;

    // リール演出用マネージャー
    [SerializeField] private ReelEffectManager reelEffectManager;
    // スロット情報UI
    [SerializeField] private SlotDataScreen slotDataScreen;

    // いずれかのリールが停止したかのイベント
    public delegate void ReelStoppedEvent();
    public event ReelStoppedEvent HasSomeReelStopped;

    // 初期化
    private void Awake()
    {
        reelManagerModel = new ReelManagerModel();
        payoutChecker = GetComponent<PayoutChecker>();

        // リール情報を渡す
        reelEffectManager.SetReels(reelObjects);
    }

    private void Start()
    {
        for (int i = 0; i < reelObjects.Count; i++)
        {
            reelObjects[i].InitializeReel(19);
            reelObjects[i].HasReelStopped += SendReelStoppedEvent;
        }
    }

    private void Update()
    {
        // リールが動いている時は
        if (reelManagerModel.IsReelWorking)
        {
            // 全リールが停止したかチェック
            if (CheckAllReelStopped())
            {
                reelManagerModel.CanStopReels = false;
                reelManagerModel.IsReelWorking = false;
                reelManagerModel.IsReelFinished = true;

                // リール停止位置記録
                reelManagerModel.GenerateLastStopped(reelObjects);

                // コルーチン停止
                StopAutoTime();
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // func

    // 数値を得る
    // マネージャー

    // リールが動作中か
    public bool GetIsReelWorking() => reelManagerModel.IsReelWorking;
    // リールの動作が終了したか
    public bool GetIsReelFinished() => reelManagerModel.IsReelFinished;
    // 停止できる状態か
    public bool GetCanStopReels() => reelManagerModel.CanStopReels;
    // オートストップ状態
    public bool HasForceStop() => reelManagerModel.HasForceStop;
    // 第一停止をしたか
    public bool GetIsFirstReelPushed() => reelManagerModel.IsFirstReelPushed;
    // 第一停止したリールのID
    public ReelID GetFirstPushReel() => reelManagerModel.FirstPushReel;
    // 停止したリール数
    public int GetStoppedCount() => reelManagerModel.StoppedReelCount;
    // 得たランダム数値
    public int GetRandomValue() => reelManagerModel.RandomValue;

    // リールオブジェクト
    // 指定リールの図柄配列を渡す
    public byte[] GetArrayContents(ReelID reelID) => reelObjects[(int)reelID].GetReelArrayData();
    // 指定したリールの現在位置(下段)を返す
    public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Lower);
    // 指定したリールの押した位置(中段の位置)を返す
    public int GetReelPushedPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Center);
    // 指定したリールの最後に押した下段位置を返す
    public int GetLastPushedLowerPos(ReelID reelID) => reelObjects[(int)reelID].GetLastPushedLowerPos();
    // 指定リールの停止予定位置を返す
    public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].GetWillStopLowerPos();
    // 指定したリールのディレイ数を返す
    public int GetLastDelay(ReelID reelID) => reelObjects[(int)reelID].GetLastDelay();
    // 指定リールの状態を確認する
    public ReelStatus GetReelStatus(ReelID reelID) => reelObjects[(int)reelID].GetCurrentReelStatus();
    // 指定リールの速度を得る
    public float GetReelSpeed(ReelID reelID) => reelObjects[(int)reelID].GetCurrentSpeed();
    // 指定リールの回転角を得る
    public float GetReelDegree(ReelID reelID) => reelObjects[(int)reelID].GetCurrentDegree();

    // リール出目データ
    // 最後に止めた出目
    public LastStoppedReelData GetLastStoppedReelData() => reelManagerModel.LastStoppedReelData;
    // 使用したリールテーブルID
    public int GetUsedReelTID(ReelID reelID) => reelManagerModel.ReelTableManager.UsedReelTableTID[(int)reelID];
    // 使用した組み合わせID
    public int GetUsedReelCID(ReelID reelID) => reelManagerModel.ReelTableManager.UsedReelTableCID[(int)reelID];
    // 払い出し結果データ表示
    public PayoutResultBuffer GetPayoutResultData() => payoutChecker.LastPayoutResult;
    // 払い出し判定モード表示
    public PayoutCheckMode GetPayoutCheckMode() => payoutChecker.CheckMode;
    // 払い出し判定モード変更
    public void ChangePayoutCheckMode(PayoutCheckMode mode) => payoutChecker.CheckMode = mode;

    // リール位置設定
    public void SetReelPos(List<int> lastReelPos)
    {
        int index = 0;
        foreach(int pos  in lastReelPos)
        {
            reelObjects[index].ChangeCurrentLower(pos);
            index += 1;
        }
    }

    // リール始動
    public void StartReels(BonusStatus currentBonusStatus, bool usingFastAuto)
    {
        // ランダム数値決定
        SetRandomValue();

        // リールが回っていなければ回転
        if (!reelManagerModel.IsReelWorking)
        {
            reelManagerModel.IsReelFinished = false;
            reelManagerModel.IsReelWorking = true;
            reelManagerModel.IsFirstReelPushed = false;
            reelManagerModel.StoppedReelCount = 0;

            for (int i = 0; i < reelObjects.Count; i++)
            {
                // 高速オートの有無で最高速度を変える
                float speed = usingFastAuto ? 1.5f : 1.0f;
                reelObjects[i].StartReel(speed, usingFastAuto);

                // JACGAME中のリール計算処理をするか
                reelObjects[i].HasJacModeLight = currentBonusStatus == BonusStatus.BonusJACGames;
            }

            // 高速オートの場合はすぐに停止可能にする
            if (usingFastAuto)
            {
                reelManagerModel.CanStopReels = true;
            }
            // リール停止可能タイマーをつける(高速オートでない場合)
            else
            {
                StartCoroutine(nameof(SetReelStopTimer));
            }
            // 1分間経過で自動停止にする
            StartCoroutine(nameof(AutoStopByTime));
        }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID, int bet, FlagId flagID, BonusTypeID bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(reelManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)reelID].GetCurrentReelStatus() == ReelStatus.Spinning)
            {
                // 停止位置を得る(中段の位置)
                int pushedPos = GetReelPushedPos(reelID);
                Debug.Log("Pushed at" + (pushedPos + 1));

                // 第一停止なら押したところの停止位置を記録
                if (!reelManagerModel.IsFirstReelPushed)
                {
                    reelManagerModel.IsFirstReelPushed = true;
                    reelManagerModel.FirstPushReel = reelID;
                    reelManagerModel.FirstPushPos = pushedPos;
                }

                // ディレイ(スベリコマ)を得る
                int delay = reelManagerModel.ReelTableManager.GetDelay(reelManagerModel.StoppedReelCount, pushedPos, reelObjects[(int)reelID].GetReelDatabase(),
                    flagID, reelID, bonusID, bet, reelManagerModel.RandomValue);

                // 停止したリール数を増やす
                reelManagerModel.StoppedReelCount += 1;

                // リールを止める
                reelObjects[(int)reelID].StopReel(pushedPos, reelManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 指定したリールの高速停止(位置指定が必要)
    public void StopSelectedReelFast(ReelID reelID, int bet, FlagId flagID, BonusTypeID bonusID, int pushedPos)
    {
        // 全リール速度が最高速度になっていれば
        if (reelManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)reelID].GetCurrentReelStatus() == ReelStatus.Spinning)
            {
                Debug.Log("PushedPos:" + pushedPos);
                // 第一停止なら押したところの停止位置を得る
                if (!reelManagerModel.IsFirstReelPushed)
                {
                    reelManagerModel.IsFirstReelPushed = true;
                    reelManagerModel.FirstPushReel = reelID;
                    reelManagerModel.FirstPushPos = pushedPos;
                }

                // ディレイ(スベリコマ)を得る
                int delay = reelManagerModel.ReelTableManager.GetDelay(reelManagerModel.StoppedReelCount, pushedPos, reelObjects[(int)reelID].GetReelDatabase(),
                    flagID, reelID, bonusID, bet, reelManagerModel.RandomValue);
                Debug.Log("Delay:" + delay);
                // リールを止める

                // 停止したリール数を増やす
                reelManagerModel.StoppedReelCount += 1;

                // すぐ指定位置まで停止させる。
                reelObjects[(int)reelID].StopReelFast(pushedPos, reelManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 何かしらのリールが止まった時のイベントを起こす
    private void SendReelStoppedEvent() => HasSomeReelStopped.Invoke();

    // 指定した数のBIG図柄が揃っているかを返す
    public BigColor GetBigLinedUpCount(int betAmount, int checkAmount)
    {
        // 赤7
        if (CountBonusSymbols(BigColor.Red, betAmount) == checkAmount)
        {
            return BigColor.Red;
        }

        // 青7
        if (CountBonusSymbols(BigColor.Blue, betAmount) == checkAmount)
        {
            return BigColor.Blue;
        }

        // BB7
        if (CountBonusSymbols(BigColor.Black, betAmount) == checkAmount)
        {
            return BigColor.Black;
        }

        return BigColor.None;
    }

    // ビッグチャンス図柄がいくつ揃っているか確認する
    private int CountBonusSymbols(BigColor bigColor, int betAmount)
    {
        // 最も多かった個数を記録
        int highestCount = 0;
        // 払い出しラインとベット枚数から確認
        foreach (PayoutLineData line in payoutChecker.GetPayoutLines())
        {
            // ベット条件を満たしているか確認
            if(betAmount >= line.BetCondition)
            {
                int currentCount = 0;
                // 停止状態になっている停止予定位置のリールからリーチ状態か確認
                for (int i = 0; i < reelObjects.Count; i++)
                {
                    if (reelObjects[i].GetCurrentReelStatus() != ReelStatus.Stopped)
                    {
                        continue;
                    }

                    // 赤7
                    if (bigColor == BigColor.Red && 
                        reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.RedSeven)
                    {
                        currentCount += 1;
                    }
                    // 青7
                    if (bigColor == BigColor.Blue &&
                        reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BlueSeven)
                    {
                        currentCount += 1;
                    }

                    // BB7
                    if(bigColor == BigColor.Black)
                    {
                        // 右リールは赤7があるか確認
                        if (i == (int)ReelID.ReelRight &&
                            reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.RedSeven)
                        {
                            currentCount += 1;
                        }
                        // 他のリールはBARがあるか確認
                        else if (i != (int)ReelID.ReelRight &&
                            reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BAR)
                        {
                            currentCount += 1;
                        }
                    }
                }
                // 最も多かったカウントを記録
                if(currentCount > highestCount)
                {
                    highestCount = currentCount;
                }
            }
        }
        return highestCount;
    }

    // 払い出しのチェック
    public void StartCheckPayout(int betAmount) => payoutChecker.CheckPayoutLines(betAmount, reelManagerModel.LastStoppedReelData);

    // ランダム数値の決定
    private void SetRandomValue()
    {
        // 強制的に変更する場合は指定した数値に
        if (instantRandomMode)
        {
            reelManagerModel.RandomValue = instantRandomValue;

            if(!infinityRandomMode)
            {
                instantRandomMode = false;
            }
        }
        else
        {
            reelManagerModel.RandomValue = Random.Range(1, MaxRandomLots + 1);
        }
    }

    // 全リールが停止したか確認
    private bool CheckAllReelStopped()
    {
        foreach (ReelObjectPresenter obj in reelObjects)
        {
            // 止まっていないリールがまだあれば falseを返す
            if (obj.GetCurrentReelStatus() != ReelStatus.Stopped)
            {
                return false;
            }
        }
        return true;
    }

    // コルーチン用
    // リール停止可能にする
    private IEnumerator SetReelStopTimer()
    {
        reelManagerModel.CanStopReels = false;
        yield return new WaitForSeconds(ReelWaitTime);
        reelManagerModel.CanStopReels = true;
        yield return null;
    }

    // リールを自動停止させる
    private IEnumerator AutoStopByTime()
    {
        yield return new WaitForSeconds(ReelAutoStopTime);
        reelManagerModel.HasForceStop = true;
        yield return null;
    }

    // リール自動停止のコルーチンストップ
    private void StopAutoTime()
    {
        StopCoroutine(nameof(AutoStopByTime));
        reelManagerModel.HasForceStop = false;
    }
}

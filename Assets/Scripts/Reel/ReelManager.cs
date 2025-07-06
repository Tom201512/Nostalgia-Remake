using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Payout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // const
    // リール速度が一定になってから停止できるようになるまでの秒数
    public const float ReelWaitTime = 0.5f;

    [SerializeField] bool CanStopImmediately;
    // var
    // リールマネージャーのデータ
    private ReelManagerBehaviour data;
    // リールのオブジェクト
    [SerializeField] private List<ReelObject> reelObjects;
    // 払い出し確認機能
    private PayoutChecker payoutChecker;
    // 強制ランダム数値
    [SerializeField] private bool instantRandomMode;
    // 強制時のランダム数値
    [Range(1,6),SerializeField] private int instantRandomValue;

    // いずれかのリールが停止したかのイベント
    public delegate void ReelStoppedEvent();
    public event ReelStoppedEvent HasSomeReelStopped;

    // 初期化
    void Awake()
    {
        for (int i = 0; i < reelObjects.Count; i++)
        {
            reelObjects[i].SetReelData(i, 19);
            reelObjects[i].HasReelStopped += SendReelStoppedEvent;
        }

        data = new ReelManagerBehaviour();
        payoutChecker = GetComponent<PayoutChecker>();
    }

    void Update()
    {
        // リールが動いている時は
        if (data.IsReelWorking)
        {
            // 全リールが等速かチェック(停止可能にする)
            if (!data.CanStopReels && CheckReelSpeedMaximum() && !IsInvoking())
            {
                StartCoroutine(nameof(SetReelStopTimer));
            }

            // 全リールが停止したかチェック
            if (CheckAllReelStopped())
            {
                data.CanStopReels = false;
                data.IsReelWorking = false;
                data.IsReelFinished = true;

                // リール停止位置記録
                data.GenerateLastStopped(reelObjects);
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
    public bool GetIsReelWorking() => data.IsReelWorking;
    // リールの動作が終了したか
    public bool GetIsReelFinished() => data.IsReelFinished;
    // 停止できる状態か
    public bool GetCanStopReels() => data.CanStopReels;
    // 第一停止をしたか
    public bool GetIsFirstReelPushed() => data.IsFirstReelPushed;
    // 第一停止したリールのID
    public ReelID GetFirstPushReel() => data.FirstPushReel;
    // 停止したリール数
    public int GetStoppedCount() => data.StoppedReelCount;
    // 得たランダム数値
    public int GetRandomValue() => data.RandomValue;

    // リールオブジェクト
    // 指定したリールの現在位置(下段)を返す
    public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos(ReelPosID.Lower);
    // 指定したリールの停止可能位置(中段)を返す
    public int GetReelCenterPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos(ReelPosID.Center);
    // 指定したリールの最後に押した位置を返す
    public int GetStoppedReelPos(ReelID reelID) => reelObjects[(int)reelID].GetLastPushedPos();
    // 指定リールの停止予定位置を返す
    public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].GetWillStopPos();
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
    public LastStoppedReelData GetLastStopped() => data.LastStopped;
    // 使用したリールテーブルID
    public int GetUsedReelTableID(ReelID reelID) => data.ReelTableManager.UsedReelTableID[(int)reelID];
    // 払い出し結果データ表示
    public PayoutResultBuffer GetPayoutResultData() => payoutChecker.LastPayoutResult;
    // 払い出し判定モード表示
    public PayoutCheckMode GetPayoutCheckMode() => payoutChecker.CheckMode;
    // 払い出し判定モード変更
    public void ChangePayoutCheckMode(PayoutCheckMode mode) => payoutChecker.CheckMode = mode;

    // リール始動
    public void StartReels()
    {
        // ランダム数値決定
        SetRandomValue();

        // リールが回っていなければ回転
        if (!data.IsReelWorking)
        {
            data.IsReelFinished = false;
            data.IsReelWorking = true;
            data.IsFirstReelPushed = false;

            for (int i = 0; i < reelObjects.Count; i++)
            {
                reelObjects[i].StartReel(1.0f);
            }

            data.StoppedReelCount = 0;
        }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID, int betAmounts, FlagId flagID, BonusType bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(data.CanStopReels)
        {
            // 止められる状態なら
            if (reelObjects[(int)reelID].GetCurrentReelStatus() == ReelStatus.WaitForStop)
            {
                // 中段の位置を得る
                int pushedPos = GetReelCenterPos(reelID);

                // 第一停止なら押したところの停止位置を得る
                if (!data.IsFirstReelPushed)
                {
                    data.IsFirstReelPushed = true;
                    data.FirstPushReel = reelID;
                    data.FirstPushPos = pushedPos;
                }

                // ここでディレイ(スベリコマ)を得て転送
                // 条件をチェック
                int tableIndex = data.ReelTableManager.FindTableToUse(reelID, reelObjects[(int)reelID].GetReelDatabase(),
                    flagID, data.FirstPushReel, betAmounts, (int)bonusID, data.RandomValue, data.FirstPushPos);
                // ディレイ(スベリコマ)を得る
                int delay = data.ReelTableManager.GetDelayFromTable(reelObjects[(int)reelID].GetReelDatabase(), pushedPos, tableIndex);
                // リールを止める

                // オートなどで高速停止が有効の場合はすぐ指定位置まで停止させる。
                if(CanStopImmediately)
                {
                    reelObjects[(int)reelID].StopReelFast(pushedPos, delay);
                }
                // 通常のリール停止(ディレイを伴って停止)
                else
                {
                    reelObjects[(int)reelID].StopReel(pushedPos, delay);
                }
                // 停止したリール数を増やす
                data.StoppedReelCount += 1;
            }
        }
    }

    // 何かしらのリールが止まった時のイベントを起こす
    private void SendReelStoppedEvent() => HasSomeReelStopped.Invoke();

    // 指定した数のBIG図柄が揃っているかを返す
    public BigColor GetBigLinedUpCounts(int betAmounts, int checkAmounts)
    {
        // 赤7
        if (CountBonusSymbols(BigColor.Red, betAmounts) == checkAmounts)
        {
            return BigColor.Red;
        }

        // 青7
        if (CountBonusSymbols(BigColor.Blue, betAmounts) == checkAmounts)
        {
            return BigColor.Blue;
        }

        // BB7
        if (CountBonusSymbols(BigColor.Black, betAmounts) == checkAmounts)
        {
            return BigColor.Black;
        }

        return BigColor.None;
    }

    // ビッグチャンス図柄がいくつ揃っているか確認する
    private int CountBonusSymbols(BigColor bigColor, int betAmounts)
    {
        // 最も多かった個数を記録
        int highestCount = 0;
        // 払い出しラインとベット枚数から確認
        foreach (PayoutLineData line in payoutChecker.GetPayoutLines())
        {
            // ベット条件を満たしているか確認
            if(betAmounts >= line.BetCondition)
            {
                int currentCount = 0;
                // 停止中状態になっている停止予定位置のリールからリーチ状態か確認
                for (int i = 0; i < reelObjects.Count; i++)
                {
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
    public void StartCheckPayouts(int betAmounts) => payoutChecker.CheckPayoutLines(betAmounts, data.LastStopped);

    // 全リール速度が最高速度かチェック
    private bool CheckReelSpeedMaximum()
    {
        foreach (ReelObject obj in reelObjects)
        {
            // 一部リールが最高速度でなければ falseを返す
            if (!obj.IsMaximumSpeed())
            {
                return false;
            }
        }
        return true;
    }

    // ランダム数値の決定
    private void SetRandomValue()
    {
        // 強制的に変更する場合は指定した数値に
        if (instantRandomMode)
        {
            data.RandomValue = instantRandomValue;
            instantRandomMode = false;
        }
        else
        {
            data.RandomValue = Random.Range(1, MaxRandomLots);
        }
    }

    // 全リールが停止したか確認
    private bool CheckAllReelStopped()
    {
        foreach (ReelObject obj in reelObjects)
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
        data.CanStopReels = false;
        yield return new WaitForSeconds(ReelWaitTime);
        data.CanStopReels = true;
    }
}

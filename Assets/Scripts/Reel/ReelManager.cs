using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Reels.Payout;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.Flash.FlashManager;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // const
    // リール速度が一定になってから停止できるようになるまでの秒数
    public const float ReelWaitTime = 0.5f;

    // var
    // リールマネージャーのデータ
    private ReelManagerBehaviour data;

    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;

    // フラッシュ機能
    private FlashManager flashManager;
    // 払い出し確認機能
    private PayoutChecker payoutChecker;

    // 強制ランダム数値
    [SerializeField] private bool instantRandomMode;
    // 強制時のランダム数値
    [Range(1,6),SerializeField] private int instantRandomValue;

    // 初期化
    void Awake()
    {
        for (int i = 0; i < reelObjects.Length; i++)
        {
            reelObjects[i].SetReelData(i, 19);
        }

        data = new ReelManagerBehaviour();
        flashManager = GetComponent<FlashManager>();
        payoutChecker = GetComponent<PayoutChecker>();
    }

    private void Start()
    {
        flashManager.SetReelObjects(reelObjects);
        //flashManager.StartReelFlash(FlashID.V_Flash);
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
    // 指定したリールの現在位置を返す
    public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos(ReelPosID.Lower);
    // 指定したリールを止めた位置を返す
    public int GetStoppedReelPos(ReelID reelID) => reelObjects[(int)reelID].GetLastPushedPos();
    // 指定リールの停止予定位置を返す
    public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].GetWillStopPos();
    // 指定したリールのディレイ数を返す
    public int GetLastDelay(ReelID reelID) => reelObjects[(int)reelID].GetLastDelay();
    // 指定リールの状態を確認する
    public ReelStatus GetReelStatus(ReelID reelID) => reelObjects[(int)reelID].GetCurrentReelStatus();

    // リール出目データ
    // 最後に止めた出目
    public LastStoppedReelData GetLastStopped() => data.LastStopped;
    // 使用したリールテーブルID
    public int GetUsedReelTableID(ReelID reelID) => data.ReelTableManager.UsedReelTableID[(int)reelID];

    //フラッシュ
    // フラッシュで待機中か
    public bool GetHasFlashWait() => flashManager.HasFlashWait;

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

            for (int i = 0; i < reelObjects.Length; i++)
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
                int pushedPos = reelObjects[(int)reelID].GetReelPos(ReelPosID.Center);

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
                reelObjects[(int)reelID].StopReel(pushedPos, delay);
                // 停止したリール数を増やす
                data.StoppedReelCount += 1;
            }
        }
    }

    // ビッグチャンス図柄がいくつ揃っているか確認する
    public int CountBonusSymbols(BigColor bigColor, int betAmounts)
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
                for (int i = 0; i < reelObjects.Length; i++)
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

    // フラッシュ関連
    // リール全点灯
    public void TurnOnAllReels(bool isJacGame)
    {
        // JAC GAME中は中段のみ光らせる
        if(isJacGame)
        {
            flashManager.EnableJacGameLight();
        }
        else
        {
            flashManager.TurnOnAllReels();
        }
    }

    // リールライト全消灯
    public void TurnOffAllReels() => flashManager.TurnOffAllReels();

    // リールフラッシュを開始させる
    public void StartReelFlash(FlashID flashID) => flashManager.StartReelFlash(flashID);

    // 払い出しのリールフラッシュを開始させる
    public void StartPayoutReelFlash(float waitSeconds) => flashManager.StartPayoutFlash(waitSeconds, payoutChecker.LastPayoutResult.PayoutLines);

    // フラッシュ停止
    public void StopReelFlash() => flashManager.StopFlash();

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

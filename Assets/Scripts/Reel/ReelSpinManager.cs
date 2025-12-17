using ReelSpinGame_Payout;
using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Symbol;
using ReelSpinGame_Reels.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_Reels.ReelSpinManagerModel;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

// リール回転マネージャー
public class ReelSpinManager : MonoBehaviour
{
    // const
    public const float ReelWaitTime = 0.5f;         // 停止可能になるまでの秒数
    public const float ReelAutoStopTime = 60.0f;    // 自動停止までの秒数

    // var
    [SerializeField] private List<ReelObjectPresenter> reelObjects;    // リールオブジェクトプレゼンター

    public bool HasForceRandomValue { get; private set; } // 強制ランダム数値を使用するか
    public int ForceRandomValue { get; private set; } // 強制フラグ発動時のランダム数値

    ReelTableManager tableManager;                          // 停止テーブル
    private ReelSpinManagerModel reelSpinManagerModel;      // リールマネージャーモデル
    ReelSymbolCounter reelSymbolCounter;            // リール図柄のカウント機能

    // いずれかのリールが停止したかのイベント
    public delegate void ReelStoppedEvent();
    public event ReelStoppedEvent HasSomeReelStopped;

    // 初期化
    private void Awake()
    {
        tableManager = new ReelTableManager();
        reelSpinManagerModel = new ReelSpinManagerModel();
        reelSymbolCounter = GetComponent<ReelSymbolCounter>();
    }

    void Start()
    {
        // リール位置初期化
        for (int i = 0; i < reelObjects.Count; i++)
        {
            reelObjects[i].InitializeReel(19);
            reelObjects[i].HasReelStopped += SendReelStoppedEvent;
        }
    }

    void Update()
    {
        // リールが動いている時は
        if (reelSpinManagerModel.IsReelWorking)
        {
            // 全リールが停止したかチェック
            if (CheckAllReelStopped())
            {
                reelSpinManagerModel.CanStopReels = false;
                reelSpinManagerModel.IsReelWorking = false;
                reelSpinManagerModel.IsReelFinished = true;

                // リール停止位置記録
                reelSpinManagerModel.GenerateLastStopped(reelObjects);

                // コルーチン停止
                StopAutoTime();
            }
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    // func

    // 数値を得る
    public bool GetIsReelWorking() => reelSpinManagerModel.IsReelWorking;           // リールが動作中か
    public bool GetIsReelFinished() => reelSpinManagerModel.IsReelFinished;         // リールの動作が終了したか
    public bool GetCanStopReels() => reelSpinManagerModel.CanStopReels;             // 停止できる状態か
    public bool GetHasForceStop() => reelSpinManagerModel.HasForceStop;                // オートストップ状態
    public bool GetIsFirstReelPushed() => reelSpinManagerModel.IsFirstReelPushed;   // 第一停止をしたか
    public ReelID GetFirstPushReel() => reelSpinManagerModel.FirstPushReel;         // 第一停止したリールのID
    public int GetStoppedCount() => reelSpinManagerModel.StoppedReelCount;          // 停止したリール数
    public int GetRandomValue() => reelSpinManagerModel.RandomValue;                // 得たランダム数値

    // リールオブジェクト
    public byte[] GetArrayContents(ReelID reelID) => reelObjects[(int)reelID].GetReelArrayData();                   // 図柄配列
    public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Lower);     // 指定リールの現在位置(下段)
    public int GetReelPushedPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Center);     // 指定したリールの押した位置(中段の位置)を返す
    public int GetLastPushedLowerPos(ReelID reelID) => reelObjects[(int)reelID].GetLastPushedLowerPos();            // 指定したリールの最後に押した下段位置を返す
    public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].GetWillStopLowerPos();                 // 指定リールの停止予定位置を返す
    public int GetLastDelay(ReelID reelID) => reelObjects[(int)reelID].GetLastDelay();                              // 指定したリールのディレイ数を返す
    public ReelStatus GetReelStatus(ReelID reelID) => reelObjects[(int)reelID].GetCurrentReelStatus();              // 指定リールの状態を確認する
    public float GetReelSpeed(ReelID reelID) => reelObjects[(int)reelID].GetCurrentSpeed();                         // 指定リールの速度を得る
    public float GetReelDegree(ReelID reelID) => reelObjects[(int)reelID].GetCurrentDegree();                       // 指定リールの回転角を得る

    // リール出目データ
    public int GetUsedReelTID(ReelID reelID) => tableManager.UsedReelTableTID[(int)reelID];    // 使用したリールテーブルID
    public int GetUsedReelCID(ReelID reelID) => tableManager.UsedReelTableCID[(int)reelID];    // 使用した組み合わせID
    public LastStoppedReelData GetLastStoppedReelData() => reelSpinManagerModel.LastStoppedReelData; // 停止結果

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

    // 強制ランダム数値の設定
    public void SetForceRandomValue(int value)
    {
        ForceRandomValue = value;
        HasForceRandomValue = true;
    }

    // リール始動
    public void StartReels(BonusStatus currentBonusStatus, bool usingFastAuto)
    {
        SetRandomValue();        // ランダム数値決定

        // リールが回っていなければ回転
        if (!reelSpinManagerModel.IsReelWorking)
        {
            reelSpinManagerModel.IsReelFinished = false;
            reelSpinManagerModel.IsReelWorking = true;
            reelSpinManagerModel.IsFirstReelPushed = false;
            reelSpinManagerModel.StoppedReelCount = 0;

            for (int i = 0; i < reelObjects.Count; i++)
            {
                // 高速オートの有無で最高速度を変える
                float speed = usingFastAuto ? 1.5f : 1.0f;
                reelObjects[i].StartReel(speed, usingFastAuto);

                // JACGAME中のリール計算処理をするか
                reelObjects[i].HasJacModeLight = currentBonusStatus == BonusStatus.BonusJACGames;
            }

            // 高速オート使用ならすぐに停止可能、それ以外は時間経過で停止できるようにする
            if (usingFastAuto)
            {
                reelSpinManagerModel.CanStopReels = true;
            }
            else
            {
                StartCoroutine(nameof(SetReelStopTimer));
            }

            StartCoroutine(nameof(AutoStopByTime)); // 1分間経過で自動停止にする
        }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID, int bet, FlagID flagID, BonusTypeID bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)reelID].GetCurrentReelStatus() == ReelStatus.Spinning)
            {
                // 停止位置を得る(中段の位置)
                int pushedPos = GetReelPushedPos(reelID);
                Debug.Log("Pushed at" + (pushedPos + 1));

                int delay = GetDelay(reelID, bet, flagID, bonusID, pushedPos);

                // リールを止める
                reelSpinManagerModel.StoppedReelCount += 1;                // 停止したリール数を増やす
                reelObjects[(int)reelID].StopReel(pushedPos, reelSpinManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 指定したリールの高速停止(位置指定が必要)
    public void StopSelectedReelFast(ReelID reelID, int bet, FlagID flagID, BonusTypeID bonusID, int pushedPos)
    {
        // 全リール速度が最高速度になっていれば
        if (reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)reelID].GetCurrentReelStatus() == ReelStatus.Spinning)
            {
                int delay = GetDelay(reelID, bet, flagID, bonusID, pushedPos);
                // リールを止める
                reelSpinManagerModel.StoppedReelCount += 1;
                reelObjects[(int)reelID].StopReelFast(pushedPos, reelSpinManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 揃っているBIG図柄の数を返す
    public BigColor GetBigLinedUpCount(int betAmount, int checkAmount) => reelSymbolCounter.GetBigLinedUpCount(betAmount, checkAmount);

    // ランダム数値の決定
    void SetRandomValue()
    {
        // 強制的に変更する場合は指定した数値に
        if (HasForceRandomValue)
        {
            reelSpinManagerModel.RandomValue = ForceRandomValue;
            HasForceRandomValue = false;
        }
        else
        {
            reelSpinManagerModel.RandomValue = Random.Range(1, MaxRandomLots + 1);
        }
    }

    // スベリコマ取得
    int GetDelay(ReelID reelID, int bet, FlagID flagID, BonusTypeID bonusID, int pushedPos)
    {
        Debug.Log("PushedPos:" + pushedPos);
        // 第一停止なら押したところの停止位置を得る
        if (!reelSpinManagerModel.IsFirstReelPushed)
        {
            reelSpinManagerModel.IsFirstReelPushed = true;
            reelSpinManagerModel.FirstPushReel = reelID;
            reelSpinManagerModel.FirstPushPos = pushedPos;
        }

        // ディレイ(スベリコマ)を得る
        int delay = tableManager.GetDelay(reelSpinManagerModel.StoppedReelCount, pushedPos, reelObjects[(int)reelID].GetReelDatabase(),
            flagID, reelID, bonusID, bet, reelSpinManagerModel.RandomValue);
        Debug.Log("Delay:" + delay);

        return delay;
    }

    // 全リールが停止したか確認
    bool CheckAllReelStopped()
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

    // 何かしらのリールが止まった時のイベントを起こす
    void SendReelStoppedEvent() => HasSomeReelStopped?.Invoke();

    // コルーチン用
    // リール停止可能にする
    IEnumerator SetReelStopTimer()
    {
        reelSpinManagerModel.CanStopReels = false;
        yield return new WaitForSeconds(ReelWaitTime);
        reelSpinManagerModel.CanStopReels = true;
        yield return null;
    }

    // リールを自動停止させる
    IEnumerator AutoStopByTime()
    {
        yield return new WaitForSeconds(ReelAutoStopTime);
        reelSpinManagerModel.HasForceStop = true;
        yield return null;
    }

    // リール自動停止のコルーチンストップ
    void StopAutoTime()
    {
        StopCoroutine(nameof(AutoStopByTime));
        reelSpinManagerModel.HasForceStop = false;
    }
}

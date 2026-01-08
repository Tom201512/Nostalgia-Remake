using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Symbol;
using ReelSpinGame_Reels.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelSpinManagerModel;

// リール回転マネージャー
public class ReelSpinManager : MonoBehaviour
{
    public const float ReelWaitTime = 0.5f;         // 停止可能になるまでの秒数
    public const float ReelAutoStopTime = 60.0f;    // 自動停止までの秒数

    [SerializeField] private List<ReelObjectPresenter> reelObjects;    // リールオブジェクトプレゼンター

    // プロパティ
    public bool IsReelWorking { get => reelSpinManagerModel.IsReelWorking; }            // リールが動作中か
    public bool IsReelFinished { get => reelSpinManagerModel.IsReelFinished; }          // リールの動作が終了したか
    public bool CanStopReels { get => reelSpinManagerModel.CanStopReels; }              // 停止できる状態か
    public bool HasForceStop { get => reelSpinManagerModel.HasForceStop; }              // オートストップ状態
    public bool IsFirstReelPushed { get => reelSpinManagerModel.IsFirstReelPushed; }    // 第一停止をしたか
    public ReelID FirstPushReel { get => reelSpinManagerModel.FirstPushReel; }          // 第一停止したリールのID
    public int StoppedReelCount { get => reelSpinManagerModel.StoppedReelCount; }       // 停止したリール数
    public int RandomValue { get => reelSpinManagerModel.RandomValue; }                 // 得たランダム数値

    public bool HasForceRandomValue { get; private set; } // 強制ランダム数値を使用するか
    public int ForceRandomValue { get; private set; } // 強制フラグ発動時のランダム数値

    ReelTableManager tableManager;                          // 停止テーブル
    private ReelSpinManagerModel reelSpinManagerModel;      // リールマネージャーモデル
    ReelSymbolCounter reelSymbolCounter;            // リール図柄のカウント機能

    // いずれかのリールが停止したかのイベント
    public delegate void ReelStoppedEvent(ReelID reelID);
    public event ReelStoppedEvent HasSomeReelStopped;

    void Awake()
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

    // リールオブジェクト
    public byte[] GetArrayContents(ReelID reelID) => reelObjects[(int)reelID].ReelArray;                            // 図柄配列
    public int GetCurrentReelPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Lower);     // 現在位置
    public int GetReelPushedPos(ReelID reelID) => reelObjects[(int)reelID].GetReelPos((sbyte)ReelPosID.Center);     // 押した位置
    public int GetLastPushedLowerPos(ReelID reelID) => reelObjects[(int)reelID].LastPushedPos;                      // 止めた位置
    public int GetWillStopReelPos(ReelID reelID) => reelObjects[(int)reelID].WillStopLowerPos;                      // 停止予定位置
    public int GetLastDelay(ReelID reelID) => reelObjects[(int)reelID].LastDelay;                                   // スベリコマ数
    public ReelStatus GetReelStatus(ReelID reelID) => reelObjects[(int)reelID].ReelStatus;                          // リール状態
    public float GetReelSpeed(ReelID reelID) => reelObjects[(int)reelID].RotateSpeed;                               // リール速度
    public float GetReelDegree(ReelID reelID) => reelObjects[(int)reelID].CurrentDegree;                            // 回転角

    // リール出目データ
    public int GetUsedReelTID(ReelID reelID) => tableManager.UsedReelTableTID[(int)reelID];    // 使用したリールテーブルID
    public int GetUsedReelCID(ReelID reelID) => tableManager.UsedReelTableCID[(int)reelID];    // 使用した組み合わせID
    public LastStoppedReelData GetLastStoppedReelData() => reelSpinManagerModel.LastStoppedReelData; // 停止結果

    // リール位置設定
    public void SetReelPos(List<int> lastReelPos)
    {
        for(int i = 0; i < reelObjects.Count; i++)
        {
             reelObjects[i].ChangeCurrentLower(lastReelPos[i]);
        }
    }

    // リールマーカー設定
    public void SetReelMarkers(List<int> markerPos)
    {
        for (int i = 0; i < reelObjects.Count; i++)
        {
            reelObjects[i].SetMarker(markerPos[i]);
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
    public void StopSelectedReel(ReelID pushedReelID, ReelMainCondition mainCondition)
    {
        // 全リール速度が最高速度になっていれば
        if (reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)pushedReelID].ReelStatus == ReelStatus.Spinning)
            {
                // 停止位置を得る(中段の位置)
                int pushedPos = GetReelPushedPos(pushedReelID);
                int delay = GetDelay(pushedReelID, mainCondition, pushedPos);

                // リールを止める
                reelSpinManagerModel.StoppedReelCount += 1;
                reelObjects[(int)pushedReelID].StopReel(pushedPos, reelSpinManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 指定したリールの高速停止(位置指定が必要)
    public void StopSelectedReelFast(ReelID pushedReelID, ReelMainCondition mainCondition, int pushedPos)
    {
        // 全リール速度が最高速度になっていれば
        if (reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelObjects[(int)pushedReelID].ReelStatus == ReelStatus.Spinning)
            {
                int delay = GetDelay(pushedReelID, mainCondition, pushedPos);
                // リールを止める
                reelSpinManagerModel.StoppedReelCount += 1;
                reelObjects[(int)pushedReelID].StopReelFast(pushedPos, reelSpinManagerModel.StoppedReelCount, delay);
            }
        }
    }

    // 揃っているBIG図柄の数を返す
    public BigType GetBigLinedUpCount(int betAmount, int checkAmount) => reelSymbolCounter.GetBigLinedUpCount(betAmount, checkAmount);

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
    int GetDelay(ReelID pushReelID, ReelMainCondition mainCondition, int pushedPos)
    {
        // 第一停止なら押したところの停止位置を得る
        if (!reelSpinManagerModel.IsFirstReelPushed)
        {
            reelSpinManagerModel.IsFirstReelPushed = true;
            reelSpinManagerModel.FirstPushReel = pushReelID;
            reelSpinManagerModel.FirstPushPos = pushedPos;
        }

        // ディレイ(スベリコマ)を得る
        int delay = tableManager.GetDelay(pushReelID, reelSpinManagerModel.StoppedReelCount, pushedPos,
            reelObjects[(int)pushReelID].GetReelDatabase(), mainCondition);

        return delay;
    }

    // 全リールが停止したか確認
    bool CheckAllReelStopped()
    {
        foreach (ReelObjectPresenter obj in reelObjects)
        {
            // 止まっていないリールがまだあれば falseを返す
            if (obj.ReelStatus != ReelStatus.Stopped)
            {
                return false;
            }
        }
        return true;
    }

    // 何かしらのリールが止まった時のイベントを起こす
    void SendReelStoppedEvent(ReelID reelID) => HasSomeReelStopped?.Invoke(reelID);

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

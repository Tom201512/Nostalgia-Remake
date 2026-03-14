using ReelSpinGame_Bonus;
using ReelSpinGame_Reel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// リール回転マネージャー
public class ReelSpinManager : MonoBehaviour
{
    public const float ReelWaitTime = 0.6f;         // 停止可能になるまでの秒数
    public const float ReelAutoStopTime = 60.0f;    // 自動停止までの秒数

    const float NormalReelSpeed = 1.0f;             // 通常時の回転速度
    const float FastAutoReelSpeed = 1.5f;           // 高速オート時の回転速度

    [SerializeField] private ReelGroupAccessor reelGroup;              // リールオブジェクトグループ

    public bool IsReelWorking { get => reelSpinManagerModel.IsReelWorking; }            // リールが動作中か
    public bool IsReelFinished { get => reelSpinManagerModel.IsReelFinished; }          // リールの動作が終了したか
    public bool CanStopReels { get => reelSpinManagerModel.CanStopReels; }              // 停止できる状態か
    public bool HasForceStop { get => reelSpinManagerModel.HasForceStop; }              // オートストップ状態
    public bool IsFirstReelPushed { get => reelSpinManagerModel.IsFirstReelPushed; }    // 第一停止をしたか
    public ReelID FirstPushReel { get => reelSpinManagerModel.FirstPushReel; }          // 第一停止したリールのID
    public int StoppedReelCount { get => reelSpinManagerModel.PushedCount; }       // 停止したリール数
    public int RandomValue { get => reelSpinManagerModel.RandomValue; }                 // 得たランダム数値

    public bool HasAllReelStopped { get => reelSpinManagerModel.PushedCount == ReelManager.ReelAmount; }

    public bool HasForceRandomValue { get; private set; }   // 強制ランダム数値を使用するか
    public int ForceRandomValue { get; private set; }       // 強制フラグ発動時のランダム数値

    private ReelSpinManagerModel reelSpinManagerModel;      // リールマネージャーモデル

    public event Action<ReelID> ReelStoppedEvent;    // いずれかのリールが停止したかのイベント
    public event Action ReelPositionChanged;         // リールのコマ位置が変更された時のイベント

    void Awake()
    {
        reelSpinManagerModel = new ReelSpinManagerModel();
    }

    void Start()
    {
        reelGroup.AnyReelStopped += OnReelStopped;
        // ブラーを切る
        foreach(ReelID reelID in Enum.GetValues(typeof(ReelID)))
        {
            reelGroup.SetReelBlur(reelID, false);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public LastStoppedReelData GetLastStoppedReelData() => reelSpinManagerModel.LastStoppedReelData; // 停止結果

    // リール位置設定
    public void SetReelPos(List<int> lastReelPos)
    {
        foreach(ReelID reelID in Enum.GetValues (typeof(ReelID)))
        {
            reelGroup.SetCurrentLower(reelID, lastReelPos[(int)reelID]);
        }
    }

    // リールマーカー設定
    public void SetReelMarkers(List<int> markerPos)
    {
        foreach (ReelID reelID in Enum.GetValues(typeof(ReelID)))
        {
            reelGroup.SetMarker(reelID, markerPos[(int)reelID]);
        }
    }

    // 強制ランダム数値の設定
    public void SetForceRandomValue(int value)
    {
        ForceRandomValue = value;
        HasForceRandomValue = true;
    }

    // リール始動
    public void StartReels(BonusModel.BonusStatus currentBonusStatus, bool isFastAuto)
    {
        SetRandomValue();        // ランダム数値決定

        // リールが回っていなければ回転
        if (!reelSpinManagerModel.IsReelWorking)
        {
            reelSpinManagerModel.IsReelFinished = false;
            reelSpinManagerModel.IsReelWorking = true;
            reelSpinManagerModel.IsFirstReelPushed = false;
            reelSpinManagerModel.PushedCount = 0;

            // 高速オートの有無で最高速度を変える
            float speed = isFastAuto ? FastAutoReelSpeed :NormalReelSpeed;
            reelGroup.StartReel(speed, isFastAuto, currentBonusStatus);

            // 高速オート使用ならすぐに停止可能、それ以外は時間経過で停止できるようにする
            if (isFastAuto)
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

    // 各リールを指定スベリ数で止める
    public void StopSelectedReel(ReelID reelID, int delay)
    {
        // 全リール速度が最高速度になっていれば
        if (reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelGroup.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                // リールを止める
                reelSpinManagerModel.PushedCount += 1;
                reelGroup.StopSelectedReel(reelID, reelSpinManagerModel.PushedCount, delay);
            }
        }
    }

    // 指定したリールの高速停止(位置指定が必要)
    public void StopSelectedReelFast(ReelID reelID, int delay, int pushedPos)
    {
        // 全リール速度が最高速度になっていれば
        if (reelSpinManagerModel.CanStopReels)
        {
            // 回転中なら停止
            if (reelGroup.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                // リールを止める
                reelSpinManagerModel.PushedCount += 1;
                reelGroup.StopReelForce(reelID, pushedPos, reelSpinManagerModel.PushedCount, delay);
            }
        }
    }

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
            reelSpinManagerModel.RandomValue = UnityEngine.Random.Range(1, ReelSpinManagerModel.MaxRandomLots + 1);
        }
    }

    // いずれかのリールが止まったときの処理
    void OnReelStopped(ReelID reelID)
    {
        ReelStoppedEvent?.Invoke(reelID);
        // ブラー解除
        reelGroup.SetReelBlur(reelID, false);
        // リールがすべて停止していたら
        if (reelSpinManagerModel.PushedCount == ReelManager.ReelAmount)
        {
            reelSpinManagerModel.CanStopReels = false;
            reelSpinManagerModel.IsReelWorking = false;
            reelSpinManagerModel.IsReelFinished = true;

            // リール停止位置記録
            reelSpinManagerModel.LastStoppedReelData = reelGroup.RecordLastStoppedData();
            // コルーチン停止
            StopAutoTime();
        }
    }

    // リール自動停止のコルーチンストップ
    void StopAutoTime()
    {
        StopCoroutine(nameof(AutoStopByTime));
        reelSpinManagerModel.HasForceStop = false;
    }

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
}
using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
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
    public FlashManager FlashManager { get; private set; }

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
        Debug.Log("ReelData load done");
        Debug.Log("ReelManager awaken");

        data = new ReelManagerBehaviour();

        FlashManager = GetComponent<FlashManager>();
        FlashManager.SetReelObjects(reelObjects);
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
                Debug.Log("All Reels are stopped");

                // リール停止位置記録
                data.GenerateLastStopped(reelObjects);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        Debug.Log("Coroutines are stopped");
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
    // 指定リールが止められるか確認する
    public bool GetCanReelStop(ReelID reelID) => reelObjects[(int)reelID].GetCanStop();

    // リール出目データ
    // 最後に止めた出目
    public LastStoppedReelData GetLastStopped() => data.LastStopped;
    // 使用したリールテーブルID
    public int GetUsedReelTableID(ReelID reelID) => data.ReelTableManager.UsedReelTableID[(int)reelID];

    // 指定リール本体の明るさ変更
    public void SetReelBodyBrightness(int reelID, byte brightness) => reelObjects[reelID].SetReelBaseBrightness(brightness);
    // 指定したリールと図柄の明るさ変更
    public void SetReelSymbolBrightness(int reelID, ReelPosID symbolPos, byte r, byte g, byte b) => 
        reelObjects[reelID].SetSymbolBrightness(GetReelArrayIndex((int)symbolPos), r, g, b);

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
            Debug.Log("Reel start");
        }
        else { Debug.Log("Reel is working now"); }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID, int betAmounts, FlagId flagID, BonusType bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(data.CanStopReels)
        {
            // 止められる状態なら
            if (!reelObjects[(int)reelID].GetHasStopped())
            {
                // 中段の位置を得る
                int pushedPos = reelObjects[(int)reelID].GetReelPos(ReelPosID.Center);
                Debug.Log("Stopped:" + pushedPos);

                // 第一停止なら押したところの停止位置を得る
                if (!data.IsFirstReelPushed)
                {
                    data.IsFirstReelPushed = true;
                    data.FirstPushReel = reelID;
                    data.FirstPushPos = pushedPos;

                    Debug.Log("FirstPush:" + reelID);
                }

                // ここでディレイ(スベリコマ)を得て転送
                // 条件をチェック
                int tableIndex = data.ReelTableManager.FindTableToUse(reelID, reelObjects[(int)reelID].GetReelDatabase()
                    , flagID, data.FirstPushReel, betAmounts, (int)bonusID, data.RandomValue, data.FirstPushPos);

                // ディレイ(スベリコマ)を得る
                int delay = data.ReelTableManager.GetDelayFromTable(reelObjects[(int)reelID].GetReelDatabase(), pushedPos, tableIndex);
                Debug.Log("Stop:" + reelID + "Delay:" + delay);

                // リールを止める
                reelObjects[(int)reelID].StopReel(pushedPos, delay);

                // 停止したリール数を増やす
                data.StoppedReelCount += 1;
            }
            else
            {
                Debug.Log("Failed to stop the " + reelID.ToString());
            }
        }
        else
        {
            Debug.Log("ReelSpeed is not maximum speed");
        }
    }

    // リーチ状態か確認する
    public BigColor CheckRiichiStatus(List<PayoutLineData> payoutLines, int betAmounts)
    {
        // 払い出しラインとベット枚数から確認
        foreach(PayoutLineData line in payoutLines)
        {
            int redCount = 0;
            int blueCount = 0;
            int bb7Count = 0;

            Debug.Log("BetCondition:" + line.BetCondition + "Bet:" + betAmounts);
            // ベット条件を満たしているか確認
            if(betAmounts >= line.BetCondition)
            {
                // 停止中状態になっている停止予定位置のリールからリーチ状態か確認
                for (int i = 0; i < reelObjects.Length; i++)
                {
                    Debug.Log("WillStop Pos:" + reelObjects[i].GetWillStopPos());
                    Debug.Log("Pos:" + line.PayoutLines[i]);
                    Debug.Log("WillStop Symbol:" + reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]));
                    // 赤7をカウント
                    if (reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.RedSeven)
                    {
                        redCount += 1;

                        // 右リールの場合はBB7のカウントとしても増やす
                        if(i == (int)ReelID.ReelRight)
                        {
                            bb7Count += 1;
                        }
                    }
                    // 青7をカウント
                    if (reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BlueSeven)
                    {
                        blueCount += 1;
                    }

                    // BARをカウント(右以外)
                    if (i != (int)ReelID.ReelRight &&
                        reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BAR)
                    {
                        bb7Count += 1;
                    }
                }

                Debug.Log("Red:" + redCount);
                Debug.Log("Blue:" + blueCount);
                Debug.Log("Black:" + bb7Count);

                // 赤7がライン上に2つあれば赤7
                if(redCount == 2)
                {
                    Debug.Log("Riichi:" + BigColor.Red);
                    return BigColor.Red;
                }
                // 青7がライン上に2つあれば青7
                if (blueCount == 2)
                {
                    Debug.Log("Riichi:" + BigColor.Blue);
                    return BigColor.Blue;
                }
                // BARが2つ、またはBAR1つ赤7一つなら
                if (bb7Count == 2)
                {
                    Debug.Log("Riichi:" + BigColor.Black);
                    return BigColor.Black;
                }
            }
        }

        Debug.Log("Riichi:" + BigColor.None);
        return BigColor.None;
    }

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

    // func
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

    // リール停止可能にする(コルーチン用)
    private IEnumerator SetReelStopTimer()
    {
        data.CanStopReels = false;
        yield return new WaitForSeconds(ReelWaitTime);
        data.CanStopReels = true;
        Debug.Log("All reels are max speed");
    }

    // 全リールが停止したか確認
    private bool CheckAllReelStopped()
    {
        foreach (ReelObject obj in reelObjects)
        {
            // 止まっていないリールがまだあれば falseを返す
            if (!obj.GetHasStopped())
            {
                return false;
            }
        }
        return true;
    }
}

using ReelSpinGame_Reels;
using UnityEngine;
using System;
using System.IO;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // 目指す目標

    // 1:リールの回転
    // 2:図柄の実装
    // 3:リールの停止
    // 4:スベリ実装
    // 5:テーブル機能搭載

    // マネージャーが持つもの

    // 各リールのデータ(3つ)
    // 各リールのMonoBehaviour
    // 全リールへのコントロール

    // const 

    public enum ReelID { ReelLeft, ReelMiddle, ReelRight };


    // var

    // 動作中か
    public bool IsWorking { get; private set; }

    // 動作完了したか
    public bool IsFinished {  get; private set; }

    // 払い出し完了したか
    public bool HasFinishedCheck { get; private set; }

    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;
    [SerializeField] private string arrayPath;

    // 払い出しチェック用
    [SerializeField] private string normalPayoutData;
    [SerializeField] private string bigPayoutData;
    [SerializeField] private string jacPayoutData;
    [SerializeField] private string payoutLineData;
    private SymbolChecker symbolChecker;

    // 停止したリール数
    private int stopReelCount;

    
    // 初期化
    void Awake()
    {
        IsFinished = false;
        IsWorking = false;
        HasFinishedCheck = false;

        stopReelCount = 0;

        try
        {
            // リール配列の読み込み
            StreamReader arrayData = new StreamReader(arrayPath) ?? throw new System.Exception("Array path file is missing");

            // 各リールごとにデータを割り当てる
            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].SetReelData(new ReelData(19, arrayData));
            }

            Debug.Log("Array load done");

            // 払い出しラインの読み込み
            StreamReader payoutLines = new StreamReader(payoutLineData) ?? throw new System.Exception("PayoutLine file is missing");
            //StreamReader normalPayoutList = new StreamReader(arrayPath) ?? throw new System.Exception("Array path file is missing");
            //StreamReader bigPayoutList = new StreamReader(arrayPath) ?? throw new System.Exception("Array path file is missing");

            symbolChecker = new SymbolChecker(payoutLines);
        }
        catch(Exception e)
        {
            throw e;
        }
        finally
        {
            Debug.Log("ReelManager awaken");
        }
    }

    // func

    // リール始動
    public void StartReels()
    {
        // リールが回っていなければ回転
        if (!IsWorking)
        {
            IsFinished = false;
            HasFinishedCheck = false;

            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].StartReel(1.0f);
            }

            IsWorking = true;
            Debug.Log("Reel start");
        }

        else { Debug.Log("Reel is working now"); }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID)
    {
        // ここでディレイ(スベリコマ)を得て転送

        // リールが止まっていなければ停止
        if(!reelObjects[(int)reelID].IsStopping)
        {
            reelObjects[(int)reelID].StopReel(0);

            // 押したリールの数をカウント
            stopReelCount += 1;

            // 全リールが停止されていればまた回せるようにする
            if (stopReelCount == reelObjects.Length)
            {
                IsWorking = false;
                IsFinished = true;
                stopReelCount = 0;
                Debug.Log("All Reels are stopped");
            }
        }
        else
        {
            Debug.Log("Failed to stop the " + reelID.ToString());
        }
    }

    // 払い出し確認
    public void StartCheckPayout(int betAmounts)
    {
        if (!IsWorking)
        {
            symbolChecker.CheckPayout(reelObjects, betAmounts);
            HasFinishedCheck = true;
        }
        else
        {
            Debug.Log("Failed to check payout because reels are spinning");
        }
    }
}

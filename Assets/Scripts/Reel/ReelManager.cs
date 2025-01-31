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

    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;
    [SerializeField] private string arrayPath;

    // 払い出しチェック用
    [SerializeField] private string normalPayoutData;
    [SerializeField] private string bigPayoutData;
    [SerializeField] private string jacPayoutData;
    [SerializeField] private string payoutLineData;
    private SymbolChecker symbolChecker;

    // 動作中か
    private bool isWorking;

    // 停止したリール数
    private int stopReelCount;

            
    void Awake()
    {
        isWorking = false;
        stopReelCount = 0;

        try
        {
            StreamReader arrayData = new StreamReader(arrayPath) ?? throw new System.Exception("Array path file is missing");

            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].SetReelData(new ReelData(19, arrayData));
            }

            Debug.Log("Array load done");

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

    public void StartReels()
    {
        // リールが回っていなければ回転

        if(!isWorking)
        {
            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].StartReel(1.0f);
            }

            isWorking = true;
            Debug.Log("Reel start");
        }

        else { Debug.Log("Reel is working now"); }
    }

    public void StopSelectedReel(ReelID reelID)
    {
        // リール停止

        // ここでディレイ(スベリコマ)を得て転送

        if(!reelObjects[(int)reelID].IsStopping)
        {
            reelObjects[(int)reelID].StopReel(0);
            stopReelCount += 1;

            // 全リールが停止されていればまた回せるようにする
            if (stopReelCount == reelObjects.Length)
            {
                isWorking = false;
                stopReelCount = 0;
                Debug.Log("All Reels are stopped");
                
                CheckPayout();
            }
        }
        else
        {
            Debug.Log("Failed to stop the " + reelID.ToString());
        }
    }

    public void CheckPayout()
    {
        for(int i = 0; i < reelObjects.Length; i++)
        {
            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelPos(ReelData.ReelPosID.Upper));
            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelSymbol(ReelData.ReelPosID.Upper));

            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelPos(ReelData.ReelPosID.Center));
            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelSymbol(ReelData.ReelPosID.Center));

            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelPos(ReelData.ReelPosID.Lower));
            Debug.Log(reelObjects[i].name + reelObjects[i].ReelData.GetReelSymbol(ReelData.ReelPosID.Lower));
        }
    }
}

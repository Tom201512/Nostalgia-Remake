using ReelSpinGame_Reels;
using ReelSpinGame_Reels.ReelArray;
using System.Collections.ObjectModel;
using UnityEngine;
using static ReelSpinGame_Reels.ReelArray.ReelArray;

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
    //public const int MaxReels = 3;

    public enum ReelID { ReelLeft, ReelMiddle, ReelRight };


    // var

    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;

    // 動作中か
    private bool isWorking;

    // 停止したリール数
    private int stopReelCount;

    private ReadOnlyCollection<ReadOnlyCollection<ReelSymbols>> array;
            
    void Awake()
    {
        isWorking = false;
        stopReelCount = 0;
        Debug.Log(ReelArray.LeftArray[0]);

        array = new ReadOnlyCollection<ReadOnlyCollection<ReelSymbols>>(
            new[]
            {
                ReelArray.LeftArray,
                ReelArray.MiddleArray,
                ReelArray.RightArray,
            });

        for(int i = 0; i < reelObjects.Length; i++)
        {
            reelObjects[i].SetReelData(new ReelData(19, array[i]));
        }

        Debug.Log("ReelManager awaken");
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
            reelObjects[(int)reelID].StopReel(4);
            stopReelCount += 1;

            // 全リールが停止されていればまた回せるようにする
            if (stopReelCount == reelObjects.Length)
            {
                isWorking = false;
                stopReelCount = 0;
                Debug.Log("All Reels are stopped");
            }
        }
        else
        {
            Debug.Log("Failed to stop the " + reelID.ToString());
        }
    }
}

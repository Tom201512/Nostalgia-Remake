using ReelSpinGame_Reels;
using ReelSpinGame_Reels.ReelArray;
using System;
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

    [SerializeField] private ReelObject[] reelObjects;

    private ReadOnlyCollection<ReadOnlyCollection<ReelSymbols>> array;
            
    void Awake()
    {
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
            reelObjects[i].SetReelData(new ReelData((int)ReelID.ReelLeft, array[i]));
        }

        //Debug.Log(reelObjects[(int)ReelID.ReelLeft].ReelData.Array);
    }

    // func

    public void StartReels()
    {
        // リールが回っていなければ回転
        for(int i = 0; i < reelObjects.Length; i++)
        {
            reelObjects[i].StartReel(1.0f);
        }
    }

    public void StopSelectedReel(ReelID reelID)
    {
        // リール停止
    }
}

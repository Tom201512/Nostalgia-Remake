using ReelSpinGame_Main.File;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // const 
    // リール数
    public const int ReelAmounts = 3; 
    // リール識別用ID
    public enum ReelID { ReelLeft, ReelMiddle, ReelRight };

    // var
    // 全リールが動作中か
    public bool IsWorking { get; private set; }
    // 動作完了したか
    public bool IsFinished {  get; private set; }
    // 第一停止をしたか
    private bool isFirstReelPushed;
    // 最初に止めたリール番号
    private ReelID firstPushReel;
    // 第一停止リールの停止位置
    private int firstPushPos;
    
    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;
    // リール制御
    private ReelTableManager reelTableManager;

    // 最後に止めた位置
    public List<int> LastPos { get; private set; }
    // 最後に止まった出目
    public List<List<ReelData.ReelSymbols>> LastSymbols { get; private set; }

    // 初期化
    void Awake()
    {
        IsFinished = true;
        IsWorking = false;

        isFirstReelPushed = false;
        firstPushReel = ReelID.ReelLeft;
        firstPushPos = 0;

        LastPos = new List<int>();
        LastSymbols = new List<List<ReelData.ReelSymbols>>();

        // リール配列データの設定
        string[] reelConditionDatas = { FileManager.ReelLeftCondtiion, FileManager.ReelMiddleCondtiion, FileManager.ReelRightCondtiion };
        string[] delayTableDatas = { FileManager.ReelLeftTable, FileManager.ReelMiddleTable, FileManager.ReelRightTable };

        // リール条件とテーブルの数が一致するか確認する
        if (reelConditionDatas.Length != ReelAmounts ||
            delayTableDatas.Length != ReelAmounts)
        {
            throw new System.Exception("Either data of conditions and tables doesn't match the amount of reels");
        }

        try
        {
            StreamReader arrayData = new StreamReader(FileManager.ReelArrayPath);

            // 各リールごとにデータを割り当てる
            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].SetReelData(new ReelData(19, arrayData));
            }

            Debug.Log("ReelData load done");
            Debug.Log("Array load done");

            // リール制御読み込み

            List<StreamReader> conditions = new List<StreamReader>();
            List<StreamReader> tables = new List<StreamReader>();

            for (int i = 0; i < ReelAmounts; i++)
            {
                conditions.Add(new StreamReader(reelConditionDatas[i]) ?? throw new System.Exception("Condition file at" + i + "is missing"));
                tables.Add(new StreamReader(delayTableDatas[i]) ?? throw new System.Exception("ReelTable L file t" + i + "is missing"));
            }
            reelTableManager = new ReelTableManager(conditions, tables);

            Debug.Log("ReelData load done");
        }
        finally
        {
            Debug.Log("ReelManager awaken");
        }
    }

    void Start()
    {
        // リールの明るさを変更するテスト
        //reelObjects[(int)ReelID.ReelLeft].SetSymbolBrightness((int)ReelData.ReelPosArrayID.Center, 128);
        //reelObjects[(int)ReelID.ReelLeft].SetReelBaseBrightness(128);
    }

    void Update()
    {
        // 全リールが停止したかチェック
        if(IsWorking && CheckAllReelStopped())
        {
            IsWorking = false;
            IsFinished = true;
            Debug.Log("All Reels are stopped");

            // リール停止位置記録
            GenerateLastStopped();
        }
    }

    // func

    // 指定したリールの現在位置を返す(デバッグ用)
    public int GetCurrentReelPos(int reelID) => reelObjects[reelID].GetReelPos((int)ReelData.ReelPosID.Lower);

    // リール始動
    public void StartReels()
    {
        // リールが回っていなければ回転
        if (!IsWorking)
        {
            IsFinished = false;
            IsWorking = true;
            isFirstReelPushed = false;

            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].StartReel(1.0f);
            }
            Debug.Log("Reel start");
        }
        else { Debug.Log("Reel is working now"); }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID)
    {
        // 押した位置
        int pushedPos = reelObjects[(int)reelID].GetStoppedPos();
        Debug.Log("Stopped:" + pushedPos);

        // 第一停止なら押したところの停止位置を得る
        if (!isFirstReelPushed)
        {
            isFirstReelPushed = true;
            firstPushReel = reelID + 1;
            firstPushPos = pushedPos;

            Debug.Log("FirstPush:" + reelID);
        }

        // ここでディレイ(スベリコマ)を得て転送
        // 条件をチェック
        int tableIndex = reelTableManager.FindTableToUse(reelID, 0, (int)firstPushReel, 0, 3, 0, firstPushPos);

        // 先ほど得たディレイ分リール停止を遅らせる
        if (!reelObjects[(int)reelID].HasStopped)
        {
            int delay = reelTableManager.GetDelayFromTable(reelID, pushedPos, tableIndex);
            Debug.Log("Stop:" + reelID + "Delay:" + delay);
            reelObjects[(int)reelID].StopReel(delay);
        }
        else
        {
            Debug.Log("Failed to stop the " + reelID.ToString());
        }
    }

    // 全リールが停止したか確認
    private bool CheckAllReelStopped()
    {
        foreach (ReelObject obj in reelObjects)
        {
            // 止まっていないリールがまだあれば falseを返す
            if (!obj.HasStopped)
            {
                return false;
            }
        }
        return true;
    }

    // 最後に止めた結果を作る
    private void GenerateLastStopped()
    {
        // 初期化
        LastPos.Clear();
        LastSymbols.Clear();

        string posBuffer = "";

        // リール図柄を作成する
        for(int i = 0; i < reelObjects.Length; i++)
        {
            LastPos.Add(reelObjects[i].GetReelPos((int)ReelData.ReelPosID.Lower));
            posBuffer += LastPos[i];

            Debug.Log("Position:" + LastPos[i]);

            // データ作成
            LastSymbols.Add(new List<ReelData.ReelSymbols>());

            // 各位置の図柄を得る(枠下2段目から枠上2段目まで)
            for (int j = (int)ReelData.ReelPosID.Lower3rd; j < (int)ReelData.ReelPosID.Upper3rd; j++)
            {
                LastSymbols[i].Add(reelObjects[i].ReelData.GetReelSymbol(j));
                Debug.Log("Symbol:" + reelObjects[i].ReelData.GetReelSymbol(j));
            }
        }
        Debug.Log("Final ReelPosition" + posBuffer);

        // 各リールごとに表示(デバッグ)
        foreach(List<ReelData.ReelSymbols> reelResult in LastSymbols)
        {
            Debug.Log("Reel:");
            for(int i = 0; i < reelResult.Count; i++)
            {
                Debug.Log(reelResult[i]);
            }
        }
    }
}

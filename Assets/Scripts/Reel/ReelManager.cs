using ReelSpinGame_Bonus;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class ReelManager : MonoBehaviour
{
    // リールマネージャー

    // const 
    // リール数
    public const int ReelAmounts = 3; 
    // リール識別用ID
    public enum ReelID { ReelLeft, ReelMiddle, ReelRight };
    // 最大 ランダムテーブル数(1~6)
    const int MaxRandomLots = 6;

    // var
    // 全リールが動作中か
    public bool IsWorking { get; private set; }
    // 動作完了したか
    public bool IsFinished {  get; private set; }
    // 停止可能になったか(リール速度が一定になって0.5秒後)
    public bool CanStop {  get; private set; }
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
    // フラッシュ機能
    public FlashManager FlashManager { get; private set; }

    // 最後に止めた位置
    public List<int> LastPos { get; private set; }
    // 最後に止まった出目
    public List<List<ReelSymbols>> LastSymbols { get; private set; }

    // 強制ランダム数値
    [SerializeField] private bool instantRandomMode;
    // 強制時のランダム数値
    [Range(1,6),SerializeField] private int instantRandomValue;
    // リールテーブルのランダム数値
    public int RandomValue { get; private set; }

    // 初期化
    void Awake()
    {
        IsFinished = true;
        IsWorking = false;
        CanStop = false;

        isFirstReelPushed = false;
        firstPushReel = ReelID.ReelLeft;
        firstPushPos = 0;
        RandomValue = 0;

        LastPos = new List<int>();
        LastSymbols = new List<List<ReelSymbols>>();

        try
        {
            // 各リールごとにデータを割り当てる
            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].SetReelData(i,19);
            }
            reelTableManager = new ReelTableManager();
            Debug.Log("ReelData load done");
        }
        finally
        {
            Debug.Log("ReelManager awaken");
        }

        FlashManager = GetComponent<FlashManager>();
        FlashManager.SetReelObjects(reelObjects);
    }

    private void Start()
    {
        //FlashManager.StartFlash();
    }

    void Update()
    {
        if(IsWorking)
        {
            // 全リールが等速かチェック(停止可能にする)
            if(!CanStop && CheckReelSpeedMaximum() && !IsInvoking())
            {
                Invoke("EnableReelStop", 0.5f);
            }

            // 全リールが停止したかチェック
            if (CheckAllReelStopped())
            {
                CanStop = false;
                IsWorking = false;
                IsFinished = true;
                Debug.Log("All Reels are stopped");

                // リール停止位置記録
                GenerateLastStopped();
            }
        }
    }

    // func
    // 指定したリールの現在位置を返す
    public int GetCurrentReelPos(int reelID) => reelObjects[reelID].GetReelPos((int)ReelPosID.Lower);
    // 指定したリールを止めた位置を返す
    public int GetStoppedReelPos(int reelID) => reelObjects[reelID].GetLastPressedPos();
    // 指定したリールのディレイ数を返す
    public int GetLastDelay(int reelID) => reelObjects[reelID].GetLastDelay();
    // 指定したリールの使用テーブルIDを返す
    public int GetLastTableID(int reelID) => reelTableManager.UsedReelTableID[reelID];
    // 指定リール本体の明るさ変更
    public void SetReelBodyBrightness(int reelID, byte brightness) => reelObjects[reelID].SetReelBaseBrightness(brightness);
    // 指定したリールと図柄の明るさ変更
    public void SetReelSymbolBrightness(int reelID, ReelPosArrayID symbolPos, byte r, byte g, byte b) => 
        reelObjects[reelID].SetSymbolBrightness((int)symbolPos, r, g, b);

    // リール始動
    public void StartReels()
    {
        // ランダム数値決定

        RandomValue = Random.Range(1, MaxRandomLots);
        if (instantRandomMode)
        {
            RandomValue = instantRandomValue;
        }


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
    public void StopSelectedReel(ReelID reelID, int betAmounts, FlagLots.FlagId flagID, BonusManager.BonusType bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(CanStop)
        {
            // 止められる状態なら
            if (!reelObjects[(int)reelID].HasStopped)
            {
                // 押した位置
                int pushedPos = reelObjects[(int)reelID].GetPressedPos();
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
                int tableIndex = reelTableManager.FindTableToUse(reelObjects[(int)reelID].ReelData
                    , (int)flagID, (int)firstPushReel, betAmounts, (int)bonusID, RandomValue, firstPushPos);

                // 先ほど得たディレイ分リール停止を遅らせる
                int delay = reelTableManager.GetDelayFromTable(reelObjects[(int)reelID].ReelData, pushedPos, tableIndex);
                Debug.Log("Stop:" + reelID + "Delay:" + delay);
                reelObjects[(int)reelID].StopReel(delay);
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

    // リール停止可能にする(コルーチン用)
    private void EnableReelStop()
    {
        CanStop = true;
        Debug.Log("All reels are max speed");
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

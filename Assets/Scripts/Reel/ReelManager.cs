using ReelSpinGame_Reels;
using static ReelSpinGame_Reels.ReelData;
using System.IO;
using UnityEngine;
using static PayoutChecker;

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
    // 判定完了したか
    public bool HasFinishedCheck { get; private set; }
    
    // リールのオブジェクト
    [SerializeField] private ReelObject[] reelObjects;
    // リール配列のファイル
    [SerializeField] private string arrayPath;

    // 払い出し表のデータ
    [SerializeField] private string normalPayoutData;
    [SerializeField] private string bigPayoutData;
    [SerializeField] private string jacPayoutData;
    [SerializeField] private string payoutLineData;

    // 図柄判定
    private PayoutChecker payoutChecker;

    // 最後に当たった結果
    public PayoutResultBuffer LastPayoutResult { get; private set; }

    // 初期化
    void Awake()
    {
        IsFinished = true;
        IsWorking = false;
        HasFinishedCheck = true;

        LastPayoutResult = new PayoutResultBuffer(0, 0, false);

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

            // 払い出しデータの読み込み
            StreamReader normalPayout = new StreamReader(normalPayoutData) ?? throw new System.Exception("NormalPayoutData file is missing"); 
            StreamReader bigPayout = new StreamReader(bigPayoutData) ?? throw new System.Exception("BigPayoutData file is missing");
            StreamReader jacPayout = new StreamReader(jacPayoutData) ?? throw new System.Exception("JacPayoutData file is missing");

            payoutChecker = new PayoutChecker(normalPayout, bigPayout, jacPayout, payoutLines, PayoutChecker.PayoutCheckMode.PayoutNormal);
        }
        finally
        {
            Debug.Log("ReelManager awaken");
        }
    }

    void Update()
    {
        // 全リールが停止したかチェック
        if(IsWorking && CheckAllReelStopped())
        {
            IsWorking = false;
            IsFinished = true;
            Debug.Log("All Reels are stopped");
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
            IsWorking = true;

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
        // ここでディレイ(スベリコマ)を得て転送
        // リールが止まっていなければ停止
        if(!reelObjects[(int)reelID].HasStopped)
        {
            reelObjects[(int)reelID].StopReel(0);
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
            LastPayoutResult = payoutChecker.CheckPayoutLines(reelObjects, betAmounts);
            HasFinishedCheck = true;
        }
        else
        {
            Debug.Log("Failed to check payout because reels are spinning");
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
}

using ReelSpinGame_Reels;
using System.Collections;
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
    public ReelManagerBehaviour Data { get; private set; }

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

        Data = new ReelManagerBehaviour();

        FlashManager = GetComponent<FlashManager>();
        FlashManager.SetReelObjects(reelObjects);
    }

    void Update()
    {
        // リールが動いている時は
        if (Data.IsWorking)
        {
            // 全リールが等速かチェック(停止可能にする)
            if (!Data.CanStop && CheckReelSpeedMaximum() && !IsInvoking())
            {
                StartCoroutine(nameof(SetReelStopTimer));
            }

            // 全リールが停止したかチェック
            if (CheckAllReelStopped())
            {
                Data.SetCanStop(false);
                Data.SetIsWorking(false);
                Data.SetIsFinished(true);
                Debug.Log("All Reels are stopped");

                // リール停止位置記録
                Data.GenerateLastStopped(reelObjects);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        Debug.Log("Coroutines are stopped");
    }

    // func
    // 指定したリールの現在位置を返す
    public int GetCurrentReelPos(int reelID) => reelObjects[reelID].ReelData.GetReelPos((int)ReelPosID.Lower);
    // 指定したリールを止めた位置を返す
    public int GetStoppedReelPos(int reelID) => reelObjects[reelID].GetLastPressedPos();
    // 指定したリールのディレイ数を返す
    public int GetLastDelay(int reelID) => reelObjects[reelID].GetLastDelay();

    // 指定リール本体の明るさ変更
    public void SetReelBodyBrightness(int reelID, byte brightness) => reelObjects[reelID].SetReelBaseBrightness(brightness);
    // 指定したリールと図柄の明るさ変更
    public void SetReelSymbolBrightness(int reelID, ReelPosID symbolPos, byte r, byte g, byte b) => 
        reelObjects[reelID].SetSymbolBrightness(GetReelArrayIndex((int)symbolPos), r, g, b);

    // リール始動
    public void StartReels()
    {
        // ランダム数値決定
        Data.SetRandomValue(instantRandomMode, instantRandomValue);

        // リールが回っていなければ回転
        if (!Data.IsWorking)
        {
            Data.SetIsFinished(false);
            Data.SetIsWorking(true);
            Data.SetIsFirstReelPushed(false);

            for (int i = 0; i < reelObjects.Length; i++)
            {
                reelObjects[i].StartReel(1.0f);
            }
            Debug.Log("Reel start");
        }
        else { Debug.Log("Reel is working now"); }
    }

    // 各リール停止
    public void StopSelectedReel(ReelID reelID, int betAmounts, FlagId flagID, BonusType bonusID)
    {
        // 全リール速度が最高速度になっていれば
        if(Data.CanStop)
        {
            // 止められる状態なら
            if (!reelObjects[(int)reelID].HasStopped)
            {
                // 押した位置
                int pushedPos = reelObjects[(int)reelID].GetPressedPos();
                Debug.Log("Stopped:" + pushedPos);

                // 第一停止なら押したところの停止位置を得る
                if (!Data.IsFirstReelPushed)
                {
                    Data.SetIsFirstReelPushed(true);
                    Data.SetFirstPushReel(reelID);
                    Data.SetFirstPushPos(pushedPos);

                    Debug.Log("FirstPush:" + reelID);
                }

                // ここでディレイ(スベリコマ)を得て転送
                // 条件をチェック
                int tableIndex = Data.ReelTableManager.FindTableToUse(reelObjects[(int)reelID].ReelData
                    , flagID, Data.FirstPushReel, betAmounts, (int)bonusID, Data.RandomValue, Data.FirstPushPos);

                // 先ほど得たディレイ分リール停止を遅らせる
                int delay = Data.ReelTableManager.GetDelayFromTable(reelObjects[(int)reelID].ReelData, pushedPos, tableIndex);
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
    private IEnumerator SetReelStopTimer()
    {
        Data.SetCanStop(false);
        yield return new WaitForSeconds(ReelWaitTime);
        Data.SetCanStop(true);
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
}

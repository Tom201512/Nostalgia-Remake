using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;

public class FlagLotsTest : MonoBehaviour
{
    // フラグ処理のテスト用
    private FlagLots flagLots;
    private FlagCounter flagCounter;

    // フラグテーブル
    // 設定値
    [SerializeField] private int setting;
    // 低確率時
    [SerializeField] private string flagTableAPath;
    // 高確率時
    [SerializeField] private string flagTableBPath;
    // BIG中テーブル
    [SerializeField] private string flagTableBIGPath;
    // JACはずれ確率
    [SerializeField] private int jacNoneProb;

    void Awake()
    {
        // 例外処理
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid jacNoneProb, must be higher that 0"); }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            setting = UnityEngine.Random.Range(1, 6);
        }

        Debug.Log("Setting:" + setting);

        if (jacNoneProb < 0) 
        { 
            throw new System.Exception("Invalid jacNoneProb, must be higher that 0");
        }

        // ファイル読み込み
        try
        {            
            // 設定値の部分になったら読み込む
            flagLots = new FlagLots(setting);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            // カウンタ作成
            flagCounter = new FlagCounter(0);
        }
    }

    void Update()
    {
        // フラグ抽選
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.GetFlagLots();

            // 小役ならカウンタを増やし、はずれは減らす(通常時のみ)
            if(flagLots.CurrentTable == FlagLots.FlagLotMode.NormalA ||
                flagLots.CurrentTable == FlagLots.FlagLotMode.NormalB)
            {
                IncreaseFlagCounter();
                ChangeNormalTable();
            }
        }

        // フラグ変更 (通常時)
        if (OriginalInput.CheckOneKeyInput(KeyCode.A))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalA);
        }

        // フラグ変更 (BIG小役ゲーム時)
        if (OriginalInput.CheckOneKeyInput(KeyCode.B))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.BigBonus);
        }

        // フラグ変更 (JACゲーム時)
        if (OriginalInput.CheckOneKeyInput(KeyCode.C))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.JacGame);
        }

        // 小役カウンターリセット
        if (OriginalInput.CheckOneKeyInput(KeyCode.R))
        {
            flagCounter.ResetCounter();
        }
    }

    private void ChangeNormalTable()
    {
        // カウンタが0以上の場合は低確率
        if (flagLots.CurrentTable == FlagLots.FlagLotMode.NormalB &&
            flagCounter.Counter >= 0)
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalA);
            Debug.Log("Table chagned to:" + FlagLots.FlagLotMode.NormalA.ToString());
        }

        // カウンタが0より少ないなら高確率
        else if (flagLots.CurrentTable == FlagLots.FlagLotMode.NormalA &&
            flagCounter.Counter < 0)
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalB);
            Debug.Log("Table chagned to:" + FlagLots.FlagLotMode.NormalB.ToString());
        }
    }

    private void IncreaseFlagCounter()
    {
        if (flagLots.CurrentFlag == FlagLots.FlagId.FlagBell)
        {
            flagCounter?.IncreaseCounter(10);
        }

        if (flagLots.CurrentFlag == FlagLots.FlagId.FlagMelon)
        {
            flagCounter?.IncreaseCounter(15);
        }

        if (flagLots.CurrentFlag == FlagLots.FlagId.FlagCherry2)
        {
            flagCounter?.IncreaseCounter(2);
        }

        if (flagLots.CurrentFlag == FlagLots.FlagId.FlagCherry4)
        {
            flagCounter?.IncreaseCounter(4);
        }

        if (flagLots.CurrentFlag == FlagLots.FlagId.FlagNone)
        {
            flagCounter?.DecreaseCounter(6, 3);
        }
    }
}


using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using System;
using System.IO;
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

    // Start is called before the first frame update
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

        if (jacNoneProb < 0) { throw new System.Exception("Invalid jacNoneProb, must be higher that 0"); }

        try
        {
            StreamReader tableA = new StreamReader(flagTableAPath);
            StreamReader tableB = new StreamReader(flagTableBPath);
            StreamReader tableBIG = new StreamReader(flagTableBIGPath);

            // 設定値をもとにデータを得る(設定値の列まで読み込む)
            for (int i = 0; i < setting - 1; i++)
            {
                tableA.ReadLine();
                tableB.ReadLine();
                tableBIG.ReadLine();
            }

            flagLots = new FlagLots(setting, tableA, tableB, tableBIG, jacNoneProb);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            flagCounter = new FlagCounter(0);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // DrawLots
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.GetFlagLots();

            // 小役ならカウンタを増やし、はずれは減らす

            if(flagLots.CurrentFlag == FlagLots.FlagId.FlagBell)
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

            if(flagLots.CurrentFlag == FlagLots.FlagId.FlagNone)
            {
                flagCounter?.DecreaseCounter(6,3);
            }

            ChangeNormalTable();
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

    private void ReadFile(string path)
    {

    }
}


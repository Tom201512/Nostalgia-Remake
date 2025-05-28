using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class FlagLotsTest : MonoBehaviour
{
    /*
    // フラグ処理のテスト用
    private FlagLots flagLots;
    private FlagCounter flagCounter;

    // フラグテーブル
    // 設定値
    [SerializeField] private int setting;
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

        //Debug.Log("Setting:" + setting);

        if (jacNoneProb < 0) 
        { 
            throw new System.Exception("Invalid jacNoneProb, must be higher that 0");
        }

        flagLots = GetComponent<FlagLots>();
        flagCounter = new FlagCounter(0);
    }

    void Update()
    {
        // フラグ抽選
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.FlagBehaviour.GetFlagLots(setting, 3);

            // 小役ならカウンタを増やし、はずれは減らす(通常時のみ)
            if(flagLots.CurrentTable == FlagLots.FlagLotMode.Normal)
            {
                IncreaseFlagCounter();
            }
        }

        // フラグ変更 (通常時)
        if (OriginalInput.CheckOneKeyInput(KeyCode.A))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.Normal);
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
    }*/
}


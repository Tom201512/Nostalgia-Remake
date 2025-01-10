using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // フラグ抽選

        // const

        // 最大フラグ数
        const int MAX_FLAG_LOTS = 16384;

        // var

        // 現在フラグ
        int flagNum = 0;

        // 参照するテーブルID
        int currentTable = 0;

        public FlagLots(FlagLotsTest flagLotsTest)
        {
            flagLotsTest.DrawLots += GetFlagLots;
        }

        // func

        public void GetFlagLots()
        {
            flagNum = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flagNum);
        }
    }
}

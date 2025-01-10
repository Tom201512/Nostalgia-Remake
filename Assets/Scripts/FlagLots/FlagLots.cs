using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // �t���O���I

        // const

        // �ő�t���O��
        const int MAX_FLAG_LOTS = 16384;

        // var

        // ���݃t���O
        int flagNum = 0;

        // �Q�Ƃ���e�[�u��ID
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

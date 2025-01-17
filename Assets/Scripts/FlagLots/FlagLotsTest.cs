using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;
using System;
using UnityEngine;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;

public class FlagLotsTest : MonoBehaviour
{
    // �t���O�����̃e�X�g�p
    private FlagLots flagLots;
    private FlagCounter flagCounter;

    // Start is called before the first frame update
    void Awake()
    {
        flagLots = new FlagLots(6);
        flagCounter = new FlagCounter(0);
    }

    // Update is called once per frame
    void Update()
    {

        // DrawLots
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.GetFlagLots();

            // �����Ȃ�J�E���^�𑝂₵�A�͂���͌��炷

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

        // �J�E���^��0�ȏ�̏ꍇ�͒�m��
        if (flagLots.CurrentTable == FlagLots.FlagLotMode.NormalB &&
            flagCounter.Counter >= 0)
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalA);
            Debug.Log("Table chagned to:" + FlagLots.FlagLotMode.NormalA.ToString());
        }

        // �J�E���^��0��菭�Ȃ��Ȃ獂�m��
        else if (flagLots.CurrentTable == FlagLots.FlagLotMode.NormalA &&
            flagCounter.Counter < 0)
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalB);
            Debug.Log("Table chagned to:" + FlagLots.FlagLotMode.NormalB.ToString());
        }
    }
}


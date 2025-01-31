using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using System;
using System.IO;
using UnityEngine;

public class FlagLotsTest : MonoBehaviour
{
    // �t���O�����̃e�X�g�p
    private FlagLots flagLots;
    private FlagCounter flagCounter;

    // �t���O�e�[�u��

    // �ݒ�l
    [SerializeField] private int setting;

    // ��m����
    [SerializeField] private string flagTableAPath;

    // ���m����
    [SerializeField] private string flagTableBPath;

    // BIG���e�[�u��
    [SerializeField] private string flagTableBIGPath;

    // JAC�͂���m��
    [SerializeField] private int jacNoneProb;

    // Start is called before the first frame update
    void Awake()
    {
        // ��O����
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid jacNoneProb, must be higher that 0"); }
        // 0�Ȃ烉���_����I��
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

            // �ݒ�l�����ƂɃf�[�^�𓾂�(�ݒ�l�̗�܂œǂݍ���)
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

    private void ReadFile(string path)
    {

    }
}


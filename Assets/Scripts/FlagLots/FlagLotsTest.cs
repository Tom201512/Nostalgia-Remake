using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using System;
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

        if (jacNoneProb < 0) 
        { 
            throw new System.Exception("Invalid jacNoneProb, must be higher that 0");
        }

        // �t�@�C���ǂݍ���
        try
        {            
            // �ݒ�l�̕����ɂȂ�����ǂݍ���
            flagLots = new FlagLots(setting);
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            // �J�E���^�쐬
            flagCounter = new FlagCounter(0);
        }
    }

    void Update()
    {
        // �t���O���I
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.GetFlagLots();

            // �����Ȃ�J�E���^�𑝂₵�A�͂���͌��炷(�ʏ펞�̂�)
            if(flagLots.CurrentTable == FlagLots.FlagLotMode.NormalA ||
                flagLots.CurrentTable == FlagLots.FlagLotMode.NormalB)
            {
                IncreaseFlagCounter();
                ChangeNormalTable();
            }
        }

        // �t���O�ύX (�ʏ펞)
        if (OriginalInput.CheckOneKeyInput(KeyCode.A))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.NormalA);
        }

        // �t���O�ύX (BIG�����Q�[����)
        if (OriginalInput.CheckOneKeyInput(KeyCode.B))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.BigBonus);
        }

        // �t���O�ύX (JAC�Q�[����)
        if (OriginalInput.CheckOneKeyInput(KeyCode.C))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.JacGame);
        }

        // �����J�E���^�[���Z�b�g
        if (OriginalInput.CheckOneKeyInput(KeyCode.R))
        {
            flagCounter.ResetCounter();
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


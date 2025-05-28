using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Lots.FlagCounter;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

public class FlagLotsTest : MonoBehaviour
{
    /*
    // �t���O�����̃e�X�g�p
    private FlagLots flagLots;
    private FlagCounter flagCounter;

    // �t���O�e�[�u��
    // �ݒ�l
    [SerializeField] private int setting;
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
        // �t���O���I
        if (OriginalInput.CheckOneKeyInput(KeyCode.Space))
        {
            flagLots?.FlagBehaviour.GetFlagLots(setting, 3);

            // �����Ȃ�J�E���^�𑝂₵�A�͂���͌��炷(�ʏ펞�̂�)
            if(flagLots.CurrentTable == FlagLots.FlagLotMode.Normal)
            {
                IncreaseFlagCounter();
            }
        }

        // �t���O�ύX (�ʏ펞)
        if (OriginalInput.CheckOneKeyInput(KeyCode.A))
        {
            flagLots.ChangeTable(FlagLots.FlagLotMode.Normal);
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


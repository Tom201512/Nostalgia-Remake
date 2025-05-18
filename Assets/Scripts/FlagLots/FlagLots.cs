using ReelSpinGame_Datas;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // �t���O���I

        // const
        // �ő�t���O��
        const int MaxFlagLots = 16384;
        // �e�[�u���V�[�N�ʒu
        const int SeekNum = 6;

        // enum
        // �t���OID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJacIn, FlagJac }
        // �t���O�e�[�u��
        public enum FlagLotMode { Normal, BigBonus, JacGame };

        // var
        // �t���O�f�[�^�x�[�X
        [SerializeField] FlagDatabase flagDatabase;
        // ���݃t���O(�v���p�e�B)
        public FlagId CurrentFlag { get; private set; }
        // �Q�Ƃ���e�[�u��ID
        public FlagLotMode CurrentTable { get; private set; }
        // �t���O�J�E���^
        public FlagCounter.FlagCounter FlagCounter { get; private set; }

        // �f�o�b�O�p(������)
        [SerializeField] private bool useInstant;
        [SerializeField] private FlagId instantFlagID;

        // ���I����(�ŏI�I�ɓ��I�����t���O���Q�Ƃ���̂Ɏg��)
        private FlagId[] lotResultNormal = new FlagId[] 
        {
            FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJacIn
        };

        // BIG CHANCE��
        private FlagId[] lotResultBig = new FlagId[] 
        {
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagReplayJacIn,
            FlagId.FlagBell
        };

        void Awake()
        {
            FlagCounter = new FlagCounter.FlagCounter(0);
        }

        // func
        // �e�[�u���ύX
        public void ChangeTable(FlagLotMode mode)
        {
            Debug.Log("Changed mode:" + mode);
            CurrentTable = mode;
        }

        // �t���O���I�̊J�n
        public void GetFlagLots(int setting, int betAmounts)
        {
            // �����_���e�[�u�������߂�

            // ������������ꍇ
            if(useInstant)
            {
                CurrentFlag = instantFlagID;
            }
            else
            {
                // ���݂̎Q�ƃe�[�u�������Ƃɒ��I
                switch (CurrentTable)
                {
                    case FlagLotMode.Normal:

                        // �J�E���^��0��菭�Ȃ��Ȃ獂�m��
                        if (FlagCounter.Counter < 0)
                        {
                            CurrentFlag = CheckResultByTable(setting, betAmounts, flagDatabase.NormalBTable, lotResultNormal);
                        }
                        // �J�E���^��0�ȏ�̏ꍇ�͒�m��
                        else
                        {
                            CurrentFlag = CheckResultByTable(setting, betAmounts, flagDatabase.NormalATable, lotResultNormal);
                        }

                        break;

                    case FlagLotMode.BigBonus:
                        CurrentFlag = CheckResultByTable(setting, flagDatabase.BigTable, lotResultBig);
                        break;

                    case FlagLotMode.JacGame:
                        CurrentFlag = BonusGameLots(flagDatabase.JacNonePoss);
                        break;

                    default:
                        Debug.LogError("No table found");
                        break;

                }
            }
            Debug.Log("Flag:" + CurrentFlag);
        }

        // �I�������t���O�ɂ��� �������Ȃǂł̎g�p
        public void SelectFlag(FlagId flagID)
        {
            Debug.Log("Flag:" + CurrentFlag);
            CurrentFlag = flagID;
        }

        // �e�[�u������t���O����
        private FlagId CheckResultByTable(int setting, FlagDataSets flagTable, FlagId[] lotResult)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;
            int flag = GetFlag();

            int index = 0;
            foreach(float f in flagTable.FlagDataBySettings[setting - 1].FlagTable)
            {
                //�e�����Ƃɒ��I
                flagCheckNum += Mathf.RoundToInt((float)MaxFlagLots / f);

                if (flag < flagCheckNum)
                {
                    return lotResult[index];
                }
                index += 1;
            }

            // ����������Ȃ����"�͂���"��Ԃ�
            return FlagId.FlagNone;
        }

        // �e�[�u���A�ݒ�l�ƃx�b�g��������t���O����
        private FlagId CheckResultByTable(int setting, int betAmounts, FlagDataSets flagTable, FlagId[] lotResult)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // �x�b�g�����ɍ��킹���e�[�u�����Q�Ƃ���悤�ɂ���
            int offset = SeekNum * (betAmounts - 1);
            Debug.Log(offset);

            int index = 0;

            Debug.Log(setting + offset - 1);
            foreach (float f in flagTable.FlagDataBySettings[setting + offset - 1].FlagTable)
            {
                //�e�����Ƃɒ��I
                flagCheckNum += Mathf.RoundToInt((float)MaxFlagLots / f);

                if (flag < flagCheckNum)
                {
                    return lotResult[index];
                }
                index += 1;
            }

            // ����������Ȃ����"�͂���"��Ԃ�
            return FlagId.FlagNone;
        }

        // BONUS GAME���̒��I
        private FlagId BonusGameLots(float nonePoss)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I�B�[���؎̂�)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // �͂��ꒊ�I
            flagCheckNum = Mathf.FloorToInt(MaxFlagLots / nonePoss);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // ����������Ȃ����"JAC��"��Ԃ�
            return FlagId.FlagJac;
        }

        // �t���O���I
        private int GetFlag()
        {
            // 16384�t���O�𓾂�(0~16383)
            int flag = Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);
            return flag;
        }
    }
}

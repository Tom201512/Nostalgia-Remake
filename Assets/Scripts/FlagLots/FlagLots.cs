using ReelSpinGame_Datass;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots : MonoBehaviour
    {
        // �t���O���I

        // const
        // �ő�t���O��
        const int MaxFlagLots = 16384;

        // enum
        // �t���OID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJACin, FlagJAC }
        // �t���O�e�[�u��
        public enum FlagLotMode { NormalA, NormalB, BigBonus, JacGame };

        // var
        // �t���O�f�[�^�x�[�X
        [SerializeField] FlagDatabase flagDatabase;
        // ���݃t���O(�v���p�e�B)
        public FlagId CurrentFlag { get; private set; } = FlagId.FlagNone;
        // �Q�Ƃ���e�[�u��ID
        public FlagLotMode CurrentTable { get; private set; } = FlagLotMode.NormalA;

        // ���I����(�ŏI�I�ɓ��I�����t���O���Q�Ƃ���̂Ɏg��)
        private FlagId[] lotResultNormal = new FlagId[] 
        {
            FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // BIG CHANCE��
        private FlagId[] lotResultBig = new FlagId[] 
        {
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // func
        // �e�[�u���ύX
        public void ChangeTable(FlagLotMode mode)
        {
            Debug.Log("Changed mode:" + mode);
            CurrentTable = mode;
        }

        // �t���O���I�̊J�n
        public void GetFlagLots(int setting)
        {
            // ���݂̎Q�ƃe�[�u�������Ƃɒ��I
            switch (CurrentTable)
            {
                case FlagLotMode.NormalA:
                    CurrentFlag = CheckResultByTable(setting, flagDatabase.NormalATable, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    CurrentFlag = CheckResultByTable(setting, flagDatabase.NormalBTable, lotResultNormal);
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
                flagCheckNum += Mathf.FloorToInt((float)MaxFlagLots / f);

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
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / nonePoss);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // ����������Ȃ����"JAC��"��Ԃ�
            return FlagId.FlagJAC;
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

using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
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
        // ���݃t���O(�v���p�e�B)
        public FlagId CurrentFlag { get; private set; } = FlagId.FlagNone;
        // �Q�Ƃ���e�[�u��ID
        public FlagLotMode CurrentTable { get; private set; } = FlagLotMode.NormalA;
        // �e�[�u�������l
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;
        // JAC GAME���͂���
        private float jacNoneProb;

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

        // �R���X�g���N�^
        public FlagLots(int setting, StreamReader tableAData,
            StreamReader tableBData, StreamReader tableBIGData, int jacNoneProb)
        {
            // �ݒ�l�����ƂɃe�[�u���쐬
            Debug.Log("Lots Setting set by :" + setting);

            // �ݒ�l�����ƂɃf�[�^�𓾂�(�ݒ�l�̗�܂œǂݍ���)
            for (int i = 0; i < setting - 1; i++)
            {
                tableAData.ReadLine();
                tableBData.ReadLine();
                tableBIGData.ReadLine();
            }

            // �f�[�^�ǂݍ���
            string[] valueA = tableAData.ReadLine().Split(',');
            string[] valueB = tableBData.ReadLine().Split(',');
            string[] valueBIG = tableBIGData.ReadLine().Split(',');

            // �ǂݍ��񂾃e�[�u����float�z��ɕϊ�
            flagLotsTableA = Array.ConvertAll(valueA, float.Parse);
            flagLotsTableB = Array.ConvertAll(valueB, float.Parse);
            flagLotsTableBIG = Array.ConvertAll(valueBIG, float.Parse);

            // JAC�͂���̐ݒ�
            this.jacNoneProb = jacNoneProb;

            Debug.Log("NormalA Table:");
            for (int i = 0; i < lotResultNormal.Length; i++)
            {
                Debug.Log(lotResultNormal[i].ToString() + ":" + flagLotsTableA[i]);
            }

            Debug.Log("NormalB Table:");
            for (int i = 0; i < lotResultNormal.Length; i++)
            {
                Debug.Log(lotResultNormal[i].ToString() + ":" + flagLotsTableB[i]);
            }

            Debug.Log("BIG Table:");
            for (int i = 0; i < lotResultBig.Length; i++)
            {
                Debug.Log(lotResultBig[i].ToString() + ":" + flagLotsTableBIG[i]);
            }

            Debug.Log("JAC None Probability:" + this.jacNoneProb);
        }

        // func
        // �e�[�u���ύX
        public void ChangeTable(FlagLotMode mode)
        {
            Debug.Log("Changed mode:" + mode);
            CurrentTable = mode;
        }

        // �t���O���I�̊J�n
        public void GetFlagLots()
        {
            // ���݂̎Q�ƃe�[�u�������Ƃɒ��I
            switch (CurrentTable)
            {
                case FlagLotMode.NormalA:
                    CurrentFlag = CheckResultByTable(flagLotsTableA, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    CurrentFlag = CheckResultByTable(flagLotsTableB, lotResultNormal);
                    break;

                case FlagLotMode.BigBonus:
                    CurrentFlag = CheckResultByTable(flagLotsTableBIG, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    CurrentFlag = BonusGameLots();
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + CurrentFlag);
        }

        // �e�[�u������t���O����
        private FlagId CheckResultByTable(float[] lotsTable, FlagId[] lotResult)
        {
            // 16384�t���O�𓾂�
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;


            for (int i = 0; i < lotsTable.Length; i++)
            {
                //�e�����Ƃɒ��I
                flagCheckNum += Mathf.FloorToInt((float)MaxFlagLots / lotsTable[i]);

                if (flag < flagCheckNum)
                {
                    return lotResult[i];
                }
            }
            // ����������Ȃ����"�͂���"��Ԃ�
            return FlagId.FlagNone;
        }

        // BONUS GAME���̒��I
        private FlagId BonusGameLots()
        {
            // 16384�t���O�𓾂�(0~16383)
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I�B�[���؎̂�)
            int flagCheckNum = 0;

            // �͂��ꒊ�I
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / jacNoneProb);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // ����������Ȃ����"JAC��"��Ԃ�
            return FlagId.FlagJAC;
        }
    }
}

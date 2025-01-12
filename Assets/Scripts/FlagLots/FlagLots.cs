using ReelSpinGame_Lots.FlagProb;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // �t���O���I

        // const

        // �ő�t���O��
        const int MaxFlagLots = 16384;

        //�����J�E���^�����l
        //const int CounterIncrease = 256;

        //�����J�E���^�����l
        //const int CounterDecrease1to4 = 100;

        //const int CounterDecrease5 = 104;

        //const int CounterDecrease6 = 1808;


        // enum

        // �t���OID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJACin, FlagJAC }

        // �t���O�e�[�u��
        public enum FlagLotMode { NormalA, NormalB, BigBonus, JacGame };

        // var

        // ��ݒ�
        private int lotsSetting;

        // ���݃t���O
        private FlagId currentFlag = FlagId.FlagNone;

        // �Q�Ƃ���e�[�u��ID
        private FlagLotMode currentTable = FlagLotMode.NormalB;


        // �����J�E���^
        private int flagCounter = 0;

        // �e�[�u�������l
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;

        // ���I����(�ŏI�I�ɓ��I�����t���O���Q�Ƃ���̂Ɏg��)
        private FlagId[] lotResultNormal = new FlagId[] {FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin};

        // BIG CHANCE��
        private FlagId[] lotResultBig = new FlagId[] {FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin};

        // �R���X�g���N�^
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting, int flagCounter)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // �ݒ�l�����ƂɃe�[�u���쐬
            this.lotsSetting = lotsSetting;

            Debug.Log("Lots Setting set by :" + lotsSetting);

            MakeTables();

            this.flagCounter = flagCounter;
        }


        // func

        // �e�[�u���쐬(��������)
        private void MakeTables()
        {
            flagLotsTableA = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbA,
                    FlagLotsProb.MelonProbA,
                    FlagLotsProb.BellProbA,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableB = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbB,
                    FlagLotsProb.MelonProbB,
                    FlagLotsProb.BellProbB,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableBIG = new float[] {
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.MelonProbInBig,
                    FlagLotsProb.BigBellProbability[lotsSetting - 1],
                    FlagLotsProb.JACinProbInBig};

            Debug.Log("NormalA Table:");
            for(int i = 0; i < lotResultNormal.Length; i++)
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
        }

        // �t���O���I�̊J�n
        public void GetFlagLots()
        {
            // ���݂̎Q�ƃe�[�u�������Ƃɒ��I

            switch(currentTable)
            {
                case FlagLotMode.NormalA:
                    currentFlag = CheckResultByTable(flagLotsTableA, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    currentFlag = CheckResultByTable(flagLotsTableB, lotResultNormal);
                    break;

                case FlagLotMode.BigBonus:
                    currentFlag = CheckResultByTable(flagLotsTableBIG, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    currentFlag = BonusGameLots();
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + currentFlag);
        }

        // BONUS GAME���̒��I
        private FlagId BonusGameLots()
        {
            // 16384�t���O�𓾂�
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;

            // �͂��ꒊ�I
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / FlagLotsProb.JAC_NONE_PROB);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // ����������Ȃ���΁@JAC���@��Ԃ�
            return FlagId.FlagJAC;
        }

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

            return FlagId.FlagNone;
        }
    }
}

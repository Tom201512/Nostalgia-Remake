using ReelSpinGame_Lots.FlagProb;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // �t���O���I

        // const

        // �ő�t���O��
        const int MAX_FLAG_LOTS = 16384;

        // enum

        // �t���OID
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY_JACIN, FLAG_JAC }

        // �t���O�e�[�u��
        public enum FLAG_LOT_MODE { LOT_NORMAL_A, LOT_NORMAL_B, LOT_BIGBONUS, LOT_JACGAME };

        // var

        // ��ݒ�
        private int lotsSetting;

        // ���݃t���O
        private FLAG_ID currentFlag = FLAG_ID.FLAG_NONE;

        // �Q�Ƃ���e�[�u��ID
        private FLAG_LOT_MODE currentTable = FLAG_LOT_MODE.LOT_NORMAL_A;

        // �e�[�u�������l
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;

        // ���I����(�ŏI�I�ɓ��I�����t���O���Q�Ƃ���̂Ɏg��)
        private FLAG_ID[] lotResultNormal = new FLAG_ID[] {FLAG_ID.FLAG_BIG,
            FLAG_ID.FLAG_REG,
            FLAG_ID.FLAG_CHERRY2,
            FLAG_ID.FLAG_CHERRY4,
            FLAG_ID.FLAG_MELON,
            FLAG_ID.FLAG_BELL,
            FLAG_ID.FLAG_REPLAY_JACIN};

        // BIG CHANCE��
        private FLAG_ID[] lotResultBig = new FLAG_ID[] {FLAG_ID.FLAG_CHERRY2,
            FLAG_ID.FLAG_CHERRY4,
            FLAG_ID.FLAG_MELON,
            FLAG_ID.FLAG_BELL,
            FLAG_ID.FLAG_REPLAY_JACIN};



        // �R���X�g���N�^
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // �ݒ�l�����ƂɃe�[�u���쐬
            this.lotsSetting = lotsSetting;

            Debug.Log("Lots Setting set by :" + lotsSetting);

            MakeTables();
        }


        // func

        private void MakeTables()
        {
            flagLotsTableA = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.CHERRY2_PROB,
                    FlagLotsProb.CHERRY4_PROB_A,
                    FlagLotsProb.MELON_PROB_A,
                    FlagLotsProb.BELL_PROB_A,
                    FlagLotsProb.REPLAY_PROB};

            flagLotsTableB = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.CHERRY2_PROB,
                    FlagLotsProb.CHERRY4_PROB_B,
                    FlagLotsProb.MELON_PROB_B,
                    FlagLotsProb.BELL_PROB_B,
                    FlagLotsProb.REPLAY_PROB};

            flagLotsTableBIG = new float[] {
                    FlagLotsProb.BIG_CHERRY_PROB,
                    FlagLotsProb.BIG_CHERRY_PROB,
                    FlagLotsProb.BIG_MELON_PROB,
                    FlagLotsProb.BigBellProbability[lotsSetting - 1],
                    FlagLotsProb.BIG_JACIN_PROB};

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

        public void GetFlagLots()
        {
            // 16384�t���O�𓾂�
            int flag = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flag);

            // ���݂̎Q�ƃe�[�u�������Ƃɒ��I

            switch(currentTable)
            {
                case FLAG_LOT_MODE.LOT_NORMAL_A:
                case FLAG_LOT_MODE.LOT_NORMAL_B:
                    currentFlag = LotsFlags(flag);
                    break;

                case FLAG_LOT_MODE.LOT_BIGBONUS:
                    currentFlag = BigChanceLots(flag);
                    break;

                case FLAG_LOT_MODE.LOT_JACGAME:
                    currentFlag = BonusGameLots(flag);
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + currentFlag);
        }

        private FLAG_ID LotsFlags(int _flag)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;

            // �����J�E���^�����m���Ȃ�
            if(currentTable == FLAG_LOT_MODE.LOT_NORMAL_B)
            {
                for (int i = 0; i < lotResultNormal.Length; i++)
                {
                    //�e�����Ƃɒ��I
                    flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableB[i]);

                    if (_flag < flagCheckNum)
                    {
                        return lotResultNormal[i];
                    }
                }
            }

            // �����J�E���^����m���Ȃ�
            else
            {
                for (int i = 0; i < lotResultNormal.Length; i++)
                {
                    //�e�����Ƃɒ��I
                    flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableB[i]);

                    if (_flag < flagCheckNum)
                    {
                        return lotResultNormal[i];
                    }
                }
            }

            // ����������Ȃ���΁@�͂���@��Ԃ�
            return FLAG_ID.FLAG_NONE;
        }

        private FLAG_ID BigChanceLots(int _flag)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;

            for (int i = 0; i < lotResultBig.Length; i++)
            {
                //�e�����Ƃɒ��I
                flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableBIG[i]);

                if (_flag < flagCheckNum)
                {
                    return lotResultBig[i];
                }
            }

            // ����������Ȃ���΁@�͂���@��Ԃ�
            return FLAG_ID.FLAG_NONE;
        }

        private FLAG_ID BonusGameLots(int _flag)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;

            // �͂��ꒊ�I
            flagCheckNum = Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.JAC_NONE_PROB);
            if (_flag < flagCheckNum)
            {
                return FLAG_ID.FLAG_NONE;
            }

            // ����������Ȃ���΁@JAC���@��Ԃ�
            return FLAG_ID.FLAG_JAC;
        }
    }
}

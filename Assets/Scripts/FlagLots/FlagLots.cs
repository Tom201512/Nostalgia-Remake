using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReelSpinGame_Lots.FlagProb;

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
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY, FLAG_JAC }

        // �t���O�e�[�u��
        public enum FLAG_LOT_MODE { LOT_NORMAL_A, LOT_NORMAL_B, LOT_BIGBONUS, LOT_JACGAME };


        // var

        // ��ݒ�
        private int lotsSetting;

        // ���݃t���O
        private FLAG_ID currentFlag = FLAG_ID.FLAG_NONE;

        // �Q�Ƃ���e�[�u��ID
        private FLAG_LOT_MODE currentTable = FLAG_LOT_MODE.LOT_NORMAL_A;

        //�e�[�u�������l
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;


        // �R���X�g���N�^
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // �ݒ�l�����ƂɃe�[�u���쐬
            this.lotsSetting = lotsSetting;
        }

        
        // func

        public void GetFlagLots()
        {
            // 16384�t���O�𓾂�
            int flag = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flag);

            // ���݂̎Q�ƃe�[�u�������Ƃɒ��I
            currentFlag = LotsFlags(flag);
            Debug.Log("Flag:" + currentFlag);
        }

        private FLAG_ID LotsFlags(int _flag)
        {
            // ����p�̐��l(16384/�����m���ŋ��߁A�����菭�Ȃ��t���O���������瓖�I)
            int flagCheckNum = 0;

            
            // BIG���I
            flagCheckNum = Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.BigProbability[0]);
            if(_flag < flagCheckNum)
            {
                Debug.Log("BIG hits");
                return FLAG_ID.FLAG_BIG;
            }

            // REG���I
            flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.RegProbability[0]);
            if (_flag < flagCheckNum)
            {
                Debug.Log("REG hits");
                return FLAG_ID.FLAG_REG;
            }

            //��������2���`�F���[�Ȃǒ��I

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

            return FLAG_ID.FLAG_JAC;
        }
    }
}

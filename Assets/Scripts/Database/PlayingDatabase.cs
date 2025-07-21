using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using System.Collections.Generic;
using static ReelSpinGame_Bonus.BonusBehaviour;
using UnityEngine;
using System.IO;
using System;

namespace ReelSpinGame_System
{
    public class PlayingDatabase : ISavable
    {
        // �v���C�����

        // const

        // �L�^�\�Q�[����
        public const int MaximumTotalGames = 99999;

        // var
        // �Q�[�������
        // ���Q�[����(�{�[�i�X������)
        public int TotalGames { get; private set; }
        // �{�[�i�X�ԃQ�[����
        public int CurrentGames { get; private set; }

        // ���_�����
        public PlayerMedalData PlayerMedalData { get; private set; }

        // ���I�������{�[�i�X(ID���Ƃ�)
        public List<BonusHitData> BonusHitDatas { get; private set; }

        // �r�b�O�`�����X������
        public int BigTimes { get; private set; }
        // �{�[�i�X�Q�[��������
        public int RegTimes { get; private set; }

        // �R���X�g���N�^
        public PlayingDatabase()
        {
            TotalGames = 0;
            CurrentGames = 0;
            PlayerMedalData = new PlayerMedalData();
            BonusHitDatas = new List<BonusHitData>();
            BigTimes = 0;
            RegTimes = 0;
        }

        // func

        // �e��f�[�^���l�ύX

        // �Q�[�����𑝂₷
        public void IncreaseGameValue()
        {
            TotalGames += 1;
            CurrentGames += 1;
        }
        // �{�[�i�X�ԃQ�[�������Z�b�g
        public void ResetCurrentGame() => CurrentGames = 0;

        // �{�[�i�X����ǉ�(�������Ɏg�p)
        public void AddBonusResult(BonusType bonusType)
        {
            BonusHitData buffer = new BonusHitData();
            BonusHitDatas.Add(buffer);
            BonusHitDatas[^1].SetBonusType(bonusType);
            BonusHitDatas[^1].SetBonusHitGame(CurrentGames);
        }

        // ���߂̃{�[�i�X�����̓��܂��L�^
        public void SetLastBonusStart() => BonusHitDatas[^1].SetBonusStartGame(CurrentGames);
        // ���߂̃r�b�O�`�����X���̐F���L�^
        public void SetLastBigChanceColor(BigColor color) => BonusHitDatas[^1].SetBigChanceColor(color);
        // ���݂̃{�[�i�X�����ɕ����o����ǉ�����
        public void ChangeLastBonusPayouts(int payouts) => BonusHitDatas[^1].ChangeBonusPayouts(payouts);

        // �r�b�O�`�����X�񐔂̑���
        public void IncreaseBigChance() => BigTimes += 1;
        // �{�[�i�X�Q�[���񐔂̑���
        public void IncreaseBonusGame() => RegTimes += 1;

        // �Z�[�u��������
        public List<int> SaveData()
        {
            // �ϐ��f�[�^�����ׂĊi�[
            List<int> data = new List<int>();

            data.Add(TotalGames);
            data.Add(CurrentGames);
            
            // ���_�����
            foreach(int list in PlayerMedalData.SaveData())
            {
                data.Add(list);
            }

            // �{�[�i�X���̐�
            data.Add(BonusHitDatas.Count);

            // �{�[�i�X���
            for(int i = 0; i < BonusHitDatas.Count; i++)
            {
                foreach (int list in BonusHitDatas[i].SaveData())
                {
                    data.Add(list);
                }
            }

            // BIG/REG��
            data.Add(BigTimes);
            data.Add(RegTimes);

            // �f�o�b�O�p
            Debug.Log("PlayerData:");
            foreach(int i in data)
            {
                Debug.Log(i);
            }

            return data;
        }

        // �Z�[�u�ǂݍ���
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // �Q�[�����ǂݍ���
                TotalGames = bStream.ReadInt32();
                Debug.Log("TotalGames:" + TotalGames);

                CurrentGames = bStream.ReadInt32();
                Debug.Log("CurrentGames:" + CurrentGames);

                // ���_�����ǂݍ���
                PlayerMedalData.LoadData(bStream);

                // �{�[�i�X����ǂݍ���
                // �{�[�i�X����

                int bonusResultCounts = bStream.ReadInt32();
                Debug.Log("BonusResultCounts:" + bonusResultCounts);

                // ���𕪓ǂݍ���
                for (int i = 0; i < bonusResultCounts; i++)
                {
                    BonusHitData buffer = new BonusHitData();
                    BonusHitDatas.Add(buffer);
                    buffer.LoadData(bStream);
                }

                Debug.Log("BonusLoad END");

                // BIG��
                BigTimes = bStream.ReadInt32();
                Debug.Log("BigTimes:" + BigTimes);

                // REG��
                RegTimes = bStream.ReadInt32();
                Debug.Log("RegTimes:" + RegTimes);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("PlayerData Read is done");
            }

            return true;
        }
    }
}

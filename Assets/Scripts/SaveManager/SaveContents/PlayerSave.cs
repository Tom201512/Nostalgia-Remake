using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_System;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ReelSpinGame_Save.Player
{
    namespace ReelSpinGame_System
    {
        public class PlayerSave : ISavable
        {
            // �v���C���[�Z�[�u�f�[�^

            // var
            // �Q�[�������
            // ���Q�[����(�{�[�i�X������)
            public int TotalGames { get; private set; }
            // �{�[�i�X�ԃQ�[����
            public int CurrentGames { get; private set; }

            // ���_�����
            public PlayerMedalData PlayerMedalData { get; private set; }

            // ���I�������{�[�i�X(ID���Ƃ�)
            public List<BonusHitData> BonusHitRecord { get; private set; }

            // �r�b�O�`�����X������
            public int BigTimes { get; private set; }
            // �{�[�i�X�Q�[��������
            public int RegTimes { get; private set; }

            // �R���X�g���N�^
            public PlayerSave()
            {
                TotalGames = 0;
                CurrentGames = 0;
                PlayerMedalData = new PlayerMedalData();
                BonusHitRecord = new List<BonusHitData>();
                BigTimes = 0;
                RegTimes = 0;
            }

            // func

            // �f�[�^�L�^
            public void RecordData(PlayerDatabase playerData)
            {
                TotalGames = playerData.TotalGames;
                CurrentGames = playerData.CurrentGames;
                PlayerMedalData = playerData.PlayerMedalData;
                BonusHitRecord = playerData.BonusHitRecord;
                BigTimes = playerData.BigTimes;
                RegTimes = playerData.RegTimes;
            }

            // �Z�[�u
            public List<int> SaveData()
            {
                // �ϐ��f�[�^�����ׂĊi�[
                List<int> data = new List<int>();

                data.Add(TotalGames);
                data.Add(CurrentGames);

                // ���_�����
                foreach (int list in PlayerMedalData.SaveData())
                {
                    data.Add(list);
                }

                // �{�[�i�X���̐�
                data.Add(BonusHitRecord.Count);

                // �{�[�i�X���
                for (int i = 0; i < BonusHitRecord.Count; i++)
                {
                    foreach (int list in BonusHitRecord[i].SaveData())
                    {
                        data.Add(list);
                    }
                }

                // BIG/REG��
                data.Add(BigTimes);
                data.Add(RegTimes);

                return data;
            }

            // �Z�[�u�ǂݍ���
            public bool LoadData(BinaryReader br)
            {
                try
                {
                    // �Q�[�����ǂݍ���
                    TotalGames = br.ReadInt32();
                    //Debug.Log("TotalGames:" + TotalGames);

                    CurrentGames = br.ReadInt32();
                    //Debug.Log("CurrentGames:" + CurrentGames);

                    // ���_�����ǂݍ���
                    PlayerMedalData.LoadData(br);

                    // �{�[�i�X����ǂݍ���
                    // �{�[�i�X����

                    int bonusResultCounts = br.ReadInt32();
                    //Debug.Log("BonusResultCounts:" + bonusResultCounts);

                    // ���𕪓ǂݍ���
                    for (int i = 0; i < bonusResultCounts; i++)
                    {
                        BonusHitData buffer = new BonusHitData();
                        BonusHitRecord.Add(buffer);
                        buffer.LoadData(br);
                    }

                    //Debug.Log("BonusLoad END");

                    // BIG��
                    BigTimes = br.ReadInt32();
                    //Debug.Log("BigTimes:" + BigTimes);

                    // REG��
                    RegTimes = br.ReadInt32();
                    //Debug.Log("RegTimes:" + RegTimes);
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
                finally
                {
                    //Debug.Log("PlayerData Read is done");
                }

                return true;
            }
        }
    }

}


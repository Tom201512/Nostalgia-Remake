using ReelSpinGame_Datas;
using ReelSpinGame_Datas.Analytics;
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

            // �r�b�O�`�����X������
            public int BigTimes { get; private set; }
            // �{�[�i�X�Q�[��������
            public int RegTimes { get; private set; }

            // ���_�����
            public PlayerMedalData PlayerMedalData { get; private set; }
            // ���I�������{�[�i�X(ID���Ƃ�)
            public List<BonusHitData> BonusHitRecord { get; private set; }
            // ��̓f�[�^
            public AnalyticsData PlayerAnalyticsData { get; private set; }

            // �R���X�g���N�^
            public PlayerSave()
            {
                TotalGames = 0;
                CurrentGames = 0;
                BigTimes = 0;
                RegTimes = 0;
                PlayerMedalData = new PlayerMedalData();
                BonusHitRecord = new List<BonusHitData>();
                PlayerAnalyticsData = new AnalyticsData();
            }

            // func

            // �f�[�^�L�^
            public void RecordData(PlayerDatabase playerData)
            {
                TotalGames = playerData.TotalGames;
                CurrentGames = playerData.CurrentGames;
                BigTimes = playerData.BigTimes;
                RegTimes = playerData.RegTimes;
                PlayerMedalData = playerData.PlayerMedalData;
                BonusHitRecord = playerData.BonusHitRecord;
                PlayerAnalyticsData = playerData.PlayerAnalyticsData;
            }

            // �Z�[�u
            public List<int> SaveData()
            {
                // �ϐ��f�[�^�����ׂĊi�[
                List<int> data = new List<int>();

                data.Add(TotalGames);
                Debug.Log("TotalGames:" + TotalGames);
                data.Add(CurrentGames);
                Debug.Log("CurrentGames:" + CurrentGames);

                // BIG/REG��
                data.Add(BigTimes);
                Debug.Log("BigTimes:" + BigTimes);
                data.Add(RegTimes);
                Debug.Log("RegTimes:" + RegTimes);

                // ���_�����
                foreach (int list in PlayerMedalData.SaveData())
                {
                    data.Add(list);
                }

                // �{�[�i�X���̐�
                data.Add(BonusHitRecord.Count);
                Debug.Log("BonusData count :" + BonusHitRecord.Count);

                // �{�[�i�X���
                for (int i = 0; i < BonusHitRecord.Count; i++)
                {
                    foreach (int list in BonusHitRecord[i].SaveData())
                    {
                        data.Add(list);
                    }
                }

                // ��͏��
                foreach(int list in PlayerAnalyticsData.SaveData())
                {
                    data.Add(list);
                }

                return data;
            }

            // �Z�[�u�ǂݍ���
            public bool LoadData(BinaryReader br)
            {
                try
                {
                    // �Q�[�����ǂݍ���
                    TotalGames = br.ReadInt32();
                    Debug.Log("TotalGames:" + TotalGames);

                    CurrentGames = br.ReadInt32();
                    Debug.Log("CurrentGames:" + CurrentGames);

                    // BIG��
                    BigTimes = br.ReadInt32();
                    Debug.Log("BigTimes:" + BigTimes);

                    // REG��
                    RegTimes = br.ReadInt32();
                    Debug.Log("RegTimes:" + RegTimes);

                    // ���_�����ǂݍ���
                    PlayerMedalData.LoadData(br);

                    // �{�[�i�X����ǂݍ���
                    // �{�[�i�X����

                    int bonusResultCount = br.ReadInt32();
                    Debug.Log("BonusResultCount:" + bonusResultCount);

                    // ���𕪓ǂݍ���
                    for (int i = 0; i < bonusResultCount; i++)
                    {
                        Debug.Log("BonusResult[" + i + "]:");
                        BonusHitData buffer = new BonusHitData();
                        buffer.LoadData(br);
                        BonusHitRecord.Add(buffer);
                    }

                    // ��͏��ǂݍ���
                    PlayerAnalyticsData.LoadData(br);

                    Debug.Log("PlyaerLoad END");

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
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


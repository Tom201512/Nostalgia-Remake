using ReelSpinGame_Datas;
using ReelSpinGame_Interface;
using ReelSpinGame_Save.Player.ReelSpinGame_System;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_System
{
    public class PlayerDatabase : IHasSave
    {
        // �v���C���[���

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
        //public List<BonusHitData> BonusHitRecord { get; private set; }

        // �r�b�O�`�����X������
        public int BigTimes { get; private set; }
        // �{�[�i�X�Q�[��������
        public int RegTimes { get; private set; }

        // �R���X�g���N�^
        public PlayerDatabase()
        {
            TotalGames = 0;
            CurrentGames = 0;
            PlayerMedalData = new PlayerMedalData();
            //BonusHitRecord = new List<BonusHitData>();
            BigTimes = 0;
            RegTimes = 0;
        }

        // func
        // �Z�[�u�f�[�^�ɂ���
        public ISavable MakeSaveData()
        {
            PlayerSave save = new PlayerSave();
            save.RecordData(this);

            return save;
        }

        // �Z�[�u��ǂݍ���
        public void LoadSaveData(ISavable loadData)
        {
            if (loadData.GetType() == typeof(PlayerSave))
            {
                PlayerSave save = new PlayerSave();
                save = loadData as PlayerSave;

                TotalGames = save.TotalGames;
                CurrentGames = save.CurrentGames;
                PlayerMedalData.SetData(save.PlayerMedalData);
                //SetBonusRecord(save);
                BigTimes = save.BigTimes;
                RegTimes = save.RegTimes;
            }
            else
            {
                throw new Exception("Loaded data is not PlayerData");
            }
        }

        // �e��f�[�^���l�ύX

        // �Q�[�����𑝂₷
        public void IncreaseGameValue()
        {
            TotalGames += 1;
            CurrentGames += 1;
        }
        // �{�[�i�X�ԃQ�[�������Z�b�g
        public void ResetCurrentGame() => CurrentGames = 0;
        // �r�b�O�`�����X�񐔂̑���
        public void IncreaseBigChance() => BigTimes += 1;
        // �{�[�i�X�Q�[���񐔂̑���
        public void IncreaseBonusGame() => RegTimes += 1;

        /*
        // �{�[�i�X����ǉ�(�������Ɏg�p)
        public void AddBonusResult(BonusTypeID bonusType)
        {
            BonusHitData buffer = new BonusHitData();
            BonusHitRecord.Add(buffer);
            BonusHitRecord[^1].SetBonusType(bonusType);
            BonusHitRecord[^1].SetBonusHitGame(CurrentGames);
        }

        // ���߂̃{�[�i�X�����̓��܂��L�^
        public void SetLastBonusStart() => BonusHitRecord[^1].SetBonusStartGame(CurrentGames);
        // ���߂̃r�b�O�`�����X���̐F���L�^
        public void SetLastBigChanceColor(BigColor color) => BonusHitRecord[^1].SetBigChanceColor(color);
        // ���݂̃{�[�i�X�����ɕ����o����ǉ�����
        public void ChangeLastBonusPayout(int payout) => BonusHitRecord[^1].ChangeBonusPayout(payout);*/

        // �{�[�i�X��������ǂݍ���

        /*
        private void SetBonusRecord(PlayerSave save)
        {
            foreach(BonusHitData b in save.BonusHitRecord)
            {
                BonusHitRecord.Add(b);
            }
        }*/
    }
}

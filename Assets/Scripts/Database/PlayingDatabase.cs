using ReelSpinGame_Bonus;
using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_System
{
    public class PlayingDatabase
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
        public MedalData PlayerMedalData { get; private set; }

        // ���I�������{�[�i�X(ID���Ƃ�)
        public List<BonusHitData> BonusHitDatas { get; private set; }

        // �r�b�O�`�����X������
        public int BigTimes { get; private set; }
        // �{�[�i�X�Q�[��������
        public int RegTimes { get; private set; }

        // �R���X�g���N�^
        public PlayingDatabase()
        {
            PlayerMedalData = new MedalData();
            BonusHitDatas = new List<BonusHitData>();
        }

        // �Z�[�u�f�[�^����ǂݍ��ޏꍇ
        public PlayingDatabase(MedalData savedMedalData, List<BonusHitData> savedHitDatas) : this()
        {
            PlayerMedalData = savedMedalData;
            BonusHitDatas = savedHitDatas;
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
        public void AddBonusResult(BonusManager.BonusType bonusType)
        {
            Debug.Log("BonusHit");
            BonusHitData test = new BonusHitData(bonusType);
            BonusHitDatas.Add(test);
            BonusHitDatas[^1].SetBonusHitGame(CurrentGames);
        }

        // ���߂̃{�[�i�X�����̓��܂��L�^
        public void SetLastBonusStart()
        {
            BonusHitDatas[^1].SetBonusStartGame(CurrentGames);
        }

        // ���݂̃{�[�i�X�����ɕ����o����ǉ�����
        public void ChangeBonusPayoutToLast(int payouts) => BonusHitDatas[~1].ChangeBonusPayouts(payouts);

        // �r�b�O�`�����X�񐔂̑���
        public void IncreaseBigChance() => BigTimes += 1;
        // �{�[�i�X�Q�[���񐔂̑���
        public void IncreaseBonusGame() => RegTimes += 1;
    }
}

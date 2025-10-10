using ReelSpinGame_Interface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Datas.Analytics
{

    // ��͏��
    public class AnalyticsData : ISavable
    {
        // var
        // �����AJAC�A�ʏ펞���܂߂��S�Q�[����]��
        public int TotalAllGamesCount { get; private set; }
        // BIG�����Q�[�����̃Q�[����
        public int BigGamesCount { get; private set; }
        // JAC�Q�[�����̃Q�[����
        public int JacGamesCount { get; private set; }

        // �ʏ펞�e�����̓��܉�(���v���C������)
        // �����Ɠ��܂͕ʁX�ɃJ�E���g����

        // ����
        // �x��
        public int NormalBellHitCount { get; private set; }
        // �X�C�J
        public int NormalMelonHitCount { get; private set; }
        // 2���`�F���[
        public int NormalCherry2HitCount { get; private set; }
        // 4���`�F���[
        public int NormalCherry4HitCount { get; private set; }

        // �x��
        public int NormalBellLineUpCount { get; private set; }
        // �X�C�J
        public int NormalMelonLineUpCount { get; private set; }
        // 2���`�F���[
        public int NormalCherry2LineUpCount { get; private set; }
        // 4���`�F���[
        public int NormalCherry4LineUpCount { get; private set; }

        // BIG������
        // �x��
        public int BigBellHitCount { get; private set; }
        // �X�C�J
        public int BigMelonHitCount { get; private set; }
        // 2���`�F���[
        public int BigCherry2HitCount { get; private set; }
        // 4���`�F���[
        public int BigCherry4HitCount { get; private set; }

        // BIG������
        // �x��
        public int BigBellLineUpCount { get; private set; }
        // �X�C�J
        public int BigMelonLineUpCount { get; private set; }
        // 2���`�F���[
        public int BigCherry2LineUpCount { get; private set; }
        // 4���`�F���[
        public int BigCherry4LineUpCount { get; private set; }

        // �r�^�������x�p
        // JAC-IN������
        public int BigJacInTimes { get; private set; }
        // JAC�͂���������
        public int BigJacAvoidTimes { get; private set; }
        // JAC�r�^�͂���������
        public int BigJacPerfectAvoidTimes { get; private set; }
        // �A�V�X�g�͂���������
        public int BigJacAssistedAvoidTimes { get; private set; }

        // JAC���͂����
        public int JacGameNoneTimes { get; private set; }

        // �R���X�g���N�^
        public AnalyticsData()
        {
            TotalAllGamesCount = 0;
            BigGamesCount = 0;
            JacGamesCount = 0;

            NormalBellHitCount = 0;
            NormalMelonHitCount = 0;
            NormalCherry2HitCount = 0;
            NormalCherry4HitCount = 0;

            NormalBellLineUpCount = 0;
            NormalMelonLineUpCount = 0;
            NormalCherry2LineUpCount = 0;
            NormalCherry4LineUpCount = 0;

            BigBellHitCount = 0;
            BigMelonHitCount = 0;
            BigCherry2HitCount = 0;
            BigCherry4HitCount = 0;


            BigBellLineUpCount = 0;
            BigMelonLineUpCount = 0;
            BigCherry2LineUpCount = 0;
            BigCherry4LineUpCount = 0;

            BigJacInTimes = 0;
            BigJacAvoidTimes = 0;
            BigJacPerfectAvoidTimes = 0;
            BigJacAssistedAvoidTimes = 0;

            JacGameNoneTimes = 0;
        }

        // func
        // �f�[�^����Z�b�g����
        public bool SetData(ISavable analyticsData)
        {
            if (analyticsData.GetType() == typeof(AnalyticsData))
            {
                AnalyticsData data = analyticsData as AnalyticsData;

                TotalAllGamesCount = 0;
                BigGamesCount = 0;
                JacGamesCount = 0;

                NormalBellHitCount = 0;
                NormalMelonHitCount = 0;
                NormalCherry2HitCount = 0;
                NormalCherry4HitCount = 0;

                NormalBellLineUpCount = 0;
                NormalMelonLineUpCount = 0;
                NormalCherry2LineUpCount = 0;
                NormalCherry4LineUpCount = 0;

                BigBellHitCount = 0;
                BigMelonHitCount = 0;
                BigCherry2HitCount = 0;
                BigCherry4HitCount = 0;

                BigBellLineUpCount = 0;
                BigMelonLineUpCount = 0;
                BigCherry2LineUpCount = 0;
                BigCherry4LineUpCount = 0;

                BigJacInTimes = 0;
                BigJacAvoidTimes = 0;
                BigJacPerfectAvoidTimes = 0;
                BigJacAssistedAvoidTimes = 0;

                JacGameNoneTimes = 0;

                return true;
            }
            else
            {
                Debug.LogError("Data is not AnalyticsData");
                return false;
            }
        }

        // �g�[�^���Q�[�������Z
        public void IncreaseTotalAllGameCounts(BonusStatus bonusStatus)
        {
            TotalAllGamesCount += 1;
            if(bonusStatus == BonusStatus.BonusBIGGames)
            {
                BigGamesCount += 1;
            }
            else if(bonusStatus == BonusStatus.BonusJACGames)
            {
                JacGamesCount += 1;
            }
        }

        // ���𐬗��J�E���g����
        public void IncreaseHitCountByFlag(FlagId flagID, BonusStatus bonusStatus)
        {
            if(bonusStatus == BonusStatus.BonusBIGGames)
            {
                switch (flagID)
                {
                    case FlagId.FlagCherry2:
                        BigCherry2HitCount += 1;
                        break;
                    case FlagId.FlagCherry4:
                        BigCherry4HitCount += 1;
                        break;
                    case FlagId.FlagMelon:
                        BigMelonHitCount += 1;
                        break;
                    case FlagId.FlagBell:
                        BigBellHitCount += 1;
                        break;

                    // BIG����JAC-IN�������܂߂�(���܂̓J�E���g���Ȃ�)
                    case FlagId.FlagReplayJacIn:
                        BigJacInTimes += 1;
                        break;
                }
            }
            else if(bonusStatus == BonusStatus.BonusJACGames)
            {
                if(flagID == FlagId.FlagNone)
                {
                    JacGameNoneTimes += 1;
                }
            }
            else
            {
                switch (flagID)
                {
                    case FlagId.FlagCherry2:
                        NormalCherry2HitCount += 1;
                        break;
                    case FlagId.FlagCherry4:
                        NormalCherry4HitCount += 1;
                        break;
                    case FlagId.FlagMelon:
                        NormalMelonHitCount += 1;
                        break;
                    case FlagId.FlagBell:
                        NormalBellHitCount += 1;
                        break;
                }
            }
        }

        // ������܃J�E���g����
        public void IncreaseLineUpCountByFlag(FlagId flagID, BonusStatus bonusStatus)
        {
            if (bonusStatus == BonusStatus.BonusBIGGames)
            {
                switch (flagID)
                {
                    case FlagId.FlagCherry2:
                        BigCherry2LineUpCount += 1;
                        break;
                    case FlagId.FlagCherry4:
                        BigCherry4LineUpCount += 1;
                        break;
                    case FlagId.FlagMelon:
                        BigMelonLineUpCount += 1;
                        break;
                    case FlagId.FlagBell:
                        BigBellLineUpCount += 1;
                        break;
                }
            }
            else if (bonusStatus == BonusStatus.BonusJACGames)
            {
                if (flagID == FlagId.FlagNone)
                {
                    JacGameNoneTimes += 1;
                }
            }
            else
            {
                switch (flagID)
                {
                    case FlagId.FlagCherry2:
                        NormalCherry2LineUpCount += 1;
                        break;
                    case FlagId.FlagCherry4:
                        NormalCherry4LineUpCount += 1;
                        break;
                    case FlagId.FlagMelon:
                        NormalMelonLineUpCount += 1;
                        break;
                    case FlagId.FlagBell:
                        NormalBellLineUpCount += 1;
                        break;
                }
            }
        }

        // �r�^�n�Y�V�L�^
        public void CountJacAvoidCounts(int leftPushedPos, int random)
        {
            // �r�^�����ł̔���
            if(leftPushedPos == 10 || leftPushedPos == 16)
            {
                BigJacAvoidTimes += 1;
                BigJacPerfectAvoidTimes += 1;
            }
            // �A�V�X�g����ŊO�������̃`�F�b�N(�����������ł��邱��)
            else if((leftPushedPos == 9 || leftPushedPos == 15) &&
                random % 2 == 0)
            {
                BigJacAvoidTimes += 1;
                BigJacAssistedAvoidTimes += 1;
            }
        }

        // �f�[�^��������
        public List<int> SaveData()
        {
            // �ϐ����i�[
            List<int> data = new List<int>();
            data.Add(TotalAllGamesCount);
            Debug.Log("TotalAllGamesCount" + TotalAllGamesCount);
            data.Add(BigGamesCount);
            Debug.Log("BigGamesCount" + BigGamesCount);
            data.Add(JacGamesCount);
            Debug.Log("JacGamesCount" + JacGamesCount);

            data.Add(NormalBellHitCount);
            Debug.Log("NormalBellHitCount" + NormalBellHitCount);
            data.Add(NormalMelonHitCount);
            Debug.Log("NormalMelonHitCount" + NormalMelonHitCount);
            data.Add(NormalCherry2HitCount);
            Debug.Log("NormalCherry2HitCount" + NormalCherry2HitCount);
            data.Add(NormalCherry4HitCount);
            Debug.Log("NormalCherry4HitCount" + NormalCherry4HitCount);

            data.Add(NormalBellLineUpCount);
            Debug.Log("NormalBellLineUpCount" + NormalBellLineUpCount);
            data.Add(NormalMelonLineUpCount);
            Debug.Log("NormalMelonLineUpCount" + NormalMelonLineUpCount);
            data.Add(NormalCherry2LineUpCount);
            Debug.Log("NormalCherry2LineUpCount" + NormalCherry2LineUpCount);
            data.Add(NormalCherry4LineUpCount);
            Debug.Log("NormalCherry4LineUpCount" + NormalCherry4LineUpCount);

            data.Add(BigBellHitCount);
            Debug.Log("BigBellHitCount" + BigBellHitCount);
            data.Add(BigMelonHitCount);
            Debug.Log("BigMelonHitCount" + BigMelonHitCount);
            data.Add(BigCherry2HitCount);
            Debug.Log("BigCherry2HitCount" + BigCherry2HitCount);
            data.Add(BigCherry4HitCount);
            Debug.Log("BigCherry4HitCount" + BigCherry4HitCount);

            data.Add(BigBellLineUpCount);
            Debug.Log("BigBellLineUpCount" + BigBellLineUpCount);
            data.Add(BigMelonLineUpCount);
            Debug.Log("BigMelonLineUpCount" + BigMelonLineUpCount);
            data.Add(BigCherry2LineUpCount);
            Debug.Log("BigCherry2LineUpCount" + BigCherry2LineUpCount);
            data.Add(BigCherry4LineUpCount);
            Debug.Log("BigCherry4LineUpCount" + BigCherry4LineUpCount);

            data.Add(BigJacInTimes);
            Debug.Log("BigJacInTimes" + BigJacInTimes);
            data.Add(BigJacAvoidTimes);
            Debug.Log("BigJacAvoidTimes" + BigJacAvoidTimes);
            data.Add(BigJacPerfectAvoidTimes);
            Debug.Log("BigJacPerfectAvoidTimes" + BigJacPerfectAvoidTimes);
            data.Add(BigJacAssistedAvoidTimes);
            Debug.Log("BigJacAssistedAvoidTimes" + BigJacAssistedAvoidTimes);

            data.Add(JacGameNoneTimes);
            Debug.Log("JacGameNoneTimes" + JacGameNoneTimes);

            return data;
        }

        // �f�[�^�ǂݍ���
        public bool LoadData(BinaryReader br)
        {
            try
            {
                TotalAllGamesCount = br.ReadInt32();
                Debug.Log("TotalAllGamesCount" + TotalAllGamesCount);
                BigGamesCount = br.ReadInt32();
                Debug.Log("BigGamesCount" + BigGamesCount);
                JacGamesCount = br.ReadInt32();
                Debug.Log("JacGamesCount" + JacGamesCount);

                NormalBellHitCount = br.ReadInt32();
                Debug.Log("NormalBellHitCount" + NormalBellHitCount);
                NormalMelonHitCount = br.ReadInt32();
                Debug.Log("NormalMelonHitCount" + NormalMelonHitCount);
                NormalCherry2HitCount = br.ReadInt32();
                Debug.Log("NormalCherry2HitCount" + NormalCherry2HitCount);
                NormalCherry4HitCount = br.ReadInt32();
                Debug.Log("NormalCherry4HitCount" + NormalCherry4HitCount);

                NormalBellLineUpCount = br.ReadInt32();
                Debug.Log("NormalBellLineUpCount" + NormalBellLineUpCount);
                NormalMelonLineUpCount = br.ReadInt32();
                Debug.Log("NormalMelonLineUpCount" + NormalMelonLineUpCount);
                NormalCherry2LineUpCount = br.ReadInt32();
                Debug.Log("NormalCherry2LineUpCount" + NormalCherry2LineUpCount);
                NormalCherry4LineUpCount = br.ReadInt32();
                Debug.Log("NormalCherry4LineUpCount" + NormalCherry4LineUpCount);

                BigBellHitCount = br.ReadInt32();
                Debug.Log("BigBellHitCount" + BigBellHitCount);
                BigMelonHitCount = br.ReadInt32();
                Debug.Log("BigMelonHitCount" + BigMelonHitCount);
                BigCherry2HitCount = br.ReadInt32();
                Debug.Log("BigCherry2HitCount" + BigCherry2HitCount);
                BigCherry4HitCount = br.ReadInt32();
                Debug.Log("BigCherry4HitCount" + BigCherry4HitCount);

                BigBellLineUpCount = br.ReadInt32();
                Debug.Log("BigBellLineUpCount" + BigBellLineUpCount);
                BigMelonLineUpCount = br.ReadInt32();
                Debug.Log("BigMelonLineUpCount" + BigMelonLineUpCount);
                BigCherry2LineUpCount = br.ReadInt32();
                Debug.Log("BigCherry2LineUpCount" + BigCherry2LineUpCount);
                BigCherry4LineUpCount = br.ReadInt32();
                Debug.Log("BigCherry4LineUpCount" + BigCherry4LineUpCount);

                BigJacInTimes = br.ReadInt32();
                Debug.Log("BigJacInTimes" + BigJacInTimes);
                BigJacAvoidTimes = br.ReadInt32();
                Debug.Log("BigCherry4LineUpCount" + BigCherry4LineUpCount);
                BigJacPerfectAvoidTimes = br.ReadInt32();
                Debug.Log("BigCherry4LineUpCount" + BigCherry4LineUpCount);
                BigJacAssistedAvoidTimes = br.ReadInt32();
                Debug.Log("BigCherry4LineUpCount" + BigCherry4LineUpCount);

                JacGameNoneTimes = br.ReadInt32();
                Debug.Log("JacGameNoneTimes" + JacGameNoneTimes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}
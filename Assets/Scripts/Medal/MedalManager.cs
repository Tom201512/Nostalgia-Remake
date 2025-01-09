using System;
using System.Timers;
using UnityEngine;

namespace ReelSpinGame_Medal
{
    //�X���b�g�����̃��_���Ǘ�
    public class MedalManager
    {
        // const

        // �ő�N���W�b�g����
        public const int MAX_CREDITS = 50;

        // �ő�x�b�g����
        public const int MAX_BET = 3;

        // ���_���X�V�̊Ԋu(�~���b)
        public const int MEDAL_UPDATETIME = 120;

        // �ő啥���o��
        public const int MAX_PAYOUT = 15;


        // val

        // �N���W�b�g����
        public int Credits { get; private set; }

        // �x�b�g����
        public int BetAmounts { get; private set; }

        // �����o������
        public int PayoutAmounts { get; private set; }

        // �ō��x�b�g����
        public int MaxBetAmounts { get; private set; }

        // �����p�^�C�}�[
        Timer updateTimer;


        // �C�x���g�p
        private MedalTest medalTest;


        public MedalManager(int _credits, int _currentMaxBet, MedalTest _medalTest)
        {
            this.Credits = _credits;
            BetAmounts = 0;
            PayoutAmounts = 0;
            this.MaxBetAmounts = _currentMaxBet;

            this.medalTest = _medalTest;

            // �C�x���g���󂯂�
            this.medalTest.BetMedal += BetMedals;
            this.medalTest.BetMax += BetMaxMedals;
            this.medalTest.StartPayout += StartPayout;


            // �����p�^�C�}�[�쐬
            updateTimer = new Timer(MEDAL_UPDATETIME);
        }

        // func

        // �x�b�g����
        private void BetMedals(int amounts)
        {
            BetAmounts = Math.Clamp(BetAmounts + amounts, 0, MAX_BET);
            Debug.Log("Bet finished Current Bet:" + BetAmounts);
        }

        private void BetMaxMedals()
        {
            if(BetAmounts < MAX_BET)
            {
                BetAmounts = MAX_BET;
                Debug.Log("Bet finished Current Bet:" + BetAmounts);
            }

            else
            {
                Debug.Log("Already reached Max Bet:" + MaxBetAmounts);
            }
        }

        private void StartPayout(int amounts)
        {
            // �����o�������Ă��Ȃ����`�F�b�N
            if(!updateTimer.Enabled)
            {
                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MAX_PAYOUT);
                updateTimer.Elapsed += PayoutMedal;
                updateTimer.Start();
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }


        // �����o������
        private void PayoutMedal(object sender, ElapsedEventArgs e)
        {
            PayoutAmounts -= 1;
            Debug.Log("Payout Medal by 1");

            // �S�ĕ����o�����珈���I��
            if(PayoutAmounts == 0) 
            { 
                updateTimer.Stop();
                updateTimer.Elapsed -= PayoutMedal;
            }
        }
    }
}

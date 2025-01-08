using ReelSpinGame_Interface;
using ReelSpinGame_Observing;
using System;
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

        // ���_���X�V�̃t���[����
        public const float MEDAL_UPDATETIME = 0.12f;

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
            this.medalTest.betMedal += BetMedals;
            this.medalTest.betMax += BetMaxMedals;
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
    }
}

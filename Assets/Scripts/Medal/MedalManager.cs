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
        public const int MaxCredit = 50;

        // �ő�x�b�g����
        public const int MaxBet = 3;

        // ���_���X�V�̊Ԋu(�~���b)
        public const int MedalUpdateTime = 120;

        // �ő啥���o��
        public const int MaxPayout = 15;


        // var

        // �N���W�b�g����
        public int Credits { get; private set; }

        // �x�b�g����
        public int CurrentBet { get; private set; }

        // �����o������
        public int PayoutAmounts { get; private set; }

        // �ō��x�b�g����
        public int MaxBetAmounts { get; private set; }

        // �c��x�b�g����
        private int remainingBet;

        // �����p�^�C�}�[
        private Timer updateTimer;


        // �R���X�g���N�^
        public MedalManager(int _credits, int _currentMaxBet)
        {
            this.Credits = _credits;
            CurrentBet = 0;
            PayoutAmounts = 0;
            this.MaxBetAmounts = _currentMaxBet;

            // �����p�^�C�}�[�쐬
            updateTimer = new Timer(MedalUpdateTime);
        }

        // �f�X�g���N�^
        ~MedalManager()
        {
            // Timer�̃X�g�b�v
            updateTimer.Stop();
            updateTimer.Elapsed -= InsertMedal;
            updateTimer.Elapsed -= PayoutMedal;
        }


        // func

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����`�F�b�N
            if (!updateTimer.Enabled)
            {
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)

                if(amounts != CurrentBet && amounts <= MaxBetAmounts)
                {
                    remainingBet = Math.Clamp(SetRemaining(amounts), 0, MaxBet);

                    // �������݂̃x�b�g��菭�Ȃ������Ȃ烊�Z�b�g
                    if(amounts < CurrentBet)
                    {
                       ResetMedal();
                    }

                    Debug.Log("Bet Received:" + remainingBet);

                    updateTimer.Elapsed += InsertMedal;
                    updateTimer.Start();
                }
                else
                {
                    if(amounts > MaxBetAmounts)
                    {
                        Debug.Log("The MAX Bet is now :" + MaxBetAmounts);
                    }
                    else
                    {
                        Debug.Log("You already Bet:" + amounts);
                    }
                }
            }
            else
            {
                Debug.Log("Insert is enabled");
            }
        }

        // �����o���J�n
        public void StartPayout(int amounts)
        {
            // �����o�������Ă��Ȃ����`�F�b�N
            if (!updateTimer.Enabled)
            {
                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MaxPayout);
                updateTimer.Elapsed += PayoutMedal;
                updateTimer.Start();
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }

        // ���_�����Z�b�g
        private void ResetMedal()
        {
            CurrentBet = 0;
            Debug.Log("Reset Bet");
        }

        // 
        private int SetRemaining(int amount)
        {
            // ���݂̃x�b�g�������������q����ƍ��������|����
            // (2BET����1BET�̓��Z�b�g���Ē���)

            // �����ꍇ(1���ȏ�x�b�g����Ă��邱��)
            if(amount > CurrentBet && CurrentBet > 0)
            {
                Debug.Log("You bet more than current bet");
                return amount - CurrentBet;
            }

            // ���Ȃ��ꍇ
            // 0���Ȃ炻�̂܂�
            return amount;
        }


        // �R���[�`���p

        // ��������
        private void InsertMedal(object sender, ElapsedEventArgs e)
        {
            remainingBet -= 1;
            Debug.Log("Bet Medal by 1");
            CurrentBet += 1;

            // �S�ĕ����o�����珈���I��
            if (remainingBet <= 0)
            {
                updateTimer.Stop();
                updateTimer.Elapsed -= InsertMedal;

                Debug.Log("Bet Finished");
                Debug.Log("CurrentBet:" + CurrentBet);
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

                Debug.Log("Payout Finished");
            }
        }
    }
}

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
        // �c��x�b�g����
        private int remainingBet;
        // �����p�^�C�}�[
        private Timer updateTimer;

        // �N���W�b�g����
        public int Credits { get; private set; }
        // �x�b�g����
        public int CurrentBet { get; private set; }
        // �����o������
        public int PayoutAmounts { get; private set; }
        // �ō��x�b�g����
        public int MaxBetAmounts { get; private set; }

        // �R���X�g���N�^
        public MedalManager(int credits, int curretMaxBet)
        {
            Credits = credits;
            CurrentBet = 0;
            PayoutAmounts = 0;
            MaxBetAmounts = curretMaxBet;
            // �����p�^�C�}�[�쐬
            updateTimer = new Timer(MedalUpdateTime);
        }

        // �f�X�g���N�^
        ~MedalManager()
        {
            // Timer�̃X�g�b�v
            updateTimer.Stop();
            updateTimer.Dispose();
        }

        // func
        // MAX�x�b�g�����ύX
        public void ChangeMaxBet(int maxBet)
        {
            Debug.Log("Change MAXBET:" + maxBet);
            MaxBetAmounts = maxBet;
        }

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            Debug.Log("Received MAX_BET");
            StartBet(MaxBetAmounts);
        }

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

                    InsertMedal();
                    updateTimer.Elapsed += UpdateInsert;
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
                PayoutMedal();
                updateTimer.Elapsed += UpdatePayout;
                updateTimer.Start();
            }
            else
            {
                Debug.Log("Payout is enabled");
            }
        }

        // ���_�����Z�b�g
        public void ResetMedal()
        {
            CurrentBet = 0;
            Debug.Log("Reset Bet");
        }

        // �c��x�b�g������ݒ�
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

        private void UpdateInsert(object sender, ElapsedEventArgs e)
        {
            // ��������
            if (remainingBet > 0)
            {
                InsertMedal();
            }
            // �S�ē��������珈���I��
            else
            {
                updateTimer.Stop();
                updateTimer.Elapsed -= UpdateInsert;

                Debug.Log("Bet Finished");
                Debug.Log("CurrentBet:" + CurrentBet);
            }
        }

        private void UpdatePayout(object sender, ElapsedEventArgs e)
        {
            // �����o������
            if (PayoutAmounts > 0)
            {
                PayoutMedal();
            }
            // �S�ĕ����o�����珈���I��
            else
            {
                updateTimer.Stop();
                updateTimer.Elapsed -= UpdatePayout;

                Debug.Log("Payout Finished");
            }
        }

        // ��������
        private void InsertMedal()
        {
            remainingBet -= 1;
            Debug.Log("Remaining:" + remainingBet);
            Debug.Log("Bet Medal by 1");
            CurrentBet += 1;
        }

        // �����o������
        private void PayoutMedal()
        {
            PayoutAmounts -= 1;
            Debug.Log("Payout Medal by 1");
        }
    }
}

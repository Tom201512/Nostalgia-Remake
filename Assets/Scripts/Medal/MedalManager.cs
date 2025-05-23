using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ReelSpinGame_Medal
{
    //�X���b�g�����̃��_���Ǘ�
    public class MedalManager
    {
        // const
        // �ő�N���W�b�g����
        public const int MaxCredit = 50;
        // �ŏ��N���W�b�g����(0��菭�Ȃ��ꍇ�̓Z�O�ɕ\���͂��Ȃ�)
        public const int MinCredit = -3;
        // �ő�x�b�g����
        public const int MaxBet = 3;
        // ���_���X�V�̊Ԋu(�~���b)
        public const int MedalUpdateTime = 120;
        // �ő啥���o��
        public const int MaxPayout = 15;

        // var
        // �c��x�b�g����
        private int remainingBet;

        // �������ꂽ���̃C�x���g
        public delegate void MedalInsertedEvent(int insert);
        public event MedalInsertedEvent HasMedalInserted;

        // �����o���ꂽ���̃C�x���g
        public delegate void MedalHasPayoutEvent(int payout);
        public event MedalHasPayoutEvent HasMedalPayout;

        // �N���W�b�g����
        public int Credits { get; private set; }
        // �x�b�g����
        public int CurrentBet { get; private set; }
        // �����o������
        public int PayoutAmounts { get; private set; }
        // �ō��x�b�g����
        public int MaxBetAmounts { get; private set; }
        // �Ō�ɂ��������_������
        public int LastBetAmounts { get; private set; }
        // ���v���C��Ԃ�
        public bool HasReplay { get; private set; }

        // �R���X�g���N�^
        public MedalManager(int credits, int curretMaxBet, int lastBetAmounts, bool hasReplay)
        {
            Credits = credits;
            CurrentBet = 0;
            PayoutAmounts = 0;
            MaxBetAmounts = curretMaxBet;
            LastBetAmounts = 0;
            HasReplay = hasReplay;

            HasMedalInserted += InsertMedal;
            HasMedalPayout += PayoutMedal;
        }

        // func
        // MAX�x�b�g�����ύX
        public void ChangeMaxBet(int maxBet)
        {
            Debug.Log("Change MAXBET:" + maxBet);
            MaxBetAmounts = maxBet;
        }

        // �N���W�b�g�����𑝂₷
        public void ChangeCredit(int amount) => Credits = Math.Clamp(Credits += amount, MinCredit, MaxCredit);

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            Debug.Log("Received MAX_BET");
            StartBet(MaxBetAmounts);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!HasReplay && UpdateInsert().IsCompleted)
            {
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if(amounts != CurrentBet && amounts <= MaxBetAmounts)
                {
                    SetRemaining(amounts);

                    // �������݂̃x�b�g��菭�Ȃ������Ȃ烊�Z�b�g
                    if(amounts < CurrentBet)
                    {
                        // �x�b�g�Ŏg�����N���W�b�g����Ԃ�
                        ChangeCredit(CurrentBet);
                        ResetMedal();
                    }

                    Debug.Log("Bet Received:" + remainingBet);

                    // ���_���̓������J�n����(�c��̓t���[������)
                    LastBetAmounts = amounts;
                    Task.Run(UpdateInsert);
                }

                // �x�b�g�����łɏI����Ă���A�܂���MAX�x�b�g�̏ꍇ(Debug)
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

            // �������Ń��_����������Ȃ��ꍇ
            else
            {
                if(HasReplay)
                {
                    Debug.Log("Replay is enabled");
                }
                else
                {
                    Debug.Log("Insert is enabled");
                }
            }
        }

        // �����o���J�n
        public void StartPayout(int amounts)
        {
            // �����o�������Ă��Ȃ����`�F�b�N
            if (UpdatePayout().IsCompleted)
            {
                // �N���W�b�g������0��菭�Ȃ����`�F�b�N
                if(Credits < 0)
                {
                    Credits = 0;
                }

                PayoutAmounts = Math.Clamp(PayoutAmounts + amounts, 0, MaxPayout);

                // ���_���̕����o�����J�n����(�c��̓t���[������)

                if (amounts > 0)
                {
                    Task.Run(UpdatePayout);
                }
                else
                {
                    Debug.Log("No Payouts");
                }
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

        // ���v���C��Ԃɂ���(�O��Ɠ������_��������������)
        public void SetReplay()
        {
            Debug.Log("Enable Replay" + LastBetAmounts);
            SetRemaining(LastBetAmounts);
            Task.Run(UpdateInsert);
            HasReplay = true;
        }

        // ���v���C��ԉ���
        public void DisableReplay()
        {
            HasReplay = false;
            LastBetAmounts = 0;
        }

        // �c��x�b�g������ݒ�
        private void SetRemaining(int amount)
        {
            // ���݂̃x�b�g�������������q����ƍ��������|����
            // (2BET����1BET�̓��Z�b�g���Ē���)

            // �����ꍇ(1���ȏ�x�b�g����Ă��邱��)
            if(amount > CurrentBet && CurrentBet > 0)
            {
                Debug.Log("You bet more than current bet");
                remainingBet = Math.Clamp(amount - CurrentBet, 0, MaxBet);
            }

            // ���Ȃ��ꍇ
            else
            {
                // 0���Ȃ炻�̂܂�
                remainingBet = Math.Clamp(amount, 0, MaxBet);
            }
        }

        // �R���[�`���p
        private async Task UpdateInsert()
        {
            // ��������
            while (remainingBet > 0)
            {
                HasMedalInserted.Invoke(1);
                await Task.Delay(MedalUpdateTime);
            }
            // �S�ē��������珈���I��

            Debug.Log("Bet Finished");
            Debug.Log("CurrentBet:" + CurrentBet);
        }

        private async Task UpdatePayout()
        {
            // �����o������
            while (PayoutAmounts > 0)
            {
                HasMedalPayout.Invoke(1);
                await Task.Delay(MedalUpdateTime);
            }

            // �S�ĕ����o�����珈���I��
            Debug.Log("Payout Finished");
        }

        // ��������
        private void InsertMedal(int amount)
        {
            remainingBet -= amount;
            Debug.Log("Remaining:" + remainingBet);
            Debug.Log("Bet Medal by 1");
            CurrentBet += amount;

            if (!HasReplay)
            {
                ChangeCredit(-amount);
            }
        }

        // �����o������
        private void PayoutMedal(int amount)
        {
            PayoutAmounts -= amount;
            ChangeCredit(amount);
            Debug.Log("Payout Medal by:" + amount);
        }
    }
}

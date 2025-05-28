using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehaviour;

namespace ReelSpinGame_Medal
{
    //�X���b�g�����̃��_���Ǘ�
    public class MedalManager : MonoBehaviour
    {
        // const
        // ���_���X�V�̊Ԋu(�~���b)
        public const float MedalUpdateTime = 0.12f;

        // var
        // ���_�������̃f�[�^
        private MedalBehaviour data;

        // ���_����UI
        [SerializeField] private MedalPanel medalPanel;

        // �N���W�b�g�Z�O�����g
        [SerializeField] private MedalSevenSegment creditSegments;
        // �����o���Z�O�����g
        [SerializeField] private MedalSevenSegment payoutSegments;

        // ���_���̍X�V��������
        public bool HasMedalUpdate { get; private set; }
        // ���_�����������ꂽ��
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsert;
        // ���_���������o���ꂽ��
        public delegate void MedalHasPayoutEvent(int payout);
        public event MedalHasPayoutEvent HasMedalPayout;

        void Awake()
        {
            HasMedalUpdate = false;
        }

        // �R���X�g���N�^
        public void SetMedalData(int credits, int curretMaxBet, int lastBetAmounts, bool hasReplay)
        {
            data = new MedalBehaviour(credits, curretMaxBet, lastBetAmounts, hasReplay);
            //////Debug.Log("Credits:" + credits);
            creditSegments.ShowSegmentByNumber(credits);
        }

        // �^�C�}�[�����̔j��
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // ���l�𓾂�
        public int GetCredits() => data.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetPayoutAmounts() => data.PayoutAmounts;
        public int GetMaxBet() => data.MaxBetAmounts;
        public int GetLastBetAmounts() => data.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.HasReplay;

        // ���l��ς���
        public int ChangeMaxBet(int amounts) => data.MaxBetAmounts = Math.Clamp(data.MaxBetAmounts + amounts, 0, MaxBetLimit);

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.MaxBetAmounts);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!data.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amounts != data.CurrentBet && amounts <= data.MaxBetAmounts)
                {
                    // �x�b�g�����ݒ�
                    data.SetRemainingBet(amounts);
                    // ���_���̓������J�n����(�c��̓t���[������)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // �x�b�g�����łɏI����Ă���A�܂���MAX�x�b�g�̏ꍇ(////Debug)
                else
                {
                    if (amounts > data.MaxBetAmounts)
                    {
                        ////Debug.Log("The MAX Bet is now :" + data.MaxBetAmounts);
                    }
                    else
                    {
                        ////Debug.Log("You already Bet:" + amounts);
                    }
                }
            }

            // �������Ń��_����������Ȃ��ꍇ
            else
            {
                if (data.HasReplay)
                {
                    ////Debug.Log("Replay is enabled");
                }
                else
                {
                    ////Debug.Log("Insert is enabled");
                }
            }
        }

        // �����o���J�n
        public void StartPayout(int amounts)
        {
            // �����o�������Ă��Ȃ����`�F�b�N
            if (!HasMedalUpdate)
            {
                // ���_���̕����o�����J�n����(�c��̓t���[������
                if (amounts > 0)
                {
                    data.PayoutAmounts = Math.Clamp(data.PayoutAmounts + amounts, 0, MaxPayout);
                    StartCoroutine(nameof(UpdatePayout));
                }
                else
                {
                    ////Debug.Log("No Payouts");
                }
            }
            else
            {
                ////Debug.Log("Payout is enabled");
            }
        }

        // ���v���C��Ԃɂ���(�O��Ɠ������_��������������)
        public void EnableReplay()
        {
            ////Debug.Log("Enable Replay" + data.LastBetAmounts);
            data.HasReplay = true;
            data.RemainingBet = data.LastBetAmounts;
        }

        // ���v���C��Ԃ�����
        public void DisableReplay()
        {
            data.HasReplay = false;
            data.LastBetAmounts = 0;
        }

        // ���v���C�������J�n
        public void StartReplayInsert()
        {
            StartCoroutine(nameof(UpdateInsert));
        }

        // ���_�������I��
        public void FinishMedalInsert()
        {
            data.CurrentBet = 0;
            data.FinishedBet = false;
        }

        // func
        // �R���[�`���p
        private IEnumerator UpdateInsert()
        {
            ////Debug.Log("StartBet");
            HasMedalUpdate = true;
            // �c��x�b�g�������Ȃ��Ȃ�܂ŏ���
            while (data.RemainingBet > 0)
            {
                // ���_������
                data.InsertOneMedal();
                // �C�x���g���M
                HasMedalInsert.Invoke();
                // �����v�A�Z�O�����g�X�V
                medalPanel.UpdateLampByBet(data.CurrentBet, data.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.Credits);
                // �����o���Z�O�����g������
                payoutSegments.TurnOffAllSegments();
                // 0.12�b�ҋ@
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
            //////Debug.Log("Bet Finished");
            //////Debug.Log("CurrentBet:" + data.CurrentBet);
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;
            // �����o������
            while (data.PayoutAmounts > 0)
            {
                // ���_�������o��
                data.PayoutOneMedal();
                HasMedalPayout.Invoke(1);
                // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                creditSegments.ShowSegmentByNumber(data.Credits);
                ////Debug.Log("LastPayoutAmounts:" + data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // �S�ĕ����o�����珈���I��
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }
    }
}

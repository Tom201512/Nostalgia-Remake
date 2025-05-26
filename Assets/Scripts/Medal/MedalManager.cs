using System.Collections;
using UnityEngine;

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
        public MedalBehaviour Data { get; private set; }

        // ���_����UI
        [SerializeField] private MedalPanel medalPanel;

        // �N���W�b�g�Z�O�����g
        [SerializeField] private MedalSevenSegment creditSegments;
        // �����o���Z�O�����g
        [SerializeField] private MedalSevenSegment payoutSegments;

        // ���_���̍X�V��������
        public bool HasMedalUpdate { get; private set; }
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
            Data = new MedalBehaviour(credits, curretMaxBet, lastBetAmounts, hasReplay);
            Debug.Log("Credits:" + credits);
            creditSegments.ShowSegmentByNumber(credits);
        }

        // �^�C�}�[�����̔j��
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            Debug.Log("Received MAX_BET");
            StartBet(Data.MaxBetAmounts);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!Data.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amounts != Data.CurrentBet && amounts <= Data.MaxBetAmounts)
                {
                    // �x�b�g�����ݒ�
                    Data.SetRemainingBet(amounts);
                    // ���_���̓������J�n����(�c��̓t���[������)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // �x�b�g�����łɏI����Ă���A�܂���MAX�x�b�g�̏ꍇ(Debug)
                else
                {
                    if (amounts > Data.MaxBetAmounts)
                    {
                        Debug.Log("The MAX Bet is now :" + Data.MaxBetAmounts);
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
                if (Data.HasReplay)
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
            if (!HasMedalUpdate)
            {
                // ���_���̕����o�����J�n����(�c��̓t���[������
                if (amounts > 0)
                {
                    Data.ChangePayoutAmounts(amounts);

                    StartCoroutine(nameof(UpdatePayout));
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

        // ���v���C��Ԃɂ���(�O��Ɠ������_��������������)
        public void SetReplay()
        {
            Debug.Log("Enable Replay" + Data.LastBetAmounts);
            Data.EnableReplay();
            StartCoroutine(nameof(UpdateInsert));
        }

        // func
        // �R���[�`���p
        private IEnumerator UpdateInsert()
        {
            Debug.Log("StartBet");
            HasMedalUpdate = true;
            // �c��x�b�g�������Ȃ��Ȃ�܂ŏ���
            while (Data.RemainingBet > 0)
            {
                // ���_������
                Data.InsertOneMedal();
                // �����v�A�Z�O�����g�X�V
                medalPanel.UpdateLampByBet(Data.CurrentBet, Data.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(Data.Credits);
                // �����o���Z�O�����g������
                payoutSegments.TurnOffAllSegments();
                // 0.12�b�ҋ@
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
            Debug.Log("Bet Finished");
            Debug.Log("CurrentBet:" + Data.CurrentBet);
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;
            // �����o������
            while (Data.PayoutAmounts > 0)
            {
                // ���_�������o��
                Data.PayoutOneMedal();
                HasMedalPayout.Invoke(1);
                // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                creditSegments.ShowSegmentByNumber(Data.Credits);
                Debug.Log("LastPayoutAmounts:" + Data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(Data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // �S�ĕ����o�����珈���I��
            HasMedalUpdate = false;
            Debug.Log("Payout Finished");
        }
    }
}

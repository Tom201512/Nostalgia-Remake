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
        // ���_������
        public MedalBehaviour MedalBehaviour { get; private set; }

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
            MedalBehaviour = new MedalBehaviour(credits, curretMaxBet, lastBetAmounts, hasReplay);
            // �����ŃN���W�b�g�X�V
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
            StartBet(MedalBehaviour.MaxBetAmounts);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!MedalBehaviour.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amounts != MedalBehaviour.CurrentBet && amounts <= MedalBehaviour.MaxBetAmounts)
                {
                    // �x�b�g�����ݒ�
                    MedalBehaviour.SetRemainingBet(amounts);
                    // ���_���̓������J�n����(�c��̓t���[������)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // �x�b�g�����łɏI����Ă���A�܂���MAX�x�b�g�̏ꍇ(Debug)
                else
                {
                    if (amounts > MedalBehaviour.MaxBetAmounts)
                    {
                        Debug.Log("The MAX Bet is now :" + MedalBehaviour.MaxBetAmounts);
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
                if (MedalBehaviour.HasReplay)
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
                    MedalBehaviour.ChangePayoutAmounts(amounts);

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
            Debug.Log("Enable Replay" + MedalBehaviour.LastBetAmounts);
            MedalBehaviour.EnableReplay();
            StartCoroutine(nameof(UpdateInsert));
        }

        // func
        // �R���[�`���p
        private IEnumerator UpdateInsert()
        {
            Debug.Log("StartBet");
            HasMedalUpdate = true;
            // �c��x�b�g�������Ȃ��Ȃ�܂ŏ���
            while (MedalBehaviour.RemainingBet > 0)
            {
                // ���_������
                MedalBehaviour.InsertOneMedal();
                // �����v�A�Z�O�����g�X�V
                medalPanel.UpdateLampByBet(MedalBehaviour.CurrentBet, MedalBehaviour.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(MedalBehaviour.Credits);
                // �����o���Z�O�����g������
                payoutSegments.TurnOffAllSegments();
                // 0.12�b�ҋ@
                yield return new WaitForSeconds(MedalUpdateTime);
            }

            HasMedalUpdate = false;
            Debug.Log("Bet Finished");
            Debug.Log("CurrentBet:" + MedalBehaviour.CurrentBet);
        }

        private IEnumerator UpdatePayout()
        {
            HasMedalUpdate = true;
            // �����o������
            while (MedalBehaviour.PayoutAmounts > 0)
            {
                // ���_�������o��
                MedalBehaviour.PayoutOneMedal();
                HasMedalPayout.Invoke(1);
                // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                creditSegments.ShowSegmentByNumber(MedalBehaviour.Credits);
                payoutSegments.ShowSegmentByNumber(MedalBehaviour.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // �S�ĕ����o�����珈���I��
            HasMedalUpdate = false;
            Debug.Log("Payout Finished");
        }
    }
}

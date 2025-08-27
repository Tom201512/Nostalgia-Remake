using ReelSpinGame_Interface;
using ReelSpinGame_Save.Medal;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Medal
{
    //�X���b�g�����̃��_���Ǘ�
    public class MedalManager : MonoBehaviour, IHasSave
    {
        // const
        // ���_���X�V�̊Ԋu(�~���b)
        public const float MedalUpdateTime = 0.12f;

        // var
        // ���_�������̃f�[�^
        private MedalBehavior data;

        // ���_����UI
        [SerializeField] private MedalPanel medalPanel;

        // �N���W�b�g�Z�O�����g
        [SerializeField] private MedalSevenSegment creditSegments;
        // �����o���Z�O�����g
        [SerializeField] private MedalSevenSegment payoutSegments;

        // ���_���̍X�V��������
        public bool HasMedalUpdate { get; private set; }
        // �Z�O�����g���X�V����
        public bool HasSegmentUpdate { get; private set; }
        // ���_�����������ꂽ��
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsert;

        void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
            HasSegmentUpdate = false;
        }

        void Start()
        {
            // �N���W�b�g�X�V
            creditSegments.ShowSegmentByNumber(data.system.Credit);
        }

        // �^�C�}�[�����̔j��
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // ���l�𓾂�
        public int GetCredit() => data.system.Credit;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayout() => data.RemainingPayout;
        public int GetMaxBet() => data.system.MaxBetAmount;
        public int GetLastBetAmount() => data.system.LastBetAmount;
        public int GetLastPayout() => data.LastPayoutAmount;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.system.HasReplay;

        // �Z�[�u�f�[�^�ɂ���
        public ISavable MakeSaveData()
        {
            MedalSave save = new MedalSave();
            save.RecordData(data.system);
            return save;
        }

        // �Z�[�u��ǂݍ���
        public void LoadSaveData(ISavable loadData)
        {
            if(loadData.GetType() == typeof(MedalSave))
            {
                MedalSave save = loadData as MedalSave;

                data.system.Credit = save.Credit;
                data.system.MaxBetAmount = save.MaxBetAmount;
                data.system.LastBetAmount = save.LastBetAmount;
                data.system.HasReplay = save.HasReplay;
            }
            else
            {
                throw new Exception("Loaded data is not MedalData");
            }
        }

        // MAX�x�b�g�����ύX
        public int ChangeMaxBet(int amount) => data.system.MaxBetAmount = Math.Clamp(amount, 0, MaxBetLimit);

        // �����o���Z�O�����g�X�V���J�n����
        public void StartSegmentUpdate() => HasSegmentUpdate = true;

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.system.MaxBetAmount, false);
        }

        // �x�b�g�����J�n
        public void StartBet(int amount, bool cutCoroutine)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!data.system.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amount != data.CurrentBet && amount <= data.system.MaxBetAmount)
                {
                    // �x�b�g�����ݒ�
                    data.SetRemainingBet(amount);

                    // �R���[�`���𖳎�����ꍇ
                    if (cutCoroutine)
                    {
                        data.CurrentBet = amount;
                        data.system.Credit = Math.Clamp(data.system.Credit -= amount, MinCredit, MaxCredit);
                        data.FinishedBet = true;

                        // �����v�A�Z�O�����g�X�V
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                        // �N���W�b�g�X�V
                        creditSegments.ShowSegmentByNumber(data.system.Credit);
                        // �����o���Z�O�����g������
                        payoutSegments.TurnOffAllSegments();
                    }
                    else
                    {
                        // ���_���̓������J�n����(�c��̓t���[������)
                        StartCoroutine(nameof(UpdateInsert));
                    }
                }
            }
        }

        // �����o���J�n
        public void StartPayout(int amount, bool cutCoroutine)
        {
            // �����o�������Ă��Ȃ����`�F�b�N
            if (!HasMedalUpdate)
            {
                // ���_���̕����o�����J�n����(���_����������͉̂��o�ŁA�f�[�^��ł͂��łɑ����Ă���)
                if (amount > 0)
                {
                    // �����o�������̐ݒ�
                    data.RemainingPayout = Math.Clamp(amount, 0, MaxPayout);
                    // �N���W�b�g�̑���
                    data.ChangeCredit(data.RemainingPayout);

                    // �R���[�`���𖳎�����ꍇ
                    if (cutCoroutine)
                    {
                        data.LastPayoutAmount = data.RemainingPayout;
                        data.RemainingPayout = 0;
                        // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                        creditSegments.ShowSegmentByNumber(data.system.Credit);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);
                        HasMedalUpdate = false;
                    }
                    else
                    {
                        // �����o���J�n
                        StartCoroutine(nameof(UpdatePayout));
                    }
                }
            }
        }

        // ���v���C��Ԃɂ���(�O��Ɠ������_��������������)
        public void EnableReplay()
        {
            data.system.HasReplay = true;
            data.RemainingBet = data.system.LastBetAmount;
        }

        // ���v���C��Ԃ�����
        public void DisableReplay() => data.system.HasReplay = false;

        // ���v���C�������J�n
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // �R���[�`���𖳎�����ꍇ
            if(hasCoroutineCut)
            {
                // �x�b�g�����ݒ�
                data.CurrentBet = data.system.LastBetAmount;
                data.FinishedBet = true;
                // �����v�A�Z�O�����g�X�V
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credit);
                // �����o���Z�O�����g������
                payoutSegments.TurnOffAllSegments();
            }
            else
            {
                StartCoroutine(nameof(UpdateInsert));
            }
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
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmount);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credit);
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

            // �Z�O�����g�X�V������܂őҋ@
            while(!HasSegmentUpdate)
            {
                yield return new WaitForEndOfFrame();
            }

            // �����o������
            while (data.RemainingPayout > 0)
            {
                // ���_�������o��
                data.PayoutOneMedal();
                //HasMedalPayout.Invoke(1);
                // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credit - data.RemainingPayout);
                ////Debug.Log("LastPayoutAmount:" + data.LastPayoutAmount);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmount);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // �S�ĕ����o�����珈���I��
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }
    }
}

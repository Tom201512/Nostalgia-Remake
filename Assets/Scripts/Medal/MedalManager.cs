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
        // ���_�����������ꂽ��
        public delegate void MedalHasInsertEvent();
        public event MedalHasInsertEvent HasMedalInsert;

        void Awake()
        {
            data = new MedalBehavior();
            HasMedalUpdate = false;
        }

        void Start()
        {
            // �N���W�b�g�X�V
            creditSegments.ShowSegmentByNumber(data.system.Credits);
        }

        // �^�C�}�[�����̔j��
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // ���l�𓾂�
        public int GetCredits() => data.system.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayouts() => data.RemainingPayouts;
        public int GetMaxBet() => data.system.MaxBetAmounts;
        public int GetLastBetAmounts() => data.system.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
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

                data.system.Credits = save.Credits;
                data.system.MaxBetAmounts = save.MaxBetAmounts;
                data.system.LastBetAmounts = save.LastBetAmounts;
                data.system.HasReplay = save.HasReplay;
            }
            else
            {
                throw new Exception("Loaded data is not MedalData");
            }
        }

        // ���l��ς���
        public int ChangeMaxBet(int amounts) => data.system.MaxBetAmounts = Math.Clamp(amounts, 0, MaxBetLimit);

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.system.MaxBetAmounts, false);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts, bool cutCoroutine)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!data.system.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amounts != data.CurrentBet && amounts <= data.system.MaxBetAmounts)
                {
                    // �x�b�g�����ݒ�
                    data.SetRemainingBet(amounts);

                    // �R���[�`���𖳎�����ꍇ
                    if (cutCoroutine)
                    {
                        for(int i = 0; i < data.RemainingBet; i++)
                        {
                            data.InsertOneMedal();
                        }

                        // �����v�A�Z�O�����g�X�V
                        medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                        // �N���W�b�g�X�V
                        creditSegments.ShowSegmentByNumber(data.system.Credits);
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
        public void StartPayout(int amounts, bool cutCoroutine)
        {
            Debug.Log("CutCoroutine:" + cutCoroutine);
            // �����o�������Ă��Ȃ����`�F�b�N
            if (!HasMedalUpdate)
            {
                // ���_���̕����o�����J�n����(���_����������͉̂��o�ŁA�f�[�^��ł͂��łɑ����Ă���)
                if (amounts > 0)
                {
                    // �����o�������̐ݒ�
                    data.RemainingPayouts = Math.Clamp(data.RemainingPayouts + amounts, 0, MaxPayout);
                    // �N���W�b�g�̑���
                    data.ChangeCredits(amounts);

                    // �R���[�`���𖳎�����ꍇ
                    if (cutCoroutine)
                    {
                        Debug.Log("Remaining:" +  data.RemainingPayouts);
                        while(data.RemainingPayouts > 0)
                        {
                            data.PayoutOneMedal();
                        }

                        // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                        creditSegments.ShowSegmentByNumber(data.system.Credits);
                        Debug.Log("Payout:" + data.LastPayoutAmounts);
                        payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

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
            data.RemainingBet = data.system.LastBetAmounts;
        }

        // ���v���C��Ԃ�����
        public void DisableReplay() => data.system.HasReplay = false;

        // ���v���C�������J�n
        public void StartReplayInsert(bool hasCoroutineCut)
        {
            // �R���[�`���𖳎�����ꍇ
            if(hasCoroutineCut)
            {
                for (int i = 0; i < data.system.LastBetAmounts; i++)
                {
                    data.InsertOneMedal();
                }

                // �����v�A�Z�O�����g�X�V
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credits);
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
                medalPanel.UpdateLampByBet(data.CurrentBet, data.system.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credits);
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
            while (data.RemainingPayouts > 0)
            {
                // ���_�������o��
                data.PayoutOneMedal();
                //HasMedalPayout.Invoke(1);
                // �N���W�b�g�ƕ����o���Z�O�����g�X�V
                creditSegments.ShowSegmentByNumber(data.system.Credits - data.RemainingPayouts);
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

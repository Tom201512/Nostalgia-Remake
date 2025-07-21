using ReelSpinGame_Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Medal.MedalBehavior;

namespace ReelSpinGame_Medal
{
    //�X���b�g�����̃��_���Ǘ�
    public class MedalManager : MonoBehaviour, ISavable
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

        // ���_�����̃Z�b�g
        public void SetMedalData(MedalSystemSave medalSystemSave)
        {
            data.MedalSave = medalSystemSave;
            Debug.Log("Credits:" + data.MedalSave.Credits);
            Debug.Log("MaxBet:" + data.MedalSave.MaxBetAmounts);
            Debug.Log("LastBet:" + data.MedalSave.LastBetAmounts);
            Debug.Log("HasReplay:" + data.MedalSave.HasReplay);

            if(data.MedalSave.HasReplay)
            {
                EnableReplay();
            }
            creditSegments.ShowSegmentByNumber(data.MedalSave.Credits);
        }

        // �^�C�}�[�����̔j��
        private void OnDestroy() 
        {
            StopAllCoroutines();
        }

        // ���l�𓾂�
        public int GetCredits() => data.MedalSave.Credits;
        public int GetCurrentBet() => data.CurrentBet;
        public int GetRemainingPayouts() => data.RemainingPayouts;
        public int GetMaxBet() => data.MedalSave.MaxBetAmounts;
        public int GetLastBetAmounts() => data.MedalSave.LastBetAmounts;
        public int GetLastPayout() => data.LastPayoutAmounts;
        public bool GetBetFinished() => data.FinishedBet;
        public bool GetHasReplay() => data.MedalSave.HasReplay;

        // ���l��ς���
        public int ChangeMaxBet(int amounts) => data.MedalSave.MaxBetAmounts = Math.Clamp(amounts, 0, MaxBetLimit);

        // MAX_BET�p�̏���
        public void StartMAXBet()
        {
            ////Debug.Log("Received MAX_BET");
            StartBet(data.MedalSave.MaxBetAmounts);
        }

        // �x�b�g�����J�n
        public void StartBet(int amounts)
        {
            // �����������Ă��Ȃ����A�܂��̓��v���C�łȂ����`�F�b�N
            if (!data.MedalSave.HasReplay && !HasMedalUpdate)
            {
                // �����𒲐�
                // ���݂̖����ƈ������x�b�g(���݂�MAX BET�𒴂��Ă��Ȃ�����, JAC��:1BET, �ʏ�:3BET)
                if (amounts != data.CurrentBet && amounts <= data.MedalSave.MaxBetAmounts)
                {
                    // �x�b�g�����ݒ�
                    data.SetRemainingBet(amounts);
                    // ���_���̓������J�n����(�c��̓t���[������)
                    StartCoroutine(nameof(UpdateInsert));
                }
                // �x�b�g�����łɏI����Ă���A�܂���MAX�x�b�g�̏ꍇ(////Debug)
                else
                {
                    if (amounts > data.MedalSave.MaxBetAmounts)
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
                if (data.MedalSave.HasReplay)
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
                // ���_���̕����o�����J�n����(���_����������͉̂��o�ŁA�f�[�^��ł͂��łɑ����Ă���)
                if (amounts > 0)
                {
                    // �����o�������̐ݒ�
                    data.RemainingPayouts = Math.Clamp(data.RemainingPayouts + amounts, 0, MaxPayout);
                    // �N���W�b�g�̑���
                    data.ChangeCredits(amounts);

                    // �����o���J�n
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
            Debug.Log("Enable Replay" + data.MedalSave.LastBetAmounts);
            data.MedalSave.HasReplay = true;
            data.RemainingBet = data.MedalSave.LastBetAmounts;
        }

        // ���v���C��Ԃ�����
        public void DisableReplay()
        {
            data.MedalSave.HasReplay = false;
            data.MedalSave.LastBetAmounts = 0;
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
                medalPanel.UpdateLampByBet(data.CurrentBet, data.MedalSave.LastBetAmounts);
                // �N���W�b�g�X�V
                creditSegments.ShowSegmentByNumber(data.MedalSave.Credits);
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
                creditSegments.ShowSegmentByNumber(data.MedalSave.Credits - data.RemainingPayouts);
                ////Debug.Log("LastPayoutAmounts:" + data.LastPayoutAmounts);
                payoutSegments.ShowSegmentByNumber(data.LastPayoutAmounts);

                yield return new WaitForSeconds(MedalUpdateTime);
            }
            // �S�ĕ����o�����珈���I��
            HasMedalUpdate = false;
            ////Debug.Log("Payout Finished");
        }

        // �Z�[�u
        public List<int> SaveData()
        {
            // ���_������(�N���W�b�g���A�ō��Ŋ|�����閇���A�Ō�Ɋ|���������A���v���C�̗L�����L�^)

            // �ϐ����i�[
            List<int> data = new List<int>();
            data.Add(this.data.MedalSave.Credits);
            data.Add(this.data.MedalSave.MaxBetAmounts);
            data.Add(this.data.MedalSave.LastBetAmounts);
            data.Add(this.data.MedalSave.HasReplay ? 1 : 0);

            return data;
        }

        // �ǂݍ���
        public bool LoadData(BinaryReader bStream)
        {
            try
            {
                // �N���W�b�g����
                data.MedalSave.Credits = bStream.ReadInt32();
                Debug.Log("Credits:" + data.MedalSave.Credits);

                // �ő�x�b�g����
                data.MedalSave.MaxBetAmounts = bStream.ReadInt32();
                Debug.Log("MaxBetAmounts:" + data.MedalSave.MaxBetAmounts);

                // �Ō�Ɋ|��������
                data.MedalSave.LastBetAmounts = bStream.ReadInt32();
                Debug.Log("LastBetAmounts:" + data.MedalSave.LastBetAmounts);

                // ���v���C�̗L��
                data.MedalSave.HasReplay = (bStream.ReadInt32() == 1 ? true : false);
                Debug.Log("HasReplay:" + data.MedalSave.HasReplay);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Debug.Log("MedalSystem Loaded");
            }

            return true;
        }
    }
}

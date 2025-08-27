using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using UnityEngine;
using System.Collections;

namespace ReelSpinGame_State.LotsState
{
    public class EffectState : IGameStatement
    {
        // var
        // ���̃Q�[���̏��
        public MainGameFlow.GameStates State { get; }

        // �Q�[���}�l�[�W��
        private GameManager gM;

        // �R���X�g���N�^
        public EffectState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Effect;
            gM = gameManager;
        }

        public void StateStart()
        {
            // �����I�[�g���؂�Ă���Ή��o�ɓ���
            if (!gM.Auto.HasAuto ||
                (gM.Auto.HasAuto && gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal))
            {
                EnableSounds();
                // �r�^�ӏ������������`�F�b�N
                bool hasBita = gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 10 || gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 16;

                // ���o�J�n
                gM.Effect.StartBeforePayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(),
                    gM.Bonus.GetCurrentBonusStatus(), hasBita);
            }
            else
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }

        public void StateUpdate()
        {
            // UI�X�V
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);

            // �����o���O�̉��o��҂�
            if(!gM.Effect.HasBeforePayoutEffect)
            {
                // �����o���J�n
                if(!gM.Effect.HasPayoutEffectStart)
                {
                    gM.Medal.StartSegmentUpdate();
                    gM.Effect.StartPayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus(),
                        gM.Reel.GetPayoutResultData(), gM.Reel.GetPayoutResultData().PayoutLines);
                }

                // �����o�����I���܂őҋ@
                if (gM.Medal.GetRemainingPayout() == 0 && !gM.Effect.GetHasFlashWait())
                {
                    // ���[�v���Ă��鉹���~
                    gM.Effect.StopLoopSound();
                    // �����o���㉉�o���n�߂�
                    Debug.Log("StartAfterEffect");
                    gM.Effect.StartAfterPayoutEffect();

                    // �S�Ẳ��o���I������烁�_�������ֈڍs
                    if (!gM.Effect.HasAfterPayoutEffect)
                    {
                        gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
                    }
                }
            }
        }

        public void StateEnd()
        {
            // UI�X�V
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
            // �{�[�i�X���o�X�V
            BonusEffectUpdate();
        }

        // �{�[�i�X�֘A�̉��o�X�V
        private void BonusEffectUpdate()
        {
            // �{�[�i�X���̃����v����
            gM.Bonus.UpdateSegments();
            // �{�[�i�X����BGM����
            gM.Effect.PlayBonusBGM(gM.Bonus.GetCurrentBonusStatus(), false);
        }

        // �����I�[�g�����I������SE,BGM�̍Đ�
        private void EnableSounds()
        {
            // BGM, SE�̃~���[�g����
            gM.Effect.ChangeSoundSettingByAuto(gM.Auto.HasAuto, gM.Auto.AutoSpeedID);
        }
    }
}

using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_AutoPlay.AutoPlayFunction.AutoStopOrder;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_State.PlayingState
{
    public class FakeReelSpinState : IGameStatement
    {
        // �^���V�Z
        // const
        // var
        // �L�[���͂���������
        bool hasInput;

        // ���̃Q�[���̏��
        public MainGameFlow.GameStates State { get; }
        // �Q�[���}�l�[�W��
        private GameManager gM;

        // �R���X�g���N�^
        public FakeReelSpinState(GameManager gameManager)
        {
            hasInput = false;

            State = MainGameFlow.GameStates.FakeReel;
            gM = gameManager;
        }

        public void StateStart()
        {
            // �^���V�Z���J�n
            gM.Effect.StartFakeReelSpin();
        }

        public void StateUpdate()
        {
            // �^���V�Z���I���܂ő҂�
            if (!gM.Effect.GetHasFakeReelSpin())
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.WaitState);
            }
        }

        public void StateEnd()
        {

        }
    }
}
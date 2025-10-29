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
        // ‹^——V‹Z
        // const
        // var
        // ƒL[“ü—Í‚ª‚ ‚Á‚½‚©
        bool hasInput;

        // ‚±‚ÌƒQ[ƒ€‚Ìó‘Ô
        public MainGameFlow.GameStates State { get; }
        // ƒQ[ƒ€ƒ}ƒl[ƒWƒƒ
        private GameManager gM;

        // ƒRƒ“ƒXƒgƒ‰ƒNƒ^
        public FakeReelSpinState(GameManager gameManager)
        {
            hasInput = false;

            State = MainGameFlow.GameStates.FakeReel;
            gM = gameManager;
        }

        public void StateStart()
        {
            // ‹^——V‹Z‚ğŠJn
            gM.Effect.StartFakeReelSpin();
        }

        public void StateUpdate()
        {
            // ‹^——V‹Z‚ªI‚í‚é‚Ü‚Å‘Ò‚Â
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
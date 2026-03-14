using ReelSpinGame_Interface;
using ReelSpinGame_Main;

namespace ReelSpinGame_State.PlayingState
{
    // ‹^——V‹ZƒXƒe[ƒg
    public class FakeReelSpinState : IGameStatement
    {
        private GameManager gM;     // ƒQ[ƒ€ƒ}ƒl[ƒWƒƒ

        public FakeReelSpinState(GameManager gameManager)
        {
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
            if (!gM.Effect.GetHasFakeSpin())
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.WaitState);
            }
        }

        public void StateEnd()
        {

        }
    }
}
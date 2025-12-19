using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PlayingState
{
    // ‹^——V‹ZƒXƒe[ƒg
    public class FakeReelSpinState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }   // ‚±‚ÌƒQ[ƒ€‚Ìó‘Ô
        private GameManager gM;                         // ƒQ[ƒ€ƒ}ƒl[ƒWƒƒ

        public FakeReelSpinState(GameManager gameManager)
        {
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
            if (!gM.Effect.GetHasFakeSpin())
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.WaitState);
            }
        }

        public void StateEnd()
        {

        }
    }
}
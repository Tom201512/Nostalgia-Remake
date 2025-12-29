using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PlayingState
{
    // 疑似遊技ステート
    public class FakeReelSpinState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }   // ステート名

        private GameManager gM;     // ゲームマネージャ

        public FakeReelSpinState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.FakeReel;
            gM = gameManager;
        }

        public void StateStart()
        {
            // 疑似遊技を開始
            gM.Effect.StartFakeReelSpin();
        }

        public void StateUpdate()
        {
            // 疑似遊技が終わるまで待つ
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
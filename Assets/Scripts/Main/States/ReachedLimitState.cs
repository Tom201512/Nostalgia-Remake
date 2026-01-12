using ReelSpinGame_Interface;

namespace ReelSpinGame_State.LotsState
{
    // 打ち止めステート
    public class ReachedLimitState : IGameStatement
    {
        private GameManager gM;                              // ゲームマネージャ

        public ReachedLimitState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            gM.SetLimitReached();
            gM.Effect.StartLimitReachedEffect();
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {

        }
    }
}
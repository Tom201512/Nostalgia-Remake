using ReelSpinGame_Interface;
using ReelSpinGame_Medal;

namespace ReelSpinGame_State.LotsState
{
    // 打ち止めステート
    public class ReachedLimitState : IGameStatement
    {
        private GameManager gM;                              // ゲームマネージャ
        private MedalManager medalManager;      // メダル管理

        public ReachedLimitState(GameManager gameManager, MedalManager medalManager)
        {
            gM = gameManager;
            this.medalManager = medalManager;
        }

        public void StateStart()
        {
            gM.SetLimitReached();
            gM.Effect.StartLimitReachedEffect();
            medalManager.DisableMedalBetLamp();
            gM.Status.TurnOffInsertAndStart();
            gM.Status.TurnOffReplayLamp();
        }

        public void StateUpdate()
        {
        }

        public void StateEnd()
        {

        }
    }
}
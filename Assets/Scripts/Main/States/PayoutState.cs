using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public PayoutState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Payout;
            this.gameManager = gameManager;

        }
        public void StateStart()
        {
            Debug.Log("Start Payout State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Payout State");
        }

        public void StateEnd()
        {
            Debug.Log("End Payout State");
        }
    }
}
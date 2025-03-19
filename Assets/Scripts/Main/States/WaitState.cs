using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.LotsState
{
    public class WaitState : IGameStatement
    {
        // var
        // このゲームの状態

        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public WaitState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Wait;
            this.gameManager = gameManager;
        }

        public void StateStart()
        {
            Debug.Log("Start Wait State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Wait State");
            if(!gameManager.Wait.hasWait)
            {
                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.playingState);
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Wait State");
            gameManager.Wait.SetWaitTimer();
        }
    }
}
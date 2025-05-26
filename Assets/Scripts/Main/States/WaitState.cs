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

            // ウェイトランプ点灯
            if (!gameManager.Wait.HasWait)
            {
                gameManager.Status.TurnOnWaitLamp();
            }
        }

        public void StateUpdate()
        {
            // ウェイトが切れるまで待つ
            Debug.Log("Update Wait State");
            if(!gameManager.Wait.HasWait)
            {
                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.PlayingState);
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Wait State");
            gameManager.Wait.SetWaitTimer();

            // ウェイトランプを切る
            gameManager.Status.TurnOffWaitLamp();
        }
    }
}
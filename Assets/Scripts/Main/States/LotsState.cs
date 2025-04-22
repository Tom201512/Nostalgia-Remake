using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.LotsState
{
    public class LotsState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public LotsState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.FlagLots;
            this.gameManager = gameManager;
        }

        public void StateStart()
        {
            Debug.Log("Start Lots State");

            if(gameManager.UseInstant)
            {
                gameManager.Lots.SelectFlag(gameManager.InstantFlagID);
            }
            else
            {
                gameManager.Lots.GetFlagLots(gameManager.Setting);
            }

            gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.WaitState);
        }

        public void StateUpdate()
        {
            Debug.Log("Update Lots State");
        }

        public void StateEnd()
        {
            Debug.Log("End Lots State");
        }
    }
}
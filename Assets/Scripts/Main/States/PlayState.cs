using UnityEngine;
using ReelSpinGame_Interface;

namespace ReelSpinGame_State.PlayingState
{
    public class PlayingState : IGameStatement
    {
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public PlayingState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Playing;
            this.gameManager = gameManager;
        }

        public void StateStart()
        {
            Debug.Log("Start Playing State");
        }

        public void StateUpdate()
        {
            Debug.Log("Update Playing State");
        }

        public void StateEnd()
        {
            Debug.Log("End Playing State");
        }
    }
}
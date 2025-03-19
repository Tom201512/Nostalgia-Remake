using UnityEngine;
using ReelSpinGame_Interface;
using ReelSpinGame_Medal;
using ReelSpinGame_Util.OriginalInputs;

namespace ReelSpinGame_State.InsertState
{
    public class InsertState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public InsertState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Insert;
            this.gameManager = gameManager;
        }

        // func
        public void StateStart()
        {
            Debug.Log("Start Medal Insert");
            gameManager.Medal.ResetMedal();
        }

        public void StateUpdate()
        {
            // MAX BET
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.MaxBet]))
            {
                gameManager.Medal.StartMAXBet();
            }

            // BET2
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetOne]))
            {
                gameManager.Medal.StartBet(2);
            }

            // BET1
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetOne]))
            {
                gameManager.Medal?.StartBet(1);
            }

            // ベット終了
            if(OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StartAndMax]))
            {
                // すでにベットされている場合は抽選へ
                if (gameManager.Medal.CurrentBet > 0)
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.lotsState);
                }
                // そうでない場合はMAX BET
                else
                {
                    gameManager.Medal.StartMAXBet();
                }
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Medal Insert");
        }
    }
}
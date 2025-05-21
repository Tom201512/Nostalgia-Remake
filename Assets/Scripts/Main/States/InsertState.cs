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
        }

        public void StateUpdate()
        {
            // MAX BET
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.MaxBet]))
            {
                gameManager.Medal.StartMAXBet();
            }

            // BET2
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetTwo]))
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
                // 投入枚数を反映する
                gameManager.Medal.ApplyLastBetToReceiver();

                // すでにベットされている場合は抽選へ
                if (gameManager.Medal.CurrentBet > 0)
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.LotsState);

                    // ボーナス中なら払い出し枚数を減らす
                    if(gameManager.Bonus.CurrentBonusStatus != ReelSpinGame_Bonus.BonusManager.BonusStatus.BonusNone)
                    {
                        gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Medal.CurrentBet);
                    }
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
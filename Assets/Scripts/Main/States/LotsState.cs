using UnityEngine;
using ReelSpinGame_Interface;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Bonus.BonusBehaviour;

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
            Debug.Log("Start Lots.FlagBehaviour.State");

            gameManager.Lots.StartFlagLots(gameManager.Setting, gameManager.Medal.Data.LastBetAmounts);

            // ボーナス当選ならプレイヤー側にデータを作成(後で入賞時のゲーム数をカウントする)
            if(gameManager.Lots.Data.CurrentFlag == FlagId.FlagBig)
            {
                gameManager.PlayerData.AddBonusResult(BonusType.BonusBIG);
            }
            else if (gameManager.Lots.Data.CurrentFlag == FlagId.FlagReg)
            {
                gameManager.PlayerData.AddBonusResult(BonusType.BonusREG);
            }

            // ボーナス中ならここでゲーム数を減らす
            if (gameManager.Bonus.Data.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                gameManager.Bonus.Data.DecreaseGames();
            }
            // そうでない場合は通常時のゲーム数を加算
            else
            {
                gameManager.PlayerData.IncreaseGameValue();
            }

            gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.WaitState);
        }

        public void StateUpdate()
        {
            Debug.Log("Update Lots.FlagBehaviour.State");
        }

        public void StateEnd()
        {
            Debug.Log("End Lots.FlagBehaviour.State");
            if (gameManager.Wait.HasWait)
            {
                gameManager.Status.TurnOnWaitLamp();
            }
        }
    }
}
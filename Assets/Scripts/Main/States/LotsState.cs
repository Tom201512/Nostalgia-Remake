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

            gameManager.Lots.GetFlagLots(gameManager.Setting, gameManager.Medal.LastBetAmounts);

            // ボーナス当選ならプレイヤー側にデータを作成(後で入賞時のゲーム数をカウントする)
            if(gameManager.Lots.CurrentFlag == ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagBig)
            {
                gameManager.PlayerData.AddBonusResult(ReelSpinGame_Bonus.BonusManager.BonusType.BonusBIG);
            }
            else if (gameManager.Lots.CurrentFlag == ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagReg)
            {
                gameManager.PlayerData.AddBonusResult(ReelSpinGame_Bonus.BonusManager.BonusType.BonusREG);
            }

            // ボーナス中ならここでゲーム数を減らす
            if (gameManager.Bonus.CurrentBonusStatus != ReelSpinGame_Bonus.BonusManager.BonusStatus.BonusNone)
            {
                gameManager.Bonus.DecreaseGames();
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
            Debug.Log("Update Lots State");
        }

        public void StateEnd()
        {
            Debug.Log("End Lots State");
        }
    }
}
using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

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
                StopReelFlash();
            }

            // BET2
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetTwo]))
            {
                gameManager.Medal.StartBet(2);
                StopReelFlash();
            }

            // BET1
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetOne]))
            {
                gameManager.Medal.StartBet(1);
                StopReelFlash();
            }

            // ベット終了
            if(OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StartAndMax]))
            {
                // 投入枚数を反映する
                gameManager.PlayerData.PlayerMedalData.DecreasePlayerMedal(gameManager.Medal.LastBetAmounts);
                gameManager.PlayerData.PlayerMedalData.IncreaseInMedal(gameManager.Medal.LastBetAmounts);

                // すでにベットされている場合は抽選へ
                if (gameManager.Medal.CurrentBet > 0)
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.LotsState);

                    // ボーナス中なら払い出し枚数を減らす
                    if(gameManager.Bonus.CurrentBonusStatus != ReelSpinGame_Bonus.BonusManager.BonusStatus.BonusNone)
                    {
                        gameManager.PlayerData.ChangeBonusPayoutToLast(-gameManager.Medal.LastBetAmounts);
                    }
                }
                // そうでない場合はMAX BET
                else
                {
                    gameManager.Medal.StartMAXBet();
                    StopReelFlash();
                }
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Medal Insert");
        }

        // フラッシュを止める
        private void StopReelFlash()
        {
            Debug.Log("Stop Flash");
            gameManager.Reel.FlashManager.StopFlash();

            // JAC GAME中なら点灯方法を少し変える
            if(gameManager.Bonus.CurrentBonusStatus == ReelSpinGame_Bonus.BonusManager.BonusStatus.BonusJACGames)
            {
                gameManager.Reel.FlashManager.EnableJacGameLight();
                Debug.Log("Jac Turn On");
            }
            else
            {
                gameManager.Reel.FlashManager.TurnOnAllReels();
                Debug.Log("Turn On");
            }
        }
    }
}
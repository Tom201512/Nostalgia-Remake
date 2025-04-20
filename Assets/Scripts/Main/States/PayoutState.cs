using UnityEngine;
using ReelSpinGame_Interface;
using ReelSpinGame_Bonus;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        //test
        public bool TestFlag = false;

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
            Debug.Log("Start Payout Check");

            StartCheckPayout(gameManager.Medal.LastBetAmounts);
            Debug.Log("Payouts result" + gameManager.Payout.LastPayoutResult.Payouts);
            Debug.Log("Bonus:" + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

            gameManager.Medal.ResetMedal();

            // 払い出し
            gameManager.Medal.StartPayout(gameManager.Payout.LastPayoutResult.Payouts);

            // ボーナスごとに処理を変える
            switch (gameManager.Bonus.CurrentBonusStatus)
            {
                // 小役ゲーム中はゲーム数を減らす
                case BonusManager.BonusStatus.BonusBIGGames:
                    gameManager.Bonus.DecreaseBigGames(gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);
                    break;

                // ボーナスゲーム中はゲーム数を減らす
                case BonusManager.BonusStatus.BonusJACGames:
                    gameManager.Bonus.DecreaseBonusGames(gameManager.Payout.LastPayoutResult.Payouts > 0);
                    break;

                // 通常時はリプレイの処理
                default:
                    // ボーナスがあればボーナス開始
                    // BIG
                    if (gameManager.Payout.LastPayoutResult.BonusID == (int)BonusManager.BonusType.BonusBIG)
                    {
                        gameManager.Bonus.StartBigChance();
                    }
                    // REG
                    if (gameManager.Payout.LastPayoutResult.BonusID == (int)BonusManager.BonusType.BonusREG)
                    {
                        gameManager.Bonus.StartBonusGame();
                    }

                    // リプレイまたはJAC-IN
                    if (gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
                    {
                        gameManager.Medal.SetReplay(); // リプレイにする
                    }
                    else if (gameManager.Medal.HasReplay)
                    {
                        gameManager.Medal.DisableReplay();
                    }
                    break;
            }

            //if (TestFlag == false)
            //{
             //   TestFlag = true;
              //  gameManager.Medal.SetReplay(); // リプレイにする
            //}
        }

        public void StateUpdate()
        {
            Debug.Log("Update Payout State");
            if(gameManager.Medal.PayoutAmounts == 0)
            {
                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.InsertState);
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Payout State");
        }

        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.IsWorking)
            {
                gameManager.Payout.CheckPayoutLines(betAmounts, gameManager.Reel.LastSymbols);
            }
            else
            {
                Debug.Log("Failed to check payout because reels are spinning");
            }
        }
    }
}
using UnityEngine;
using ReelSpinGame_Interface;

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
            Debug.Log("Bonus:" + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJAC);

            gameManager.Medal.ResetMedal();

            // 払い出しがあれば
            if (gameManager.Payout.LastPayoutResult.Payouts > 0)
            {
                gameManager.Medal.StartPayout(gameManager.Payout.LastPayoutResult.Payouts);
            }
            // JAC-IN(仮)
            // リプレイの場合(通常時のみ)
            if(gameManager.Payout.LastPayoutResult.IsReplayOrJAC)
            {
                gameManager.Medal.SetReplay(); // リプレイにする
            }
            else if(gameManager.Medal.HasReplay)
            {
                gameManager.Medal.DisableReplay();
            }

            if (TestFlag == false)
            {
                TestFlag = true;
                gameManager.Medal.SetReplay(); // リプレイにする
            }
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
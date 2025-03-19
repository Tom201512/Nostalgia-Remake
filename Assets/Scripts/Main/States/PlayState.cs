using UnityEngine;
using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine.EventSystems;

namespace ReelSpinGame_State.PlayingState
{
    public class PlayingState : IGameStatement
    {
        // const

        // var
        // 払い出し判定完了したか
        public bool hasFinishedCheck { get; private set; }
        // キー入力があったか
        bool hasInput;

        // このゲームの状態
        public MainGameFlow.GameStates State { get; }
        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public PlayingState(GameManager gameManager)
        {
            hasFinishedCheck = true;
            hasInput = false;

            State = MainGameFlow.GameStates.Playing;
            this.gameManager = gameManager;
        }

        public void StateStart()
        {
            Debug.Log("Start Playing State");

            // リール始動
            gameManager.Reel.StartReels();
            hasFinishedCheck = false;
        }

        public void StateUpdate()
        {
            Debug.Log("Update Playing State");
            // 何も入力が入っていなければ実行
            if (!hasInput)
            {
                // リール停止処理
                if (gameManager.Reel.IsWorking)
                {
                    // 左停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopLeft]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelLeft);
                    }
                    // 中停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopMiddle]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelMiddle);
                    }
                    // 右停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopRight]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelRight);
                    }
                }

                // 入力がないかチェック
                if (Input.anyKey)
                {
                    hasInput = true;
                }
                // 入力がなくすべてのリールが止まっていたら払い出し処理をする
                else if (gameManager.Reel.IsFinished && !hasFinishedCheck)
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.PayoutState);
                }
            }

            // 入力がある場合は離れたときの制御を行う
            else if (hasInput)
            {
                //Debug.Log("Input still");
                if (!Input.anyKey)
                {
                    //Debug.Log("input end");
                    hasInput = false;
                }
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Playing State");

            Debug.Log("Start Payout Check");

            StartCheckPayout(3);
            Debug.Log("Payouts result" + gameManager.Payout.LastPayoutResult.Payouts);
            Debug.Log("Bonus:" + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJAC);
        }

        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.IsWorking)
            {
                gameManager.Payout.CheckPayoutLines(betAmounts, gameManager.Reel.LastSymbols);
                hasFinishedCheck = true;
            }
            else
            {
                Debug.Log("Failed to check payout because reels are spinning");
            }
        }
    }
}
using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;

namespace ReelSpinGame_State.PlayingState
{
    public class PlayingState : IGameStatement
    {
        // const
        // var
        // キー入力があったか
        bool hasInput;

        // このゲームの状態
        public MainGameFlow.GameStates State { get; }
        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public PlayingState(GameManager gameManager)
        {
            hasInput = false;

            State = MainGameFlow.GameStates.Playing;
            this.gameManager = gameManager;
        }

        public void StateStart()
        {
            Debug.Log("Start Playing State");

            // リール始動
            gameManager.Reel.StartReels();
        }

        public void StateUpdate()
        {
            // 何も入力が入っていなければ実行
            if (!hasInput)
            {
                // リール停止処理
                if (gameManager.Reel.IsWorking)
                {
                    // 左停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopLeft]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelLeft, 
                            gameManager.Medal.Data.LastBetAmounts, 
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);
                    }
                    // 中停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopMiddle]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelMiddle, 
                            gameManager.Medal.Data.LastBetAmounts,
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);
                    }
                    // 右停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopRight]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelManager.ReelID.ReelRight, 
                            gameManager.Medal.Data.LastBetAmounts, 
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);
                    }
                }

                // 入力がないかチェック
                if (Input.anyKey)
                {
                    hasInput = true;
                }
                // 入力がなくすべてのリールが止まっていたら払い出し処理をする
                else if (gameManager.Reel.IsFinished)
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
        }
    }
}
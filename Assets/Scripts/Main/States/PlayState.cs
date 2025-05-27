using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

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

            // ボーナス中のランプ処理
            gameManager.Bonus.UpdateSegments();

            // スタートサウンド再生
            gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Start);
        }

        public void StateUpdate()
        {
            // 何も入力が入っていなければ実行
            if (!hasInput)
            {
                // リール停止処理
                if (gameManager.Reel.Data.IsWorking)
                {
                    // 左停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopLeft]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelID.ReelLeft, 
                            gameManager.Medal.Data.LastBetAmounts, 
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);

                        PlayStopSound();
                    }
                    // 中停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopMiddle]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelID.ReelMiddle, 
                            gameManager.Medal.Data.LastBetAmounts,
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);

                        PlayStopSound();
                    }
                    // 右停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopRight]))
                    {
                        gameManager.Reel.StopSelectedReel(ReelID.ReelRight, 
                            gameManager.Medal.Data.LastBetAmounts, 
                            gameManager.Lots.Data.CurrentFlag, 
                            gameManager.Bonus.Data.HoldingBonusID);

                        PlayStopSound();
                    }
                }

                // 入力がないかチェック
                if (Input.anyKey)
                {
                    hasInput = true;
                }
                // 入力がなくすべてのリールが止まっていたら払い出し処理をする
                else if (gameManager.Reel.Data.IsFinished)
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

        // 停止音再生
        public void PlayStopSound()
        {
            // 停止音サウンド再生
            gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Stop);
        }
    }
}
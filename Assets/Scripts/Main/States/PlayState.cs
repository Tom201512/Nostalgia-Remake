using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Bonus.BonusBehaviour;

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
            //Debug.Log("Start Playing State");

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
                if (gameManager.Reel.GetIsReelWorking())
                {
                    // 左停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopLeft]))
                    {
                        StopReel(ReelID.ReelLeft);
                    }
                    // 中停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopMiddle]))
                    {
                        StopReel(ReelID.ReelMiddle);
                    }
                    // 右停止
                    if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StopRight]))
                    {
                        StopReel(ReelID.ReelRight);
                    }
                }

                // 入力がないかチェック
                if (Input.anyKey)
                {
                    hasInput = true;
                }
                // 入力がなくすべてのリールが止まっていたら払い出し処理をする
                else if (gameManager.Reel.GetIsReelFinished())
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.PayoutState);
                }
            }

            // 入力がある場合は離れたときの制御を行う
            else if (hasInput)
            {
                ////Debug.Log("Input still");
                if (!Input.anyKey)
                {
                    ////Debug.Log("input end");
                    hasInput = false;
                }
            }
        }

        public void StateEnd()
        {
            //Debug.Log("End Playing State");
        }

        // リール停止
        private void StopReel(ReelID reelID)
        {
            if (gameManager.Reel.GetCanStopReels() && 
                gameManager.Reel.GetReelStatus(reelID) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                gameManager.Reel.StopSelectedReel(reelID,
                    gameManager.Medal.GetLastBetAmounts(),
                    gameManager.Lots.GetCurrentFlag(),
                    gameManager.Bonus.Data.HoldingBonusID);

                // 停止音再生
                PlayStopSound();
                // 通常時,第二停止でリーチしていたら音を鳴らす
                if (gameManager.Reel.GetStoppedCount() == 2 &&
                    gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone)
                {
                    // リーチがあれば再生
                    PlayRiichiSound();
                }
            }
        }

        // 停止音再生
        private void PlayStopSound()
        {
            // 停止音サウンド再生
            gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Stop);
        }

        // テンパイ音再生
        private void PlayRiichiSound()
        {
            // リーチの色を記録
            BigColor riichiValue = gameManager.Reel.CheckRiichiStatus(gameManager.Payout.PayoutDatabase.PayoutLines,
                            gameManager.Medal.GetLastBetAmounts());

            if(riichiValue == BigColor.Red)
            {
                gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.RedRiichiSound);
            }
            if (riichiValue == BigColor.Blue)
            {
                gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.BlueRiichiSound);
            }
            if (riichiValue == BigColor.Black)
            {
                gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.BB7RiichiSound);
            }
        }
    }
}
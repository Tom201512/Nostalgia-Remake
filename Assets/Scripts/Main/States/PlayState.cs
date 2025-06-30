using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
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
            // リール始動
            gameManager.Reel.StartReels();
            // ボーナス中のランプ処理
            gameManager.Bonus.UpdateSegments();
            // スタートサウンド再生
            gameManager.Effect.StartLeverOnEffect();

            // リール停止時に音を鳴らすよう変更
            gameManager.Reel.HasSomeReelStopped += StopReelSound;
        }

        public void StateUpdate()
        {
            if(gameManager.Auto.HasAuto)
            {
                AutoControl();
            }
            else
            {
                PlayerControl();
            }
        }

        public void StateEnd()
        {
            // リール停止時の音を外すようにする
            gameManager.Reel.HasSomeReelStopped -= StopReelSound;
        }

        // リール停止
        private void StopReel(ReelID reelID)
        {
            if (gameManager.Reel.GetCanStopReels() && 
                gameManager.Reel.GetReelStatus(reelID) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                // リールを止める
                gameManager.Reel.StopSelectedReel(reelID,gameManager.Medal.GetLastBetAmounts(),
                    gameManager.Lots.GetCurrentFlag(),gameManager.Bonus.GetHoldingBonusID());
            }
        }

        // オート時の挙動
        private void AutoControl()
        {
            // すべてのリールが止まっていたら払い出し処理をする
            if (gameManager.Reel.GetIsReelFinished())
            {
                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.PayoutState);
            }
            // オート中は指定した押し順で押すようにする
            else
            {
                // 停止待機中のリールから止めるようにする。
                if (gameManager.Reel.GetReelStatus(gameManager.Auto.AutoStopOrders[0]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
                {
                    StopReel(gameManager.Auto.AutoStopOrders[0]);
                }
                else if (gameManager.Reel.GetReelStatus(gameManager.Auto.AutoStopOrders[1]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
                {
                    StopReel(gameManager.Auto.AutoStopOrders[1]);
                }
                else if (gameManager.Reel.GetReelStatus(gameManager.Auto.AutoStopOrders[2]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
                {
                    StopReel(gameManager.Auto.AutoStopOrders[2]);
                }
            }
        }

        // プレイヤー操作時の挙動
        private void PlayerControl()
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
                if (!Input.anyKey)
                {
                    hasInput = false;
                }
            }
        }

        // リール停止のサウンド
        private void StopReelSound()
        {
            // 停止音再生
            gameManager.Effect.StartReelStopEffect();

            // 通常時,第二停止でBIG図柄がリーチしていたら音を鳴らす
            if (gameManager.Reel.GetStoppedCount() == 2 &&
                gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gameManager.Effect.StartRiichiEffect(gameManager.Reel.GetBigLinedUpCounts(gameManager.Medal.GetLastBetAmounts(), 2));
            }
        }
    }
}
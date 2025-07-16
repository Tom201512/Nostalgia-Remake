using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_AutoPlay.AutoPlayFunction.AutoStopOrder;

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
        private GameManager gM;

        // コンストラクタ
        public PlayingState(GameManager gameManager)
        {
            hasInput = false;

            State = MainGameFlow.GameStates.Playing;
            this.gM = gameManager;
        }

        public void StateStart()
        {
            // リール始動
            gM.Reel.StartReels();
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // スタートサウンド再生
            //gM.Effect.StartLeverOnEffect();

            // 予告音バージョン(試験的導入)
            gM.Effect.StartLeverOnEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), gM.Bonus.GetCurrentBonusStatus());

            // リール停止時に音を鳴らすよう変更
            gM.Reel.HasSomeReelStopped += StopReelSound;
        }

        public void StateUpdate()
        {
            if(gM.Auto.HasAuto)
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
            gM.Reel.HasSomeReelStopped -= StopReelSound;
        }

        // リール停止
        private void StopReel(ReelID reelID)
        {
            if (gM.Reel.GetCanStopReels() && 
                gM.Reel.GetReelStatus(reelID) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                // リールを止める
                gM.Reel.StopSelectedReel(reelID,gM.Medal.GetLastBetAmounts(),
                    gM.Lots.GetCurrentFlag(),gM.Bonus.GetHoldingBonusID());
            }
        }

        // オート時の挙動
        private void AutoControl()
        {
            // オート停止位置が決まっているかチェック。決まっていなければすぐ決める
            if(!gM.Auto.HasStopPosDecided)
            {
                gM.Auto.GetAutoStopPos(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(),
                    gM.Bonus.GetRemainingBigGames(), gM.Bonus.GetRemainingJacIn());
            }

            // すべてのリールが止まっていたら払い出し処理をする
            if (gM.Reel.GetIsReelFinished())
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.PayoutState);
            }
            // オート中は指定した押し順で押すようにする
            else
            {
                // BIG中の場合、(JAC回数が残り1回, 残りゲーム数が9G以上)ならJACハズシをする
                if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                    gM.Bonus.GetRemainingBigGames() > 8 && gM.Bonus.GetRemainingJacIn() == 1)
                {
                    AutoJacAvoid();
                }
                // それ以外は指定した押し順で停止
                else
                {
                    NormalAutoStop();
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
                if (gM.Reel.GetIsReelWorking())
                {
                    // 左停止
                    if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.StopLeft]))
                    {
                        StopReel(ReelID.ReelLeft);
                    }
                    // 中停止
                    if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.StopMiddle]))
                    {
                        StopReel(ReelID.ReelMiddle);
                    }
                    // 右停止
                    if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.StopRight]))
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
                else if (gM.Reel.GetIsReelFinished())
                {
                    gM.MainFlow.stateManager.ChangeState(gM.MainFlow.PayoutState);
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
            gM.Effect.StartReelStopEffect();

            // 通常時,第二停止でBIG図柄がリーチしていたら音を鳴らす
            if (gM.Reel.GetStoppedCount() == 2 &&
                gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Effect.StartRiichiEffect(gM.Reel.GetBigLinedUpCounts(gM.Medal.GetLastBetAmounts(), 2));
            }
        }

        // JACハズシ時のオート制御
        private void AutoJacAvoid()
        {
            // 停止待機中のリールがあれば、指定位置になった時に停止させる。(最速モードでは無視)
            // ただし押し順は右->中->左になる。
            if (gM.Reel.GetReelStatus(ReelID.ReelRight) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(ReelID.ReelRight) ==
                    gM.Auto.AutoStopPos[(int)ReelID.ReelRight])
                {
                    StopReel(ReelID.ReelRight);
                }
            }
            else if (gM.Reel.GetReelStatus(ReelID.ReelMiddle) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(ReelID.ReelMiddle) ==
                    gM.Auto.AutoStopPos[(int)ReelID.ReelMiddle])
                {
                    StopReel(ReelID.ReelMiddle);
                }
            }
            else if (gM.Reel.GetReelStatus(ReelID.ReelLeft) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(ReelID.ReelLeft) ==
                    gM.Auto.AutoStopPos[(int)ReelID.ReelLeft])
                {
                    StopReel(ReelID.ReelLeft);
                }
            }
        }

        // 通常のオート制御
        private void NormalAutoStop()
        {
            // 停止待機中のリールがあれば、指定位置になった時に停止させる。(最速モードでは無視)
            if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)First]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(gM.Auto.AutoStopOrders[(int)First]) ==
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)First]])
                {
                    StopReel(gM.Auto.AutoStopOrders[(int)First]);
                }
            }
            else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)Second]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(gM.Auto.AutoStopOrders[(int)Second]) ==
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Second]])
                {
                    StopReel(gM.Auto.AutoStopOrders[(int)Second]);
                }
            }
            else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)Third]) == ReelSpinGame_Reels.ReelData.ReelStatus.WaitForStop)
            {
                if (gM.Reel.GetReelCenterPos(gM.Auto.AutoStopOrders[(int)Third]) ==
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Third]])
                {
                    StopReel(gM.Auto.AutoStopOrders[(int)Third]);
                }
            }
        }
    }
}
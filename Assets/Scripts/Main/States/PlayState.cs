using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_AutoPlay.AutoPlayFunction.AutoStopOrder;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelManagerModel;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

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
            gM = gameManager;
        }

        public void StateStart()
        {
            // 強制役でランダム数値が使われていれば使う
            if(gM.Option.GetForceFlagSelectID() != -1 && gM.Option.GetForceFlagRandomID() != 0)
            {
                gM.Reel.SetForceRandomValue(gM.Option.GetForceFlagRandomID());
            }

            // 強制フラグ設定のリセット
            gM.Option.ResetForceFlagSetting();
            // リール始動(高速オート時は1.5倍のリールスピードになる)
            gM.Reel.StartReels(gM.Bonus.GetCurrentBonusStatus(), gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // スタート音再生
            gM.Effect.StartLeverOnEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), gM.Bonus.GetCurrentBonusStatus());
            // リール停止時に音を鳴らすよう変更
            gM.Reel.HasSomeReelStopped += StopReelSound;
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
        }

        public void StateUpdate()
        {
            // オートがある場合
            if(gM.Auto.HasAuto)
            {
                AutoControl();
            }
            // 強制停止が発動した場合
            else if(gM.Reel.HasForceStop())
            {
                ReelForcedStop();
            }
            // それ以外はプレイヤー操作を受け付ける
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
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                // リールを止める
                gM.Reel.StopSelectedReel(reelID, gM.Medal.GetLastBetAmount(), gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID());
            }
        }

        // 超高速オート時の停止
        private void StopReelQuick(ReelID reelID, int autoStopPos)
        {
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                // リールを止める
                gM.Reel.StopSelectedReelFast(reelID, gM.Medal.GetLastBetAmount(), gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), autoStopPos);
            }
        }

        // オート時の挙動
        private void AutoControl()
        {
            // オート停止位置が決まっているかチェック。決まっていなければすぐ決める
            if (!gM.Auto.HasStopPosDecided)
            {
                gM.Auto.GetAutoStopPos(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(),
                    gM.Bonus.GetRemainingBigGames(), gM.Bonus.GetRemainingJacIn(), gM.Medal.GetLastBetAmount());
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
                    AutoStopBehavior();
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
                    if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.StopLeft))
                    {
                        StopReel(ReelID.ReelLeft);
                    }
                    // 中停止
                    if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.StopMiddle))
                    {
                        StopReel(ReelID.ReelMiddle);
                    }
                    // 右停止
                    if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.StopRight))
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
                gM.Effect.StartRiichiEffect(gM.Reel.GetBigLinedUpCount(gM.Medal.GetLastBetAmount(), 2));
            }
        }

        // JACハズシ時のオート制御
        private void AutoJacAvoid()
        {
            // 押し順は右->中->左になる。
            // 超高速オートなら即座に止めて払い出し状態へ
            if (gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Quick)
            {
                StopReelQuick(gM.Auto.AutoStopOrders[(int)ReelID.ReelRight], gM.Auto.AutoStopPos[(int)ReelID.ReelRight]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)ReelID.ReelMiddle], gM.Auto.AutoStopPos[(int)ReelID.ReelMiddle]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)ReelID.ReelLeft], gM.Auto.AutoStopPos[(int)ReelID.ReelLeft]);
            }
            // 高速、通常速度なら停止位置についたら停止
            else
            {
                // 順番にリールが止まっていないものから停止させる
                if (gM.Reel.GetReelStatus(ReelID.ReelRight) != ReelStatus.Stopped)
                {
                    if (gM.Reel.GetReelPushedPos(ReelID.ReelRight) ==
                        gM.Auto.AutoStopPos[(int)ReelID.ReelRight])
                    {
                        StopReel(ReelID.ReelRight);
                    }
                }
                else if (gM.Reel.GetReelStatus(ReelID.ReelMiddle) != ReelStatus.Stopped)
                {
                    if (gM.Reel.GetReelPushedPos(ReelID.ReelMiddle) ==
                        gM.Auto.AutoStopPos[(int)ReelID.ReelMiddle])
                    {
                        StopReel(ReelID.ReelMiddle);
                    }
                }
                else if (gM.Reel.GetReelStatus(ReelID.ReelLeft) != ReelStatus.Stopped)
                {
                    if (gM.Reel.GetReelPushedPos(ReelID.ReelLeft) ==
                        gM.Auto.AutoStopPos[(int)ReelID.ReelLeft])
                    {
                        StopReel(ReelID.ReelLeft);
                    }
                }
            }
        }

        // JACハズシ以外のオート制御
        private void AutoStopBehavior()
        {
            // 超高速オートなら即座に止めて払い出し状態へ
            if (gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Quick)
            {
                StopReelQuick(gM.Auto.AutoStopOrders[(int)First], gM.Auto.AutoStopPos[(int)First]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)Second], gM.Auto.AutoStopPos[(int)Second]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)Third], gM.Auto.AutoStopPos[(int)Third]);
            }
            else
            {
                // オート速度が高速または通常なら規定位置になったときに停止
                if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)First]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)First]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)First]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)First]);
                    }
                }
                else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)Second]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)Second]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Second]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)Second]);
                    }
                }
                else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)Third]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)Third]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Third]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)Third]);
                    }
                }
            }
        }

        // 強制的に止める制御
        private void ReelForcedStop()
        {
            // 左->中->右から強制停止
            if (gM.Reel.GetReelStatus(ReelID.ReelLeft) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelLeft);
            }
            else if (gM.Reel.GetReelStatus(ReelID.ReelMiddle) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelMiddle);
            }
            else if (gM.Reel.GetReelStatus(ReelID.ReelRight) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelRight);
            }
        }
    }
}
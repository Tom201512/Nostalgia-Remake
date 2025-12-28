using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Reels;
using UnityEngine;

namespace ReelSpinGame_State.PlayingState
{
    // 遊技中ステート
    public class PlayingState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }       // ステート名
        private GameManager gM;                             // ゲームマネージャ

        private bool hasInput;                              // キー入力があったか

        public PlayingState(GameManager gameManager)
        {
            hasInput = false;
            State = MainGameFlow.GameStates.Playing;
            gM = gameManager;
        }

        public void StateStart()
        {
            // 強制役でランダム数値が使われていれば使う
            if (gM.Option.GetForceFlagSelectID() != -1 && gM.Option.GetForceFlagRandomID() != 0)
            {
                gM.Reel.SetForceRandomValue(gM.Option.GetForceFlagRandomID());
            }

            // 強制フラグ設定のリセット
            gM.Option.ResetForceFlagSetting();
            // リール始動
            gM.Reel.StartReels(gM.Bonus.GetCurrentBonusStatus(), gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();

            // レバーオン演出開始
            LeverOnEffectCondition condition = new LeverOnEffectCondition();
            condition.Flag = gM.Lots.GetCurrentFlag();
            condition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartLeverOnEffect(condition);

            // リール停止時に音を鳴らすよう変更
            gM.Reel.SomeReelStoppedEvent += StopReelSound;
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
        }

        public void StateUpdate()
        {
            // オートがある場合
            if (gM.Auto.HasAuto)
            {
                AutoControl();
            }
            // 強制停止が発動した場合
            else if (gM.Reel.HasForceStop())
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
            gM.Reel.SomeReelStoppedEvent -= StopReelSound;
        }

        // リール停止
        void StopReel(ReelID reelID)
        {
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.Reel.StopSelectedReel(reelID, gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), gM.Medal.LastBetAmount);
            }
        }

        // 超高速オート時の停止
        void StopReelQuick(ReelID reelID, int autoStopPos)
        {
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.Reel.StopSelectedReelFast(reelID, gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), gM.Medal.LastBetAmount, autoStopPos);
            }
        }

        // オート時の挙動
        void AutoControl()
        {
            // オート停止位置が決まっているかチェック。決まっていなければすぐ決める
            if (!gM.Auto.HasStopPosDecided)
            {
                // 停止順の設定
                gM.Auto.CurrentStopOrder = gM.OptionSave.AutoOptionData.CurrentStopOrder;

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.Lots.GetCurrentFlag();
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)StopOrderID.First];
                autoAICondition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                autoAICondition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
                autoAICondition.BigChanceGames = gM.Bonus.GetRemainingBigGames();
                autoAICondition.RemainingJacIn = gM.Bonus.GetRemainingJacIn();
                autoAICondition.BetAmount = gM.Medal.LastBetAmount;

                gM.Auto.SetAutoStopPos(autoAICondition);
            }

            // すべてのリールが止まっていたら払い出し処理をする
            if (gM.Reel.GetIsReelFinished())
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.PayoutState);
            }
            // オート中は指定した押し順で押すようにする
            else
            {
                AutoStopBehavior();
            }
        }

        // プレイヤー操作時の挙動
        void PlayerControl()
        {
            // 何も入力が入っていなければ実行
            if (!hasInput)
            {
                // ボタンごとのリールを停止させる
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
            // 前回のフレームで入力があった場合は入力がなくなるまで待機
            else if (hasInput)
            {
                if (!Input.anyKey)
                {
                    hasInput = false;
                }
            }
        }

        // リール停止の演出
        void StopReelSound()
        {
            ReelStoppedEffectCondition condition = new ReelStoppedEffectCondition();
            condition.StoppedReelCount = gM.Reel.GetStoppedCount();
            condition.RiichiBigColor = gM.Reel.GetBigLinedUpCount(gM.Medal.LastBetAmount, 2);
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartReelStopEffect(condition);
        }

        // オート制御
        void AutoStopBehavior()
        {
            // 超高速オートなら即座に止めて払い出し状態へ
            if (gM.Auto.CurrentSpeed == AutoSpeedName.Quick)
            {
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.First],
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.First]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.Second],
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Second]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.Third],
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Third]]);
            }
            else
            {
                // オート速度が高速または通常なら規定位置になったときに停止
                if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.First]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.First]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.First]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.First]);
                    }
                }
                else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Second]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]);
                    }
                }
                else if (gM.Reel.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]) == ReelStatus.Spinning)
                {
                    if (gM.Reel.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Third]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]);
                    }
                }
            }
        }

        // 強制停止
        void ReelForcedStop()
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
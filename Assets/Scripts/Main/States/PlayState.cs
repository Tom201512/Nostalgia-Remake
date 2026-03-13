using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Reels;

namespace ReelSpinGame_State.PlayingState
{
    // 遊技中ステート
    public class PlayingState : IGameStatement
    {
        private GameManager gM;                             // ゲームマネージャ
        private InputManager inputManager;                  // 入力マネージャー

        public PlayingState(GameManager gameManager, InputManager inputManager)
        {
            gM = gameManager;
            this.inputManager = inputManager;
        }

        public void StateStart()
        {
            // 入力処理の登録
            inputManager.ActionTriggeredEvent += OnActionTriggered;

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
            // オート処理
            if (gM.Auto.HasAuto)
            {
                AutoControl();
            }

            // リールがすべて停止したら払い出し処理に移行できるかチェック
            if(gM.Reel.GetIsReelFinished())
            {
                // オート時は自動移行
                if(gM.Auto.HasAuto)
                {
                    gM.MainFlow.StateManager.ChangeState(gM.MainFlow.PayoutState);
                }
                // 手動時は入力がなくなったときに移行する(または強制終了発動)
                else if (!inputManager.isKeyHeld || gM.Reel.HasForceStop())
                {
                    gM.MainFlow.StateManager.ChangeState(gM.MainFlow.PayoutState);
                }
            }
            // 強制停止が発動した場合
            else if (gM.Reel.HasForceStop())
            {
                ReelForcedStop();
            }
        }

        public void StateEnd()
        {
            // 入力処理の解除
            inputManager.ActionTriggeredEvent -= OnActionTriggered;

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

                gM.Auto.SetAutoStopPos(autoAICondition, gM.OptionSave.AutoOptionData.StopPosLockData);
            }

            AutoStopBehavior();
        }

        // プレイヤー操作時の挙動
        void PlayerControl(InputManager.ControlKeys controlKey)
        {
            // ボタンごとのリールを停止させる
            if (gM.Reel.GetIsReelWorking())
            {
                switch (controlKey)
                {
                    case InputManager.ControlKeys.StopLeft:
                        StopReel(ReelID.ReelLeft);
                        break;
                    case InputManager.ControlKeys.StopMiddle:
                        StopReel(ReelID.ReelMiddle);
                        break;
                    case InputManager.ControlKeys.StopRight:
                        StopReel(ReelID.ReelRight);
                        break;
                }
            }
        }

        // リール停止の演出
        void StopReelSound()
        {
            ReelStoppedEffectCondition condition = new ReelStoppedEffectCondition();
            condition.StoppedReelCount = gM.Reel.GetStoppedCount();
            condition.RiichiBigType = gM.Reel.GetBigLinedUpCount(gM.Medal.LastBetAmount, 2);
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

        // 入力に応じた処理を行う
        void OnActionTriggered(InputManager.ControlKeys controlKey)
        {
            // オートがある場合
            if (gM.Auto.HasAuto)
            {
                AutoControl();
            }
            // それ以外はプレイヤー操作を受け付ける
            else if (!gM.Reel.HasForceStop())
            {
                PlayerControl(controlKey);
            }
        }
    }
}
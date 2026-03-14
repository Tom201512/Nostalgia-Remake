using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Flag;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using ReelSpinGame_Reel;
using ReelSpinGame_Reel.Table;

namespace ReelSpinGame_State.PlayingState
{
    // 遊技中ステート
    public class PlayingState : IGameStatement
    {
        const int RiichiCount = 2;      // リーチのみなす数

        private GameManager gM;                             // ゲームマネージャ

        public PlayingState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            // 入力処理の登録
            gM.InputManager.ActionTriggeredEvent += OnActionTriggered;

            // 強制役でランダム数値が使われていれば使う
            if (gM.Option.GetForceFlagSelectID() != -1 && gM.Option.GetForceFlagRandomID() != 0)
            {
                gM.ReelManager.SetForceRandomValue(gM.Option.GetForceFlagRandomID());
            }

            // 強制フラグ設定のリセット
            gM.Option.ResetForceFlagSetting();
            // リール始動
            gM.ReelManager.StartReels(gM.BonusManager.CurrentBonusStatus, gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            // ボーナス中のランプ処理
            gM.BonusManager.UpdateSegments();

            // レバーオン演出開始
            LeverOnEffectCondition condition = new LeverOnEffectCondition();
            condition.Flag = gM.FlagManager.CurrentFlag;
            condition.HoldingBonus = gM.BonusManager.HoldingBonusID;
            condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
            gM.Effect.StartLeverOnEffect(condition);

            // リール停止時に音を鳴らすよう変更
            gM.ReelManager.ReelStoppedEvent += OnReelStopped;
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.MedalManager);
        }

        public void StateUpdate()
        {
            // オート処理
            if (gM.Auto.HasAuto)
            {
                AutoControl();
            }

            // リールがすべて停止したら払い出し処理に移行できるかチェック
            if(gM.ReelManager.IsReelFinished)
			{
                // オート時は自動移行
                if(gM.Auto.HasAuto)
                {
                    gM.MainFlow.StateManager.ChangeState(gM.MainFlow.PayoutState);
                }
                // 手動時は入力がなくなったときに移行する(または強制終了発動)
                else if (!gM.InputManager.isKeyHeld || gM.ReelManager.HasForceStop)
                {
                    gM.MainFlow.StateManager.ChangeState(gM.MainFlow.PayoutState);
                }
            }
            // 強制停止が発動した場合
            else if (gM.ReelManager.HasForceStop)
            {
                ReelForcedStop();
            }
        }

        public void StateEnd()
        {
            // 入力処理の解除
            gM.InputManager.ActionTriggeredEvent -= OnActionTriggered;
            // リール停止時の音を外すようにする
            gM.ReelManager.ReelStoppedEvent -= OnReelStopped;
            // 成立時の出目を記録する(ただし表示するのは入賞した後)
            RecordBonusReelStopped();
            // 高速オートが解除されたかチェック
            gM.Auto.CheckFastAutoCancelled();
            // オート残りゲーム数が0になったかチェック
            gM.Auto.DecreaseAutoSpin();
        }

        // リール停止条件データ作成
        private ReelMainCondition MakeReelMainCondition()
        {
            return ReelMainCondition.MakeReelMainCondition(gM.FlagManager.CurrentFlag, gM.BonusManager.HoldingBonusID, 
                gM.MedalManager.LastBetAmount, gM.ReelManager.RandomValue);
        }

        // リール停止時演出データ作成
        private ReelStoppedEffectCondition MakeReelStoppedEffectCondition()
        {
            return ReelStoppedEffectCondition.MakeReelStoppedEffectCondition(gM.ReelManager.StoppedReelCount, 
                gM.ReelManager.GetBigLinedUpCount(gM.MedalManager.LastBetAmount, RiichiCount), gM.BonusManager.CurrentBonusStatus);
        }


        // リール停止
        private void StopReel(ReelID reelID)
        {
            if (gM.ReelManager.CanStopReels && gM.ReelManager.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.ReelManager.StopSelectedReel(reelID, MakeReelMainCondition());
			}
        }

        // 超高速オート時の停止
        private void StopReelQuick(ReelID reelID, int autoStopPos)
        {
            if (gM.ReelManager.CanStopReels && gM.ReelManager.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.ReelManager.StopSelectedReelFast(reelID, MakeReelMainCondition(), autoStopPos);
            }
        }

        // 入力に応じた処理を行う
        private void OnActionTriggered(InputManager.ControlKeys controlKey)
        {
            // オートがある場合
            if (gM.Auto.HasAuto)
            {
                AutoControl();
            }
            // それ以外はプレイヤー操作を受け付ける
            else if (!gM.ReelManager.HasForceStop)
            {
                PlayerControl(controlKey);
            }
        }

        // オート時の挙動
        private void AutoControl()
        {
            // オート停止位置が決まっているかチェック。決まっていなければすぐ決める
            if (!gM.Auto.HasStopPosDecided)
            {
                // 停止順の設定
                gM.Auto.CurrentStopOrder = gM.OptionSave.AutoOptionData.CurrentStopOrder;

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.FlagManager.CurrentFlag;
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)StopOrderID.First];
                autoAICondition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                autoAICondition.HoldingBonus = gM.BonusManager.HoldingBonusID;
                autoAICondition.BigChanceGames = gM.BonusManager.RemainingBigGames;
                autoAICondition.RemainingJacIn = gM.BonusManager.RemainingJacIn;
                autoAICondition.BetAmount = gM.MedalManager.LastBetAmount;

                gM.Auto.SetAutoStopPos(autoAICondition, gM.OptionSave.AutoOptionData.StopPosLockData);
            }

            AutoStopBehavior();
        }

        // プレイヤー操作時の挙動
        private void PlayerControl(InputManager.ControlKeys controlKey)
        {
            // ボタンごとのリールを停止させる
            if (gM.ReelManager.IsReelWorking)
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

        // リール停止時の処理
        private void OnReelStopped(ReelID reelID) => gM.Effect.StartReelStopEffect(MakeReelStoppedEffectCondition());

        // オート制御
        private void AutoStopBehavior()
        {
            // 超高速オートなら即座に止めて払い出し状態へ
            if (gM.Auto.CurrentSpeed == AutoSpeedName.Quick)
            {
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.First], gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.First]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.Second], gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Second]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)StopOrderID.Third], gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Third]]);
            }
            else
            {
                // オート速度が高速または通常なら規定位置になったときに停止
                if (gM.ReelManager.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.First]) == ReelStatus.Spinning)
                {
                    if (gM.ReelManager.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.First]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.First]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.First]);
                    }
                }
                else if (gM.ReelManager.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]) == ReelStatus.Spinning)
                {
                    if (gM.ReelManager.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Second]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.Second]);
                    }
                }
                else if (gM.ReelManager.GetReelStatus(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]) == ReelStatus.Spinning)
                {
                    if (gM.ReelManager.GetReelPushedPos(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]) ==
                        gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)StopOrderID.Third]])
                    {
                        StopReel(gM.Auto.AutoStopOrders[(int)StopOrderID.Third]);
                    }
                }
            }
        }

        // 強制停止
        private void ReelForcedStop()
        {
            // 左->中->右から強制停止
            if (gM.ReelManager.GetReelStatus(ReelID.ReelLeft) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelLeft);
            }
            else if (gM.ReelManager.GetReelStatus(ReelID.ReelMiddle) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelMiddle);
            }
            else if (gM.ReelManager.GetReelStatus(ReelID.ReelRight) != ReelStatus.Stopped)
            {
                StopReel(ReelID.ReelRight);
            }
        }

        // 成立時出目記録
        private void RecordBonusReelStopped()
        {
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone &&
                (gM.FlagManager.CurrentFlag == FlagModel.FlagID.FlagBig || gM.FlagManager.CurrentFlag == FlagModel.FlagID.FlagReg))
            {
                gM.Player.SetBonusHitPos(gM.ReelManager.LastStoppedReelData.LastPos);
                gM.Player.SetBonusPushOrder(gM.ReelManager.LastStoppedReelData.LastStoppedOrder);
                gM.Player.SetBonusHitDelay(gM.ReelManager.LastStoppedReelData.LastReelDelay);
            }
        }
    }
}
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_AutoPlay.AutoManager.AutoStopOrder;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;
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
            // リール始動
            gM.Reel.StartReels(gM.Bonus.GetCurrentBonusStatus(), gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();

            // スタート音再生
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
            gM.Reel.SomeReelStoppedEvent -= StopReelSound;
        }

        // リール停止
        private void StopReel(ReelID reelID)
        {
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.Reel.StopSelectedReel(reelID, gM.Medal.GetLastBetAmount(), gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID());
            }
        }

        // 超高速オート時の停止
        private void StopReelQuick(ReelID reelID, int autoStopPos)
        {
            if (gM.Reel.GetCanStopReels() && gM.Reel.GetReelStatus(reelID) == ReelStatus.Spinning)
            {
                gM.Reel.StopSelectedReelFast(reelID, gM.Medal.GetLastBetAmount(), gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(), autoStopPos);
            }
        }

        // オート時の挙動
        private void AutoControl()
        {
            // オート停止位置が決まっているかチェック。決まっていなければすぐ決める
            if (!gM.Auto.HasStopPosDecided)
            {
                // BIG中の場合、(JAC回数が残り1回, 残りゲーム数が9G以上)ならJACハズシをする
                if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                    gM.Bonus.GetRemainingBigGames() > 8 && gM.Bonus.GetRemainingJacIn() == 1)
                {
                    gM.Auto.SetAutoOrder(AutoStopOrderOptions.RML);
                }
                // ボーナス成立後であれば左押しに固定する
                else if(gM.Bonus.GetHoldingBonusID() != BonusTypeID.BonusNone)
                {
                    gM.Auto.SetAutoOrder(AutoStopOrderOptions.LMR);
                }
                // それ以外はオプションで設定した押し順を使う
                else
                {
                    gM.Auto.SetAutoOrder(gM.OptionSave.AutoOptionData.AutoStopOrdersID);
                }

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.Lots.GetCurrentFlag();
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)First];
                autoAICondition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
                autoAICondition.BigChanceGames = gM.Bonus.GetRemainingBigGames();
                autoAICondition.RemainingJacIn = gM.Bonus.GetRemainingJacIn();
                autoAICondition.BetAmount = gM.Medal.GetLastBetAmount();

                gM.Auto.GetAutoStopPos(autoAICondition);
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

        // リール停止の演出
        void StopReelSound()
        {
            ReelStoppedEffectCondition condition = new ReelStoppedEffectCondition();
            condition.StoppedReelCount = gM.Reel.GetStoppedCount();
            condition.RiichiBigColor = gM.Reel.GetBigLinedUpCount(gM.Medal.GetLastBetAmount(), 2);
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartReelStopEffect(condition);
        }

        // オート制御
        void AutoStopBehavior()
        {
            // 超高速オートなら即座に止めて払い出し状態へ
            if (gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Quick)
            {
                StopReelQuick(gM.Auto.AutoStopOrders[(int)First], 
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)First]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)Second],
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Second]]);
                StopReelQuick(gM.Auto.AutoStopOrders[(int)Third],
                    gM.Auto.AutoStopPos[(int)gM.Auto.AutoStopOrders[(int)Third]]);
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
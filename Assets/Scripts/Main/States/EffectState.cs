using ReelSpinGame_AutoPlay;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using ReelSpinGame_Reels;
using ReelSpinGame_System;

namespace ReelSpinGame_State.LotsState
{
    // 演出ステート
    public class EffectState : IGameStatement
    {
        private GameManager gM;     // ゲームマネージャー

        private bool startPayout;       // 払い出しを始めたか
        private bool finishPayout;      // 払い出しが終わったか

        public EffectState(GameManager gameManager)
        {
            gM = gameManager;
            startPayout = false;
            finishPayout = false;
        }

        public void StateStart()
        {
            finishPayout = false;
            // 高速オートが切れていれば演出に入る
            if (!gM.Auto.HasAuto ||
                (gM.Auto.HasAuto && gM.Auto.CurrentSpeed == AutoSpeedName.Normal))
            {
                // BGM, SEのミュート解除
                gM.Effect.ChangeSoundSettingByAuto(gM.Auto.HasAuto, gM.Auto.CurrentSpeed);

                // 演出開始
                BeforePayoutEffectCondition condition = new BeforePayoutEffectCondition();
                condition.Flag = gM.FlagManager.CurrentFlag;
                condition.HoldingBonus = gM.BonusManager.HoldingBonusID;
                condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                condition.LastLeftStoppedPos = gM.ReelManager.GetLastPushedLowerPos((int)ReelID.ReelLeft);
                gM.Effect.StartBeforePayoutEffect(condition);
            }
            else
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }

        public void StateUpdate()
        {
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.MedalManager);
            // 払い出し前の演出を待つ
            if (!gM.Effect.GetHasBeforeEffectActivating())
            {
                // 払い出し開始
                if (!startPayout && !finishPayout)
                {
                    // セグメントを更新する
                    if(gM.MedalManager.RemainingPayout > 0)
                    {
                        gM.MedalManager.StartPayoutSegmentUpdate();
                    }

                    PayoutEffectCondition condition =
                        new PayoutEffectCondition(gM.PayoutManager.LastPayoutResult, gM.ReelManager.GetLastStoppedReelData());

                    condition.Flag = gM.FlagManager.CurrentFlag;
                    condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                    gM.Effect.StartPayoutEffect(condition);
                    startPayout = true;
                }

                // 払い出しが終わるまで待機
                else if (startPayout && !finishPayout && !gM.Effect.GetPayoutEffectActivating())
                {
                    // 払い出し後演出を始める
                    AfterPayoutEffectCondition condition = new AfterPayoutEffectCondition(gM.PayoutManager.LastPayoutResult);

                    condition.HasBonusStarted = gM.BonusManager.HasBonusStarted;
                    condition.HasBonusFinished = gM.BonusManager.HasBonusFinished;
                    condition.BigType = gM.BonusManager.BigChanceType;
                    condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                    gM.Effect.StartAfterPayoutEffect(condition);
                    finishPayout = true;
                }
                // 払い出し後演出が終わったらメダル投入へ移行
                else if (finishPayout && !gM.Effect.GetAfterPayoutEffectActivating())
                {
                    // 打ち止め状態を確認
                    CheckReachedLimitSpins();
                }
            }
        }

        public void StateEnd()
        {
            // オートが終了している場合は設定画面からの設定を受け付けられるようにする
            if (!gM.Auto.HasAuto)
            {
                gM.Option.ToggleOptionLock(false);
            }

            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.MedalManager);
            // ボーナス演出更新
            BonusEffectUpdate();

            startPayout = false;
            finishPayout = false;
            if (gM.BonusManager.HasBonusFinished)
            {
                gM.BonusManager.ResetBigType();
            }
            gM.BonusManager.ResetBonusStarted();
            gM.BonusManager.ResetBonusFinished();
        }

        // ボーナス関連の演出更新
        void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.BonusManager.UpdateSegments();
            // ボーナス中のBGM処理
            BonusEffectCondition condition = new BonusEffectCondition();
            condition.BigType = gM.BonusManager.BigChanceType;
            condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
            gM.Effect.StartBonusEffect(condition);
        }

        // 打ち止め回数に達したかチェック
        void CheckReachedLimitSpins()
        {
            // 通常時であり現在トータルゲーム数が規定数に達したら操作不能にする
            if (gM.Player.TotalGames >= PlayerDatabase.MaximumTotalGames &&
                gM.BonusManager.CurrentBonusStatus == ReelSpinGame_Bonus.BonusModel.BonusStatus.BonusNone)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.LimitReachedState);
            }
            else
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }
    }
}

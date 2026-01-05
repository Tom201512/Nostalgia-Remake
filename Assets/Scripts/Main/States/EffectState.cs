using ReelSpinGame_AutoPlay;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Reels;

namespace ReelSpinGame_State.LotsState
{
    // 演出ステート
    public class EffectState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }   // ステート名
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
                condition.Flag = gM.Lots.GetCurrentFlag();
                condition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
                condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                condition.LastLeftStoppedPos = gM.Reel.GetLastPushedLowerPos((int)ReelID.ReelLeft);
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
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
            // 払い出し前の演出を待つ
            if (!gM.Effect.GetHasBeforeEffectActivating())
            {
                // 払い出し開始
                if (!startPayout && !finishPayout)
                {
                    gM.Medal.ChangeSegmentUpdate(true);
                    PayoutEffectCondition condition =
                        new PayoutEffectCondition(gM.Payout.LastPayoutResult, gM.Reel.GetLastStoppedReelData());

                    condition.Flag = gM.Lots.GetCurrentFlag();
                    condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                    gM.Effect.StartPayoutEffect(condition);
                    startPayout = true;
                }

                // 払い出しが終わるまで待機
                else if (startPayout && !finishPayout && !gM.Effect.GetPayoutEffectActivating())
                {
                    // クレジット、払い出し枚数セグメントの処理終了
                    gM.Medal.ChangeSegmentUpdate(false);
                    // 払い出し後演出を始める
                    AfterPayoutEffectCondition condition = new AfterPayoutEffectCondition(gM.Payout.LastPayoutResult);

                    condition.HasBonusStarted = gM.Bonus.GetHasBonusStarted();
                    condition.HasBonusFinished = gM.Bonus.GetHasBonusFinished();
                    condition.BigType = gM.Bonus.GetBigChanceType();
                    condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                    gM.Effect.StartAfterPayoutEffect(condition);
                    finishPayout = true;
                }
                // 払い出し後演出が終わったらメダル投入へ移行
                else if (finishPayout && !gM.Effect.GetAfterPayoutEffectActivating())
                {
                    gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
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
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
            // ボーナス演出更新
            BonusEffectUpdate();

            startPayout = false;
            finishPayout = false;
            if (gM.Bonus.GetHasBonusFinished())
            {
                gM.Bonus.ResetBigType();
            }
            gM.Bonus.SetHasBonusStarted(false);
            gM.Bonus.SetHasBonusFinished(false);
        }

        // ボーナス関連の演出更新
        void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // ボーナス中のBGM処理
            BonusEffectCondition condition = new BonusEffectCondition();
            condition.BigType = gM.Bonus.GetBigChanceType();
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartBonusEffect(condition);
        }
    }
}

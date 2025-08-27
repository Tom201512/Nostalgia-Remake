using ReelSpinGame_Interface;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_State.LotsState
{
    public class EffectState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gM;

        // 払い出しが終わったか
        bool finishPayout;

        // コンストラクタ
        public EffectState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Effect;
            gM = gameManager;
            finishPayout = false;
        }

        public void StateStart()
        {
            finishPayout = false;
            // 高速オートが切れていれば演出に入る
            if (!gM.Auto.HasAuto ||
                (gM.Auto.HasAuto && gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal))
            {
                EnableSounds();
                // ビタ箇所を押したかチェック
                bool hasBita = gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 10 || gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 16;

                // 演出開始
                gM.Effect.StartBeforePayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetHoldingBonusID(),
                    gM.Bonus.GetCurrentBonusStatus(), hasBita);
            }
            else
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }

        public void StateUpdate()
        {
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
            // 払い出し前の演出を待つ
            if(!gM.Effect.HasBeforePayoutEffect)
            {
                // 払い出し開始
                if(!gM.Effect.HasPayoutEffectStart)
                {
                    gM.Medal.ChangeSegmentUpdate(true);
                    gM.Effect.StartPayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus(),
                        gM.Reel.GetPayoutResultData(), gM.Reel.GetPayoutResultData().PayoutLines);
                }

                // 払い出しが終わるまで待機
                if (gM.Medal.GetRemainingPayout() == 0 && !finishPayout)
                {
                    // クレジット、払い出し枚数セグメントの処理終了
                    gM.Medal.ChangeSegmentUpdate(false);
                    // ループしている音を停止
                    gM.Effect.StopLoopSound();
                    // 払い出し後演出を始める
                    gM.Effect.StartAfterPayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus());
                    finishPayout = true;
                }
                else if(finishPayout)
                {
                    // 払い出し後演出が終わったらメダル投入へ移行
                    if (!gM.Effect.HasAfterPayoutEffect)
                    {
                        gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
                    }
                }
            }
        }

        public void StateEnd()
        {
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);
            // ボーナス演出更新
            BonusEffectUpdate();
        }

        // ボーナス関連の演出更新
        private void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // ボーナス中のBGM処理
            gM.Effect.PlayBonusBGM(gM.Bonus.GetCurrentBonusStatus(), false);
        }

        // 高速オート処理終了時のSE,BGMの再生
        private void EnableSounds()
        {
            // BGM, SEのミュート解除
            gM.Effect.ChangeSoundSettingByAuto(gM.Auto.HasAuto, gM.Auto.AutoSpeedID);
        }
    }
}

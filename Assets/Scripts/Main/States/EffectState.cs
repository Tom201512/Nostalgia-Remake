using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using UnityEngine;
using System.Collections;

namespace ReelSpinGame_State.LotsState
{
    public class EffectState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gM;

        // コンストラクタ
        public EffectState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Effect;
            gM = gameManager;
        }

        public void StateStart()
        {
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
                    gM.Medal.StartSegmentUpdate();
                    gM.Effect.StartPayoutEffect(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus(),
                        gM.Reel.GetPayoutResultData(), gM.Reel.GetPayoutResultData().PayoutLines);
                }

                // 払い出しが終わるまで待機
                if (gM.Medal.GetRemainingPayout() == 0 && !gM.Effect.GetHasFlashWait())
                {
                    // ループしている音を停止
                    gM.Effect.StopLoopSound();
                    // 払い出し後演出を始める
                    Debug.Log("StartAfterEffect");
                    gM.Effect.StartAfterPayoutEffect();

                    // 全ての演出が終わったらメダル投入へ移行
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

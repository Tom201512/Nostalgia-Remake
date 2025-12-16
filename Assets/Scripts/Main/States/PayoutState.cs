using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_AutoPlay.AutoManager;
using UnityEngine;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        // const

        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }
        // ゲームマネージャ
        private GameManager gM;

        // コンストラクタ
        public PayoutState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Payout;
            gM = gameManager;

        }
        public void StateStart()
        {
            // 成立時の出目を記録する(ただし表示するのは入賞した後)
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                (gM.Lots.GetCurrentFlag() == FlagID.FlagBig || gM.Lots.GetCurrentFlag() == FlagID.FlagReg))
            {
                gM.Player.SetBonusHitPos(gM.Reel.GetLastStoppedReelData().LastPos);
                gM.Player.SetBonusPushOrder(gM.Reel.GetLastStoppedReelData().LastPushOrder);
                gM.Player.SetBonusHitDelay(gM.Reel.GetLastStoppedReelData().LastReelDelay);
            }

            // 高速オートが解除されたかチェック
            gM.Auto.CheckFastAutoCancelled();

            // 払い出し確認
            StartCheckPayout(gM.Medal.GetLastBetAmount());

            // 払い出しの結果をデータに反映
            PayoutUpdate();

            // 通常時に小役が当選していたら増加させる
            // リプレイでは増やさない(0増加)
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                if (gM.Reel.GetPayoutResultData().Payout > 0 || gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
                {
                    gM.Lots.IncreaseCounter(gM.Reel.GetPayoutResultData().Payout);
                }
            }

            // 状態遷移
            CheckGameModeStatusChange();

            // 連チャン区間の処理
            // 50Gを迎えた場合は連チャン区間を終了させる(但しボーナス非成立時のみ)
            if (gM.Player.CurrentGames == MaxZoneGames &&
                gM.Bonus.GetHoldingBonusID() == BonusTypeID.BonusNone)
            {
                gM.Bonus.ResetZonePayout();
            }

            // 払い出し開始
            // オートがあり速度が高速以上なら払い出し演出はカット
            gM.Medal.StartPayout(gM.Reel.GetPayoutResultData().Payout, 
                gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);

            // リプレイ処理
            UpdateReplay();

            // オート残りゲーム数が0になったかチェック(ボーナス中以外)
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Auto.CheckRemainingAuto();
            }

            // 差枚数のカウント(通常時のみ)
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Player.PlayerMedalData.CountCurrentSlumpGraph();
            }

            // 小役成立回数の記録
            gM.Player.PlayerAnalyticsData.IncreaseHitCountByFlag(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus());

            // 小役入賞回数の記録(払い出しがあれば)
            if(gM.Reel.GetPayoutResultData().Payout > 0 || gM.Medal.GetHasReplay())
            {
                gM.Player.PlayerAnalyticsData.IncreaseLineUpCountByFlag(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus());
            }

            // JACハズシの記録
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames && gM.Lots.GetCurrentFlag() == FlagID.FlagReplayJacIn)
            {
                gM.Player.PlayerAnalyticsData.CountJacAvoidCounts(gM.Reel.GetLastPushedLowerPos((int)ReelID.ReelLeft), gM.Reel.GetRandomValue());
            }

            // セーブ処理
            SaveData();

            // オプション設定の反映
            gM.Option.SetForceFlagSetting(gM.Bonus.GetCurrentBonusStatus(), gM.Bonus.GetHoldingBonusID());

            // 演出処理へ
            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.EffectState);
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {

        }

        // データのセーブ
        private void SaveData()
        {
            // プレイヤー情報記録
            gM.PlayerSave.RecordPlayerSave(gM.Player.MakeSaveData());
            // メダル情報記録
            gM.PlayerSave.RecordMedalSave(gM.Medal.MakeSaveData());
            // フラグ情報記録
            gM.PlayerSave.RecordFlagCounter(gM.Lots.GetCounter());
            // 最終リール位置を記録
            gM.PlayerSave.RecordReelPos(gM.Reel.GetLastStoppedReelData().LastPos);
            // ボーナス情報記録
            gM.PlayerSave.RecordBonusData(gM.Bonus.MakeSaveData());
            Debug.Log("Saved");
        }

        // 払い出し確認
        private void StartCheckPayout(int betAmount)
        {
            if (!gM.Reel.GetIsReelWorking())
            {
                gM.Reel.StartCheckPayout(betAmount);
            }
        }

        // リプレイ処理
        private void UpdateReplay()
        {
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                    gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                // 最後に賭けた枚数をOUTに反映
                gM.Player.PlayerMedalData.IncreaseOutMedal(gM.Medal.GetLastBetAmount());
                gM.Medal.EnableReplay();
            }
            else if (gM.Medal.GetHasReplay())
            {
                gM.Medal.DisableReplay();
            }
        }

        // 払い出し処理
        private void PayoutUpdate()
        {
            // プレイヤーメダルの増加、OUT枚数の増加(データのみ変更)
            gM.Player.PlayerMedalData.IncreasePlayerMedal(gM.Reel.GetPayoutResultData().Payout);
            gM.Player.PlayerMedalData.IncreaseOutMedal(gM.Reel.GetPayoutResultData().Payout);

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.ChangeBonusPayout(gM.Reel.GetPayoutResultData().Payout);
                gM.Player.ChangeLastBonusPayout(gM.Bonus.GetCurrentBonusPayout());
            }

            // 払い出し時にボーナスが揃っていればボーナス払い出しを増やす
            if(gM.Reel.GetPayoutResultData().BonusID != 0)
            {
                gM.Bonus.ChangeBonusPayout(gM.Reel.GetPayoutResultData().Payout);
                gM.Player.ChangeLastBonusPayout(gM.Bonus.GetCurrentBonusPayout());
            }

            // ゾーン区間(50G)にいる間はその払い出しを計算
            if (gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(gM.Reel.GetPayoutResultData().Payout);
            }
        }

        // ボーナス状態によるデータ変更
        private void BonusStatusUpdate()
        {
            // ビッグチャンス中に移行した場合
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotMode.BigBonus);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gM.Medal.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }

            // ボーナスゲーム中に移行した場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gM.Medal.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotMode.JacGame);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }

            // 通常時に移行した場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotMode.Normal);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                gM.Medal.ChangeMaxBet(3);

                // 終了ファンファーレ再生のフラグを立てる(通常オートで回している場合)
                if (!gM.Auto.HasAuto ||
                 (gM.Auto.HasAuto && gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal))
                {
                    gM.Effect.SetHasBonusFinished();
                }
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // リールから揃ったボーナス図柄の色を得る
            BigColor color = gM.Reel.GetBigLinedUpCount(gM.Medal.GetLastBetAmount(), 3);

            // ビッグチャンスの場合
            if (gM.Reel.GetPayoutResultData().BonusID == (int)BonusTypeID.BonusBIG)
            {
                gM.Bonus.StartBigChance(color);
                gM.Player.SetLastBigChanceColor(color);
            }

            // ボーナスゲームの場合
            else if (gM.Reel.GetPayoutResultData().BonusID == (int)BonusTypeID.BonusREG)
            {
                gM.Bonus.StartBonusGame();
            }

            // 15枚の払い出しを記録
            gM.Bonus.ChangeBonusPayout(gM.Reel.GetPayoutResultData().Payout);
            // 連チャン区間中なら枚数記録
            if(gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(gM.Reel.GetPayoutResultData().Payout);
            }
            // カウンタリセット
            gM.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            gM.Player.SetLastBonusStart();

            // ファンファーレ再生のフラグを立てる(通常オートで回している場合)
            if (!gM.Auto.HasAuto ||
             (gM.Auto.HasAuto && gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal))
            {
                gM.Effect.SetHasBonusStarted();
            }
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // ボーナス未成立でいずれかのボーナスが成立した場合はストック
            if (gM.Bonus.GetHoldingBonusID() == BonusTypeID.BonusNone)
            {
                // BIG
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagBig)
                {
                    gM.Bonus.SetBonusStock(BonusTypeID.BonusBIG);
                }
                // REG
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagReg)
                {
                    gM.Bonus.SetBonusStock(BonusTypeID.BonusREG);
                }
            }
        }

        // 各ゲームモード時の状態チェック
        private void CheckGameModeStatusChange()
        {
            // 通常時なら
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                // ボーナスがあればボーナス開始
                if (gM.Reel.GetPayoutResultData().BonusID != (int)BonusTypeID.BonusNone)
                {
                    StartBonus();
                    BonusStatusUpdate();
                    gM.Effect.SetBigColor(gM.Bonus.GetBigChanceColor());
                }
                // 取りこぼした場合はストックさせる
                else
                {
                    StockBonus();
                }

                // オートがあり、条件がボーナス成立なら終了判定
                if (gM.Auto.HasAuto)
                {
                    gM.Auto.CheckAutoEndByBonus(gM.Reel.GetPayoutResultData().BonusID);
                }
            }
            // それ以外(すでにボーナスが当選している場合)
            else
            {
                if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
                {
                    gM.Bonus.CheckBigGameStatus(gM.Reel.GetPayoutResultData().IsReplayOrJacIn);
                }
                else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
                {
                    gM.Bonus.CheckBonusGameStatus(gM.Reel.GetPayoutResultData().Payout > 0);

                }
                
                BonusStatusUpdate();

                // オートがあり終了条件がボーナス終了時の場合はここで判定する
                if(gM.Auto.HasAuto)
                {
                    gM.Auto.CheckAutoEndByBonusFinish((int)gM.Bonus.GetCurrentBonusStatus());
                }
            }
        }
    }
}
using ReelSpinGame_Interface;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Payout.PayoutManager;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        // const

        // var
        public MainGameFlow.GameStates State { get; }       // このゲームの状態
        private GameManager gM;                             // ゲームマネージャ

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

            gM.Auto.CheckFastAutoCancelled(); // 高速オートが解除されたかチェック

            gM.Payout.CheckPayouts(gM.Medal.GetLastBetAmount(), gM.Reel.GetLastStoppedReelData()); // 払い出し確認
            PayoutUpdate(); // 払い出しの結果をデータに反映

            // 小役カウンタの増減(通常時のみ)
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                if (gM.Payout.LastPayoutResult.Payout > 0 || gM.Payout.LastPayoutResult.IsReplayOrJacIn)
                {
                    gM.Lots.IncreaseCounter(gM.Payout.LastPayoutResult.Payout);
                }
            }

            CheckGameModeStatusChange(); // 状態遷移

            // 払い出し開始
            gM.Medal.StartPayout(gM.Payout.LastPayoutResult.Payout, 
                gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);

            UpdateReplay(); // リプレイ処理

            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Auto.CheckRemainingAuto(); // オート残りゲーム数が0になったかチェック(ボーナス中以外)
                gM.Player.PlayerMedalData.CountCurrentSlumpGraph(); // 差枚数のカウント(通常時のみ)
            }

            // 小役成立回数の記録
            gM.Player.PlayerAnalyticsData.IncreaseHitCountByFlag(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus());

            // 小役入賞回数の記録(払い出しがあれば)
            if(gM.Payout.LastPayoutResult.Payout > 0 || gM.Medal.GetHasReplay())
            {
                gM.Player.PlayerAnalyticsData.IncreaseLineUpCountByFlag(gM.Lots.GetCurrentFlag(), gM.Bonus.GetCurrentBonusStatus());
            }

            // JACハズシの記録
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames && gM.Lots.GetCurrentFlag() == FlagID.FlagReplayJacIn)
            {
                gM.Player.PlayerAnalyticsData.CountJacAvoidCounts(gM.Reel.GetLastPushedLowerPos((int)ReelID.ReelLeft), gM.Reel.GetRandomValue());
            }

            SaveData(); // セーブ処理

            // オプション設定の反映
            gM.Option.SetForceFlagSetting(gM.Bonus.GetCurrentBonusStatus(), gM.Bonus.GetHoldingBonusID());
            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.EffectState);            // 演出処理へ
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {

        }

        // データのセーブ
        void SaveData()
        {
            gM.PlayerSave.RecordPlayerSave(gM.Player.MakeSaveData());
            gM.PlayerSave.RecordMedalSave(gM.Medal.MakeSaveData());
            gM.PlayerSave.RecordFlagCounter(gM.Lots.GetCounter());
            gM.PlayerSave.RecordReelPos(gM.Reel.GetLastStoppedReelData().LastPos);
            gM.PlayerSave.RecordBonusData(gM.Bonus.MakeSaveData());
        }

        // リプレイ処理
        void UpdateReplay()
        {
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                    gM.Payout.LastPayoutResult.IsReplayOrJacIn)
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
        void PayoutUpdate()
        {
            // プレイヤーメダルの増加、OUT枚数の増加(データのみ変更)
            gM.Player.PlayerMedalData.IncreasePlayerMedal(gM.Payout.LastPayoutResult.Payout);
            gM.Player.PlayerMedalData.IncreaseOutMedal(gM.Payout.LastPayoutResult.Payout);

            // ボーナス中なら各ボーナスの払い出しを増やす
            // また、払い出し時にボーナスが揃っていればボーナス払い出しを増やす
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone ||
                gM.Payout.LastPayoutResult.BonusID != 0)
            {
                gM.Bonus.ChangeBonusPayout(gM.Payout.LastPayoutResult.Payout);
                gM.Player.ChangeLastBonusPayout(gM.Bonus.GetCurrentBonusPayout());
            }
            // ゾーン区間(50G)にいる間はその払い出しを計算
            if (gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(gM.Payout.LastPayoutResult.Payout);
            }
        }

        //　ボーナス開始
        void StartBonus()
        {
            // リールから揃ったボーナス図柄の色を得る
            gM.Bonus.ResetBigColor();
            BigColor color = gM.Reel.GetBigLinedUpCount(gM.Medal.GetLastBetAmount(), 3);

            // ビッグチャンスの場合
            if (gM.Payout.LastPayoutResult.BonusID == (int)BonusTypeID.BonusBIG)
            {
                gM.Bonus.StartBigChance(color);
                gM.Player.SetLastBigChanceColor(color);
            }

            // ボーナスゲームの場合
            else if (gM.Payout.LastPayoutResult.BonusID == (int)BonusTypeID.BonusREG)
            {
                gM.Bonus.StartBonusGame();
            }

            // 15枚の払い出しを記録
            gM.Bonus.ChangeBonusPayout(gM.Payout.LastPayoutResult.Payout);
            // 連チャン区間中なら枚数記録
            if(gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(gM.Payout.LastPayoutResult.Payout);
            }
            // カウンタリセット
            gM.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            gM.Player.SetLastBonusStart();
            // ボーナス開始を記録
            gM.Bonus.SetHasBonusStarted(true);
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // ボーナス未成立でいずれかのボーナスが成立した場合はストック
            if (gM.Bonus.GetHoldingBonusID() == BonusTypeID.BonusNone)
            {
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagBig)
                {
                    gM.Bonus.SetBonusStock(BonusTypeID.BonusBIG);
                }
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagReg)
                {
                    gM.Bonus.SetBonusStock(BonusTypeID.BonusREG);
                }
            }
        }

        // 各ゲームモード時の状態チェック
        private void CheckGameModeStatusChange()
        {
            // 通常時
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                // ボーナスがあればボーナス開始
                if (gM.Payout.LastPayoutResult.BonusID != (int)BonusTypeID.BonusNone)
                {
                    StartBonus();
                    BonusStatusUpdate();
                }
                // 取りこぼした場合はストックさせる
                else
                {
                    StockBonus();
                }

                // オートがあり、条件がボーナス成立なら終了判定
                if (gM.Auto.HasAuto)
                {
                    gM.Auto.CheckAutoEndByBonus(gM.Payout.LastPayoutResult.BonusID);
                }
            }
            // それ以外(すでにボーナスが当選している場合)
            else
            {
                if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
                {
                    gM.Bonus.CheckBigGameStatus(gM.Payout.LastPayoutResult.IsReplayOrJacIn);
                }
                else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
                {
                    gM.Bonus.CheckBonusGameStatus(gM.Payout.LastPayoutResult.Payout > 0);

                }
                
                // オートがあり終了条件がボーナス終了時の場合はここで判定する
                if(gM.Auto.HasAuto)
                {
                    gM.Auto.CheckAutoEndByBonusFinish((int)gM.Bonus.GetCurrentBonusStatus());
                }

                BonusStatusUpdate();        // ボーナス状態更新
            }

            // 連チャン区間の処理
            // 50Gを迎えた場合は連チャン区間を終了させる(但しボーナス非成立時のみ)
            if (gM.Player.CurrentGames == MaxZoneGames &&
                gM.Bonus.GetHoldingBonusID() == BonusTypeID.BonusNone)
            {
                gM.Bonus.ResetZonePayout();
            }
        }

        // ボーナス状態によるデータ変更
        void BonusStatusUpdate()
        {
            // ビッグチャンス中に移行した場合
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotMode.BigBonus);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gM.Medal.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }
            // ボーナスゲーム中に移行した場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gM.Medal.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotMode.JacGame);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }
            // 通常時に移行した場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotMode.Normal);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                gM.Medal.ChangeMaxBet(3);

                // ボーナス終了フラグを立てる
                gM.Bonus.SetHasBonusFinished(true);
            }
        }
    }
}
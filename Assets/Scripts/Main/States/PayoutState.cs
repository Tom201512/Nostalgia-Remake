using ReelSpinGame_AutoPlay;
using ReelSpinGame_Interface;
using ReelSpinGame_Lots;
using ReelSpinGame_Medal;
using ReelSpinGame_Reels;
using static ReelSpinGame_Bonus.BonusModel;
using static ReelSpinGame_Payout.PayoutManager;

namespace ReelSpinGame_State.PayoutState
{
    // 払い出しステート
    public class PayoutState : IGameStatement
    {
        private GameManager gM;                 // ゲームマネージャ
        private MedalManager medalManager;      // メダル管理

        public PayoutState(GameManager gameManager, MedalManager medalManager)
        {
            gM = gameManager;
            this.medalManager = medalManager;
        }
        public void StateStart()
        {
            // 払い出し確認
            gM.Payout.CheckPayouts(medalManager.LastBetAmount, gM.Reel.GetLastStoppedReelData());
            // 変更前のボーナス状態を記録
            gM.BonusManager.SetPreviousBonusStatus();
            // ボーナス開始をチェック
            CheckBonusStart();
            // 払い出し処理開始
            PayoutUpdate();
            // 小役成立回数のカウント
            CountSymbol();
            // ボーナス、連チャン区間の終了チェック
            CheckBonusEnd();
            CheckZoneEnd();
            // 払い出し開始
            medalManager.StartPayout(gM.Payout.LastPayoutResult.Payout, gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            // リプレイ処理
            UpdateReplay();

            // 通常時のみの処理
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusNone)
            {
                // 差枚数のカウント
                gM.Player.PlayerMedalData.CountCurrentSlumpGraph();
                // 回転数が打ち止め規定数に達していたらオート終了
                gM.Auto.CheckAutoEndByLimitReached(gM.Player.TotalGames);
            }

            // セーブ処理
            SaveData();
            // オプション設定の反映
            gM.Option.SetForceFlagSetting(gM.BonusManager.CurrentBonusStatus, gM.BonusManager.HoldingBonusID);
            // 演出処理へ
            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.EffectState);
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
            gM.PlayerSave.RecordPlayerSave(gM.Player.MakeSaveData());
            gM.PlayerSave.RecordMedalSave(medalManager.MakeSaveData());
            gM.PlayerSave.RecordFlagCounter(gM.Lots.GetCounter());
            gM.PlayerSave.RecordReelPos(gM.Reel.GetLastStoppedReelData().LastPos);
            gM.PlayerSave.RecordBonusData(gM.BonusManager.MakeSaveData());
        }

        // リプレイ処理
        private void UpdateReplay()
        {
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusNone &&
                    gM.Payout.LastPayoutResult.IsReplayOrJacIn)
            {
                // 最後に賭けた枚数をOUTに反映
                gM.Player.PlayerMedalData.IncreaseOutMedal(medalManager.LastBetAmount);
                medalManager.EnableReplay();
            }
            else if (medalManager.HasReplay)
            {
                medalManager.DisableReplay();
            }
        }

        // 払い出し処理
        private void PayoutUpdate()
        {
            // プレイヤーメダルの増加、OUT枚数の増加(データのみ変更)
            gM.Player.PlayerMedalData.IncreasePlayerMedal(gM.Payout.LastPayoutResult.Payout);
            gM.Player.PlayerMedalData.IncreaseOutMedal(gM.Payout.LastPayoutResult.Payout);

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gM.BonusManager.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                gM.BonusManager.ChangeBonusPayout(gM.Payout.LastPayoutResult.Payout);
                gM.Player.ChangeLastBonusPayout(gM.BonusManager.CurrentBonusPayout);
            }
            // ゾーン区間(50G)にいる間はその払い出しを計算
            if (gM.BonusManager.HasZone)
            {
                gM.BonusManager.ChangeZonePayout(gM.Payout.LastPayoutResult.Payout);
            }
        }

        // ボーナス開始をチェック
        private void CheckBonusStart()
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

        // ボーナス終了をチェック
        private void CheckBonusEnd()
        {
            // 以前の状態がボーナスだった場合チェック
            if (gM.BonusManager.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                // BIG中ならゲーム数が30ゲームを超えたかチェックする
                if (gM.BonusManager.PreviousBonusStatus == BonusStatus.BonusBIGGames)
                {
                    gM.BonusManager.CheckBigGameStatus(gM.Payout.LastPayoutResult.IsReplayOrJacIn);
                }
                // JAC中はJACゲームが終わったかチェックする
                else if (gM.BonusManager.PreviousBonusStatus == BonusStatus.BonusJACGames)
                {
                    gM.BonusManager.CheckBonusGameStatus(gM.Payout.LastPayoutResult.Payout > 0);

                }
                // 変更前とステートが変わっていたらボーナスごとに合わせた処理を実行
                if(gM.BonusManager.PreviousBonusStatus != gM.BonusManager.CurrentBonusStatus)
                {
                    BonusStatusUpdate();
                    // オートがあり終了条件がボーナス終了時の場合はここで判定する
                    if (gM.Auto.HasAuto)
                    {
                        gM.Auto.CheckAutoEndByBonusFinish(gM.BonusManager.CurrentBonusStatus);
                    }
                }
            }
        }

        // 連チャン区間終了をチェック
        private void CheckZoneEnd()
        {
            // ゲーム数が50を超えた場合は連チャン区間を終了させる(但しボーナス非成立時のみ)
            if (gM.Player.CurrentGames == MaxZoneGames &&
                gM.BonusManager.HoldingBonusID == BonusTypeID.BonusNone)
            {
                gM.BonusManager.ResetZonePayout();
            }
        }

        // 小役図柄成立回数のカウント
        private void CountSymbol()
        {
            // 小役カウンタの増減(通常時のみ)
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusNone)
            {
                if (gM.Payout.LastPayoutResult.Payout > 0 || gM.Payout.LastPayoutResult.IsReplayOrJacIn)
                {
                    gM.Lots.IncreaseCounter(gM.Payout.LastPayoutResult.Payout);
                }
            }

            // 小役成立回数の記録
            gM.Player.PlayerAnalyticsData.IncreaseHitCountByFlag(gM.Lots.GetCurrentFlag(), gM.BonusManager.CurrentBonusStatus);
            // 小役入賞回数の記録(払い出しがあれば)
            if (gM.Payout.LastPayoutResult.Payout > 0 || medalManager.HasReplay)
            {
                gM.Player.PlayerAnalyticsData.IncreaseLineUpCountByFlag(gM.Lots.GetCurrentFlag(), gM.BonusManager.CurrentBonusStatus);
            }
            // JACハズシの記録
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusBIGGames && gM.Lots.GetCurrentFlag() == FlagID.FlagReplayJacIn)
            {
                gM.Player.PlayerAnalyticsData.CountJacAvoidCounts(gM.Reel.GetLastPushedLowerPos((int)ReelID.ReelLeft), gM.Reel.GetRandomValue());
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // リールから揃ったボーナス図柄を得る
            gM.BonusManager.ResetBigType();
            BigType color = gM.Reel.GetBigLinedUpCount(medalManager.LastBetAmount, 3);

            // ビッグチャンスの場合
            if (gM.Payout.LastPayoutResult.BonusID == (int)BonusTypeID.BonusBIG)
            {
                gM.BonusManager.StartBigChance(color);
                gM.Player.SetLastBigChanceColor(color);
            }
            // ボーナスゲームの場合
            else if (gM.Payout.LastPayoutResult.BonusID == (int)BonusTypeID.BonusREG)
            {
                gM.BonusManager.StartBonusGame();
            }

            // カウンタリセット
            gM.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            gM.Player.SetLastBonusStart();
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // ボーナス未成立でいずれかのボーナスが成立した場合はストック
            if (gM.BonusManager.HoldingBonusID == BonusTypeID.BonusNone)
            {
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagBig)
                {
                    gM.BonusManager.HoldingBonusID = BonusTypeID.BonusBIG;
                }
                if (gM.Lots.GetCurrentFlag() == FlagID.FlagReg)
                {
                    gM.BonusManager.HoldingBonusID = BonusTypeID.BonusREG;
                }
            }
        }

        // ボーナス状態によるデータ変更
        private void BonusStatusUpdate()
        {
            // ビッグチャンス中に移行した場合はMAXBETを3, BIG中の抽選、払出テーブルへ変更
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotTable.BigBonus);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                medalManager.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }
            // ボーナスゲーム中に移行した場合はMAXBETを1, JAC中の抽選、払出テーブルへ変更
            else if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                medalManager.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotTable.JacGame);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }
            // 通常時に移行した場合はMAXBETを3, 通常時の抽選、払出テーブルへ変更
            else if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotTable.Normal);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                medalManager.ChangeMaxBet(3);
            }
        }
    }
}
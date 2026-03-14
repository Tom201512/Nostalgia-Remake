using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Flag;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using ReelSpinGame_Payout;
using ReelSpinGame_Reels;

namespace ReelSpinGame_State.PayoutState
{
    // 払い出しステート
    public class PayoutState : IGameStatement
    {
        private GameManager gM;                 // ゲームマネージャ

        public PayoutState(GameManager gameManager)
        {
            gM = gameManager;
        }
        public void StateStart()
        {
            // 払い出し確認
            gM.PayoutManager.CheckPayouts(gM.MedalManager.LastBetAmount, gM.ReelManager.GetLastStoppedReelData());
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
            gM.MedalManager.StartPayout(gM.PayoutManager.LastPayoutResult.Payout, gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            // リプレイ処理
            UpdateReplay();

            // 通常時のみの処理
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone)
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
            gM.PlayerSaveDatabase.RecordPlayerSave(gM.Player.MakeSaveData());
            gM.PlayerSaveDatabase.RecordMedalSave(gM.MedalManager.MakeSaveData());
            gM.PlayerSaveDatabase.RecordFlagCounter(gM.FlagManager.FlagCounter);
            gM.PlayerSaveDatabase.RecordReelPos(gM.ReelManager.GetLastStoppedReelData().LastPos);
            gM.PlayerSaveDatabase.RecordBonusData(gM.BonusManager.MakeSaveData());
        }

        // リプレイ処理
        private void UpdateReplay()
        {
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone &&
                    gM.PayoutManager.LastPayoutResult.IsReplayOrJacIn)
            {
                // 最後に賭けた枚数をOUTに反映
                gM.Player.PlayerMedalData.IncreaseOutMedal(gM.MedalManager.LastBetAmount);
                gM.MedalManager.EnableReplay();
            }
            else if (gM.MedalManager.HasReplay)
            {
                gM.MedalManager.DisableReplay();
            }
        }

        // 払い出し処理
        private void PayoutUpdate()
        {
            // プレイヤーメダルの増加、OUT枚数の増加(データのみ変更)
            gM.Player.PlayerMedalData.IncreasePlayerMedal(gM.PayoutManager.LastPayoutResult.Payout);
            gM.Player.PlayerMedalData.IncreaseOutMedal(gM.PayoutManager.LastPayoutResult.Payout);

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gM.BonusManager.CurrentBonusStatus != BonusModel.BonusStatus.BonusNone)
            {
                gM.BonusManager.ChangeBonusPayout(gM.PayoutManager.LastPayoutResult.Payout);
                gM.Player.ChangeLastBonusPayout(gM.BonusManager.CurrentBonusPayout);
            }
            // ゾーン区間(50G)にいる間はその払い出しを計算
            if (gM.BonusManager.HasZone)
            {
                gM.BonusManager.ChangeZonePayout(gM.PayoutManager.LastPayoutResult.Payout);
            }
        }

        // ボーナス開始をチェック
        private void CheckBonusStart()
        {
            // ボーナスがあればボーナス開始
            if (gM.PayoutManager.LastPayoutResult.BonusID != (int)BonusModel.BonusTypeID.BonusNone)
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
                gM.Auto.CheckAutoEndByBonus(gM.PayoutManager.LastPayoutResult.BonusID);
            }
        }

        // ボーナス終了をチェック
        private void CheckBonusEnd()
        {
            // 以前の状態がボーナスだった場合チェック
            if (gM.BonusManager.CurrentBonusStatus != BonusModel.BonusStatus.BonusNone)
            {
                // BIG中ならゲーム数が30ゲームを超えたかチェックする
                if (gM.BonusManager.PreviousBonusStatus == BonusModel.BonusStatus.BonusBIGGames)
                {
                    gM.BonusManager.CheckBigGameStatus(gM.PayoutManager.LastPayoutResult.IsReplayOrJacIn);
                }
                // JAC中はJACゲームが終わったかチェックする
                else if (gM.BonusManager.PreviousBonusStatus == BonusModel.BonusStatus.BonusJACGames)
                {
                    gM.BonusManager.CheckBonusGameStatus(gM.PayoutManager.LastPayoutResult.Payout > 0);

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
            if (gM.Player.CurrentGames == BonusModel.MaxZoneGames &&
                gM.BonusManager.HoldingBonusID == BonusModel.BonusTypeID.BonusNone)
            {
                gM.BonusManager.ResetZonePayout();
            }
        }

        // 小役図柄成立回数のカウント
        private void CountSymbol()
        {
            // 小役カウンタの増減(通常時のみ)
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone)
            {
                if (gM.PayoutManager.LastPayoutResult.Payout > 0 || gM.PayoutManager.LastPayoutResult.IsReplayOrJacIn)
                {
                    gM.FlagManager.IncreaseCounter(gM.PayoutManager.LastPayoutResult.Payout);
                }
            }

            // 小役成立回数の記録
            gM.Player.PlayerAnalyticsData.IncreaseHitCountByFlag(gM.FlagManager.CurrentFlag, gM.BonusManager.CurrentBonusStatus);
            // 小役入賞回数の記録(払い出しがあれば)
            if (gM.PayoutManager.LastPayoutResult.Payout > 0 || gM.MedalManager.HasReplay)
            {
                gM.Player.PlayerAnalyticsData.IncreaseLineUpCountByFlag(gM.FlagManager.CurrentFlag, gM.BonusManager.CurrentBonusStatus);
            }
            // JACハズシの記録
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusBIGGames && gM.FlagManager.CurrentFlag == FlagModel.FlagID.FlagReplayJacIn)
            {
                gM.Player.PlayerAnalyticsData.CountJacAvoidCounts(gM.ReelManager.GetLastPushedLowerPos((int)ReelID.ReelLeft), gM.ReelManager.GetRandomValue());
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // リールから揃ったボーナス図柄を得る
            gM.BonusManager.ResetBigType();
            BonusModel.BigType color = gM.ReelManager.GetBigLinedUpCount(gM.MedalManager.LastBetAmount, 3);

            // ビッグチャンスの場合
            if (gM.PayoutManager.LastPayoutResult.BonusID == (int)BonusModel.BonusTypeID.BonusBIG)
            {
                gM.BonusManager.StartBigChance(color);
                gM.Player.SetLastBigChanceColor(color);
            }
            // ボーナスゲームの場合
            else if (gM.PayoutManager.LastPayoutResult.BonusID == (int)BonusModel.BonusTypeID.BonusREG)
            {
                gM.BonusManager.StartBonusGame();
            }

            // カウンタリセット
            gM.FlagManager.ResetCounter();
            // 入賞時ゲーム数を記録
            gM.Player.SetLastBonusStart();
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // ボーナス未成立でいずれかのボーナスが成立した場合はストック
            if (gM.BonusManager.HoldingBonusID == BonusModel.BonusTypeID.BonusNone)
            {
                if (gM.FlagManager.CurrentFlag == FlagModel.FlagID.FlagBig)
                {
                    gM.BonusManager.HoldingBonusID = BonusModel.BonusTypeID.BonusBIG;
                }
                if (gM.FlagManager.CurrentFlag == FlagModel.FlagID.FlagReg)
                {
                    gM.BonusManager.HoldingBonusID = BonusModel.BonusTypeID.BonusREG;
                }
            }
        }

        // ボーナス状態によるデータ変更
        private void BonusStatusUpdate()
        {
            // ビッグチャンス中に移行した場合はMAXBETを3, BIG中の抽選、払出テーブルへ変更
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusBIGGames)
            {
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.BigBonus);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutBIG);
                gM.MedalManager.ChangeMaxBet(3);
                gM.FlagManager.ResetCounter();
            }
            // ボーナスゲーム中に移行した場合はMAXBETを1, JAC中の抽選、払出テーブルへ変更
            else if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusJACGames)
            {
                gM.MedalManager.ChangeMaxBet(1);
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.JacGame);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutJAC);
            }
            // 通常時に移行した場合はMAXBETを3, 通常時の抽選、払出テーブルへ変更
            else if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone)
            {
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.Normal);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutNormal);
                gM.MedalManager.ChangeMaxBet(3);
            }
        }
    }
}
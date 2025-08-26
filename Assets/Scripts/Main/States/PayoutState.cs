using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
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

        // ボーナスが開始されたか
        private bool HasBonusStarted;
        // ボーナスが終了したか
        private bool HasBonusFinished;

        // コンストラクタ
        public PayoutState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Payout;
            gM = gameManager;

        }
        public void StateStart()
        {
            // 高速オートが解除されたかチェック
            gM.Auto.CheckFastAutoCancelled();

            // ボーナス開始されたかリセット
            HasBonusStarted = false;
            HasBonusFinished = false;

            // 払い出し確認
            StartCheckPayout(gM.Medal.GetLastBetAmount());

            // 払い出しの結果をデータに反映
            PayoutUpdate();

            // フラグ増減(通常時)
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                ChangeFlagCounter();
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

            // オートが切れたら音声を有効にする
            if (!gM.Auto.HasAuto)
            {
                EnableSounds();
            }

            // セーブ処理
            SaveData();

            // ここから下は演出

            // 演出開始(高速オートが無効であることが条件)
            if(!gM.Auto.HasAuto ||
                (gM.Auto.HasAuto && gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal))
            {
                StartEffect();
            }
        }

        public void StateUpdate()
        {
            // UI更新
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);

            // 払い出しが終わったら停止
            if (gM.Medal.GetRemainingPayout() == 0)
            {
                gM.Effect.StopLoopSound();

                // 払い出し、各種演出(フラッシュ、BGMなど)の待機処理が終わっていたら投入状態へ
                if (gM.Effect.HasEffectFinished())
                {
                    gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
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

        // データのセーブ
        private void SaveData()
        {
            // プレイヤー情報記録
            gM.Save.RecordPlayerSave(gM.Player.MakeSaveData());
            // メダル情報記録
            gM.Save.RecordMedalSave(gM.Medal.MakeSaveData());
            // フラグ情報記録
            gM.Save.RecordFlagCounter(gM.Lots.GetCounter());
            // 最終リール位置を記録
            gM.Save.RecordReelPos(gM.Reel.GetLastStopped().LastPos);
            // ボーナス情報記録
            gM.Save.RecordBonusData(gM.Bonus.MakeSaveData());
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

            // ボーナス中なら各ボーナスの払い出しを増やし状態を変化させる
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.ChangeBonusPayout(gM.Reel.GetPayoutResultData().Payout);
                //gM.Player.ChangeLastBonusPayout(gM.Reel.GetPayoutResultData().Payout);
            }

            // ゾーン区間(50G)にいる間はその払い出しを計算
            if(gM.Bonus.GetHasZone())
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

                // 終了ファンファーレ再生のフラグを立てる
                HasBonusFinished = true;
            }
        }

        // 演出開始
        private void StartEffect()
        {
            // 払い出しがあったらフラッシュを開始させる
            if (gM.Reel.GetPayoutResultData().Payout != 0)
            {
                gM.Effect.StartPayoutReelFlash(gM.Reel.GetPayoutResultData().PayoutLines,
                    gM.Lots.GetCurrentFlag() == FlagId.FlagJac, gM.Reel.GetPayoutResultData().Payout);
            }

            // ボーナス中はビタハズシ成功でフラッシュ
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                gM.Lots.GetCurrentFlag() == FlagId.FlagReplayJacIn)
            {
                // 11番、17番を押した場合はフラッシュ
                if (gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 10 ||
                        gM.Reel.GetPushedPos((int)ReelID.ReelLeft) == 16)
                {
                    gM.Effect.StartVFlash(1);
                }
            }

            // 通常時
            else if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                // リプレイなら1秒待機させてフラッシュ
                if (gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
                {
                    gM.Effect.StartReplayEffect(gM.Reel.GetPayoutResultData().PayoutLines);
                }

                // 通常時BIG, REG成立時に1/6でフラッシュ
                else if (gM.Lots.GetCurrentFlag() == FlagId.FlagBig || gM.Lots.GetCurrentFlag() == FlagId.FlagReg)
                {
                    gM.Effect.StartVFlash(6);
                }

                // 通常時はずれの場合、すでにボーナスが当選していたら1/6でフラッシュ
                else if (gM.Bonus.GetHoldingBonusID() != BonusTypeID.BonusNone)
                {
                    gM.Effect.StartVFlash(6);
                }
            }

            // ファンファーレの再生
            if (HasBonusStarted)
            {
                gM.Effect.StartBonusStartEffect();
            }
            else if (HasBonusFinished)
            {
                gM.Effect.StartBonusEndEffect();
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
                // ビッグチャンス回数、入賞時の色を記録
                gM.Player.IncreaseBigChance();
                //gM.Player.SetLastBigChanceColor(color);
            }

            // ボーナスゲームの場合
            else if (gM.Reel.GetPayoutResultData().BonusID == (int)BonusTypeID.BonusREG)
            {
                gM.Bonus.StartBonusGame();
                // ボーナスゲーム回数(REG)を記録
                gM.Player.IncreaseBonusGame();
            }

            // 15枚の払い出しを記録
            //gM.Player.ChangeLastBonusPayout(gM.Reel.GetPayoutResultData().Payout);
            gM.Bonus.ChangeBonusPayout(gM.Reel.GetPayoutResultData().Payout);
            gM.Bonus.ChangeZonePayout(gM.Reel.GetPayoutResultData().Payout);
            // カウンタリセット
            gM.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            //gM.Player.SetLastBonusStart();
            // ボーナス演出を開始
            HasBonusStarted = true;
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // ボーナス未成立でいずれかのボーナスが成立した場合はストック
            if (gM.Bonus.GetHoldingBonusID() == BonusTypeID.BonusNone)
            {
                // BIG
                if (gM.Lots.GetCurrentFlag() == FlagId.FlagBig)
                {
                    gM.Bonus.SetBonusStock(BonusTypeID.BonusBIG);
                }
                // REG
                if (gM.Lots.GetCurrentFlag() == FlagId.FlagReg)
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

        // フラグ増減
        private void ChangeFlagCounter()
        {
            // 小役が当選していたら増加させる
            // リプレイでは増やさない(0増加)
            if (gM.Reel.GetPayoutResultData().Payout > 0 || gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                gM.Lots.IncreaseCounter(gM.Reel.GetPayoutResultData().Payout);
            }
            // それ以外は減少
            else
            {
                gM.Lots.DecreaseCounter(gM.Setting, gM.Medal.GetLastBetAmount());
            }
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
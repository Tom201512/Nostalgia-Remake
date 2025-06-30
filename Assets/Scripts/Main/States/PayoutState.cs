using ReelSpinGame_Interface;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;
using static ReelSpinGame_Reels.Payout.PayoutChecker;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
        // const

        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }
        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public PayoutState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Payout;
            this.gameManager = gameManager;

        }
        public void StateStart()
        {
            StartCheckPayout(gameManager.Medal.GetLastBetAmounts());
            // 払い出し開始
            gameManager.Medal.StartPayout(gameManager.Reel.GetPayoutResultData().Payouts);

            // プレイヤーメダルの増加、OUT枚数の増加(データのみ変更)
            gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal(gameManager.Reel.GetPayoutResultData().Payouts);
            gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal(gameManager.Reel.GetPayoutResultData().Payouts);

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gameManager.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gameManager.Bonus.ChangeBonusPayouts(gameManager.Reel.GetPayoutResultData().Payouts);
                gameManager.Bonus.ChangeZonePayouts(gameManager.Reel.GetPayoutResultData().Payouts);
                gameManager.PlayerData.ChangeLastBonusPayouts(gameManager.Reel.GetPayoutResultData().Payouts);
            }

            // フラッシュ演出開始
            StartFlash();
            
            // ボーナスごとに処理を変える
            switch (gameManager.Bonus.GetCurrentBonusStatus())
            {
                // 小役ゲーム中
                case BonusStatus.BonusBIGGames:
                    // 状態確認
                    gameManager.Bonus.CheckBigGameStatus(gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn);

                    // ボーナスが終了していればファンファーレ再生
                    if(gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
                    {
                        Debug.Log("Bonus END");
                        gameManager.Effect.StartBonusEndEffect();
                    }
                    break;

                // ボーナスゲーム中
                case BonusStatus.BonusJACGames:
                    // 状態確認
                    gameManager.Bonus.CheckBonusGameStatus(gameManager.Reel.GetPayoutResultData().Payouts > 0);

                    // ボーナスが終了していればファンファーレ再生
                    if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
                    {
                        Debug.Log("Bonus END");
                        gameManager.Effect.StartBonusEndEffect();
                    }
                    break;

                // 通常時はリプレイの処理、またはフラグテーブルの更新
                default:
                    // ボーナスがあればボーナス開始
                    if (gameManager.Reel.GetPayoutResultData().BonusID != (int)BonusType.BonusNone)
                    {
                        StartBonus();
                    }
                    // 取りこぼした場合はストックさせる
                    else
                    {
                        StockBonus();
                    }

                    // フラグ管理
                    // 小役が当選していたら増加させる
                    // リプレイでは増やさない(0増加)
                    if (gameManager.Reel.GetPayoutResultData().Payouts > 0 || gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn)
                    {
                        gameManager.Lots.IncreaseCounter(gameManager.Reel.GetPayoutResultData().Payouts);
                    }
                    // それ以外は減少
                    else
                    {
                        gameManager.Lots.DecreaseCounter(
                            gameManager.Setting, gameManager.Medal.GetLastBetAmounts());
                    }
                    break;
            }
        }

        public void StateUpdate()
        {
            // 払い出しが終わったら停止
            if (gameManager.Medal.GetRemainingPayouts() == 0)
            {
                gameManager.Effect.StopLoopSound();

                // 払い出し、各種演出(フラッシュ、BGMなど)の待機処理が終わっていたら投入状態へ
                if (gameManager.Effect.HasEffectFinished())
                {
                    gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.InsertState);
                }
            }
        }

        public void StateEnd()
        {
            // ボーナス状態の処理
            BonusStatusUpdate();
            // リプレイ処理
            UpdateReplay();

            // 連チャン区間の処理
            // 50Gを迎えた場合は連チャン区間を終了させる(但しボーナス非成立時のみ)
            if(gameManager.PlayerData.CurrentGames == MaxZoneGames && 
                gameManager.Bonus.GetHoldingBonusID() == BonusType.BonusNone)
            {
                gameManager.Bonus.ResetZonePayouts();
            }
        }

        // 払い出し確認
        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.GetIsReelWorking())
            {
                gameManager.Reel.StartCheckPayouts(betAmounts);
            }
        }

        // リプレイ処理
        private void UpdateReplay()
        {
            if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                    gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                // 最後に賭けた枚数をOUTに反映
                gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal(gameManager.Medal.GetLastBetAmounts());
                gameManager.Medal.EnableReplay();
                gameManager.Status.TurnOnReplayLamp();
            }
            else if (gameManager.Medal.GetHasReplay())
            {
                gameManager.Medal.DisableReplay();
                gameManager.Status.TurnOffReplayLamp();
            }
        }

        // フラッシュ開始
        private void StartFlash()
        {
            // 払い出しがあったらフラッシュを開始させる
            if (gameManager.Reel.GetPayoutResultData().Payouts != 0)
            {
                gameManager.Effect.StartPayoutReelFlash(gameManager.Reel.GetPayoutResultData().PayoutLines,
                    gameManager.Bonus.GetCurrentBonusStatus(), gameManager.Reel.GetPayoutResultData().Payouts);
            }

            // 通常時のリプレイだった場合は1秒待たせる。
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                gameManager.Effect.StartReplayEffect(gameManager.Reel.GetPayoutResultData().PayoutLines);
            }

            // 通常時はずれの場合、ボーナスが当選していたら1/6でフラッシュ
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                gameManager.Bonus.GetHoldingBonusID() != BonusType.BonusNone)
            {
                gameManager.Effect.StartRiichiPatternEffect();
            }

            // ボーナス中はビタハズシ成功でフラッシュ
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                gameManager.Lots.GetCurrentFlag() == FlagId.FlagReplayJacIn)
            {
                // 11番、17番を押した場合はフラッシュ
                if (gameManager.Reel.GetLastStopped().LastPos[(int)ReelID.ReelLeft] + 1 == 11 ||
                        gameManager.Reel.GetLastStopped().LastPos[(int)ReelID.ReelLeft] + 1 == 17)
                {
                    gameManager.Effect.StartReelFlash(FlashID.V_Flash);
                }
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // リールから揃ったボーナス図柄の色を得る
            BigColor color = gameManager.Reel.GetBigLinedUpCounts(gameManager.Medal.GetLastBetAmounts(), 3);

            // ビッグチャンス
            if (gameManager.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusBIG)
            {
                gameManager.Bonus.StartBigChance(color);
                // ビッグチャンス回数、入賞時の色を記録
                gameManager.PlayerData.IncreaseBigChance();
                gameManager.PlayerData.SetLastBigChanceColor(color);
            }

            // ボーナスゲーム
            else if (gameManager.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusREG)
            {
                gameManager.Bonus.StartBonusGame();
                gameManager.PlayerData.IncreaseBonusGame();
            }

            // 15枚の払い出しを記録
            gameManager.PlayerData.ChangeLastBonusPayouts(gameManager.Reel.GetPayoutResultData().Payouts);
            gameManager.Bonus.ChangeBonusPayouts(gameManager.Reel.GetPayoutResultData().Payouts);
            gameManager.Bonus.ChangeZonePayouts(gameManager.Reel.GetPayoutResultData().Payouts);
            // カウンタリセット
            gameManager.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            gameManager.PlayerData.SetLastBonusStart();
            // ファンファーレ演出スタート
            gameManager.Effect.StartBonusStartEffect(color);
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // いずれかのボーナスが引かれた場合はストック(BIGまたはREGのどちらかをストックする)
            // これにより常に成立後出目が出るようになる。
            if (gameManager.Bonus.GetHoldingBonusID() == BonusType.BonusNone)
            {
                // BIG
                if (gameManager.Lots.GetCurrentFlag() == FlagId.FlagBig)
                {
                    gameManager.Bonus.SetBonusStock(BonusType.BonusBIG);
                }
                // REG
                if (gameManager.Lots.GetCurrentFlag() == FlagId.FlagReg)
                {
                    gameManager.Bonus.SetBonusStock(BonusType.BonusREG);
                }
            }
        }

        // ボーナス状態によるデータ変更
        private void BonusStatusUpdate()
        {
            // ビッグチャンス中に移行した場合
            if(gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gameManager.Lots.ChangeTable(FlagLotMode.BigBonus);
                gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gameManager.Medal.ChangeMaxBet(3);
                gameManager.Lots.ResetCounter();
            }

            // ボーナスゲーム中に移行した場合
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gameManager.Medal.ChangeMaxBet(1);
                gameManager.Lots.ChangeTable(FlagLotMode.JacGame);
                gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }

            // 通常時に移行した場合
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gameManager.Lots.ChangeTable(FlagLotMode.Normal);
                gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                gameManager.Medal.ChangeMaxBet(3);
            }
            // ボーナス中のランプ処理
            gameManager.Bonus.UpdateSegments();
            // ボーナス中のBGM処理
            gameManager.Effect.PlayBonusBGM(gameManager.Bonus.GetCurrentBonusStatus());
        }
    }
}
using ReelSpinGame_Interface;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehavior;
using static ReelSpinGame_Lots.FlagBehaviour;
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
        private GameManager gM;

        // ボーナスが開始されたか
        private bool HasBonusStarted;
        // ボーナスが終了したか
        private bool HasBonusFinished;

        // コンストラクタ
        public PayoutState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Payout;
            this.gM = gameManager;

        }
        public void StateStart()
        {
            // ボーナス開始されたかリセット
            HasBonusStarted = false;
            HasBonusFinished = false;

            // 最終リール位置を記録
            gM.Save.SetReelPos(gM.Reel.GetLastStopped().LastPos);

            // 払い出し確認
            StartCheckPayout(gM.Medal.GetLastBetAmounts());

            // 払い出しの結果をデータに反映
            PayoutUpdate();

            // フラグ増減
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                ChangeFlagCounter();
            }

            // フラグ数値を記録
            gM.Save.SetFlagCounter(gM.Lots.GetCounter());

            // リプレイ処理
            UpdateReplay();

            // 状態遷移
            CheckGameModeStatusChange();

            // ボーナス状態更新

            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                BonusStatusUpdate();
            }

            // 連チャン区間の処理
            // 50Gを迎えた場合は連チャン区間を終了させる(但しボーナス非成立時のみ)
            if (gM.Save.Player.CurrentGames == MaxZoneGames &&
                gM.Bonus.GetHoldingBonusID() == BonusType.BonusNone)
            {
                gM.Bonus.ResetZonePayouts();
            }

            // ここから下は演出

            // 払い出し開始
            gM.Medal.StartPayout(gM.Reel.GetPayoutResultData().Payouts);

            // 演出開始
            StartEffect();
        }

        public void StateUpdate()
        {
            // 払い出しが終わったら停止
            if (gM.Medal.GetRemainingPayouts() == 0)
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
            // ボーナス演出更新
            BonusEffectUpdate();
        }

        // 払い出し確認
        private void StartCheckPayout(int betAmounts)
        {
            if (!gM.Reel.GetIsReelWorking())
            {
                gM.Reel.StartCheckPayouts(betAmounts);
            }
        }

        // リプレイ処理
        private void UpdateReplay()
        {
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                    gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                // 最後に賭けた枚数をOUTに反映
                gM.Save.Player.PlayerMedalData.IncreaseOutMedal(gM.Medal.GetLastBetAmounts());
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
            gM.Save.Player.PlayerMedalData.IncreasePlayerMedal(gM.Reel.GetPayoutResultData().Payouts);
            gM.Save.Player.PlayerMedalData.IncreaseOutMedal(gM.Reel.GetPayoutResultData().Payouts);

            // ボーナス中なら各ボーナスの払い出しを増やし状態を変化させる
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.ChangeBonusPayouts(gM.Reel.GetPayoutResultData().Payouts);
                gM.Bonus.ChangeZonePayouts(gM.Reel.GetPayoutResultData().Payouts);
                gM.Save.Player.ChangeLastBonusPayouts(gM.Reel.GetPayoutResultData().Payouts);
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

                // ファンファーレ再生のフラグを立てる
                HasBonusFinished = true;
            }
        }

        // 演出開始
        private void StartEffect()
        {
            // 払い出しがあったらフラッシュを開始させる
            if (gM.Reel.GetPayoutResultData().Payouts != 0)
            {
                gM.Effect.StartPayoutReelFlash(gM.Reel.GetPayoutResultData().PayoutLines,
                    gM.Lots.GetCurrentFlag() == FlagId.FlagJac, gM.Reel.GetPayoutResultData().Payouts);

                // ファンファーレの再生
                if(HasBonusStarted)
                {
                    gM.Effect.StartBonusStartEffect(gM.Bonus.GetBigChanceColor());
                }
                else if(HasBonusFinished)
                {
                    Debug.Log("BonusEND");
                    gM.Effect.StartBonusEndEffect();
                }
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
                else if (gM.Bonus.GetHoldingBonusID() != BonusType.BonusNone)
                {
                    gM.Effect.StartVFlash(6);
                }
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // リールから揃ったボーナス図柄の色を得る
            BigColor color = gM.Reel.GetBigLinedUpCounts(gM.Medal.GetLastBetAmounts(), 3);

            // ビッグチャンス
            if (gM.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusBIG)
            {
                gM.Bonus.StartBigChance(color);
                // ビッグチャンス回数、入賞時の色を記録
                gM.Save.Player.IncreaseBigChance();
                gM.Save.Player.SetLastBigChanceColor(color);
            }

            // ボーナスゲーム
            else if (gM.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusREG)
            {
                gM.Bonus.StartBonusGame();
                gM.Save.Player.IncreaseBonusGame();
            }

            // 15枚の払い出しを記録
            gM.Save.Player.ChangeLastBonusPayouts(gM.Reel.GetPayoutResultData().Payouts);
            gM.Bonus.ChangeBonusPayouts(gM.Reel.GetPayoutResultData().Payouts);
            gM.Bonus.ChangeZonePayouts(gM.Reel.GetPayoutResultData().Payouts);
            // カウンタリセット
            gM.Lots.ResetCounter();
            // 入賞時ゲーム数を記録
            gM.Save.Player.SetLastBonusStart();

            HasBonusStarted = true;
        }

        // ボーナスをストックさせる
        private void StockBonus()
        {
            // いずれかのボーナスが引かれた場合はストック(BIGまたはREGのどちらかをストックする)
            // これにより常に成立後出目が出るようになる。
            if (gM.Bonus.GetHoldingBonusID() == BonusType.BonusNone)
            {
                // BIG
                if (gM.Lots.GetCurrentFlag() == FlagId.FlagBig)
                {
                    gM.Bonus.SetBonusStock(BonusType.BonusBIG);
                }
                // REG
                if (gM.Lots.GetCurrentFlag() == FlagId.FlagReg)
                {
                    gM.Bonus.SetBonusStock(BonusType.BonusREG);
                }
            }
        }

        // 各ゲームモード時の状態チェック
        private void CheckGameModeStatusChange()
        {
            if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gM.Bonus.CheckBigGameStatus(gM.Reel.GetPayoutResultData().IsReplayOrJacIn);
                BonusStatusUpdate();
            }
            else if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gM.Bonus.CheckBonusGameStatus(gM.Reel.GetPayoutResultData().Payouts > 0);
                BonusStatusUpdate();
            }
            else
            {
                // ボーナスがあればボーナス開始
                if (gM.Reel.GetPayoutResultData().BonusID != (int)BonusType.BonusNone)
                {
                    StartBonus();
                }
                // 取りこぼした場合はストックさせる
                else
                {
                    StockBonus();
                }
            }
        }

        // フラグ増減
        private void ChangeFlagCounter()
        {
            // 小役が当選していたら増加させる
            // リプレイでは増やさない(0増加)
            if (gM.Reel.GetPayoutResultData().Payouts > 0 || gM.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                gM.Lots.IncreaseCounter(gM.Reel.GetPayoutResultData().Payouts);
            }
            // それ以外は減少
            else
            {
                gM.Lots.DecreaseCounter(gM.Setting, gM.Medal.GetLastBetAmounts());
            }
        }

        // ボーナス関連の演出更新
        private void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // ボーナス中のBGM処理
            gM.Effect.PlayBonusBGM(gM.Bonus.GetCurrentBonusStatus());
        }
    }
}
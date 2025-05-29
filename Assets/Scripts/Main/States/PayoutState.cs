using JetBrains.Annotations;
using ReelSpinGame_Bonus;
using ReelSpinGame_Interface;
using System.Collections;
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

        //test
        public bool TestFlag = false;

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
            //Debug.Log("Start Payout State");
            //Debug.Log("Start Payout Check");

            StartCheckPayout(gameManager.Medal.GetLastBetAmounts());
            //Debug.Log("Payouts result" + gameManager.Payout.LastPayoutResult.Payouts);
            //Debug.Log("Bonus.Data." + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

            // 払い出しの為にプレイヤーのメダルをアタッチする
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;

            gameManager.Medal.StartPayout(gameManager.Reel.GetPayoutResultData().Payouts);

            // 1枚でも払い出しがあれば音再生
            if(gameManager.Reel.GetPayoutResultData().Payouts > 0)
            {
                PayoutSound();
            }

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gameManager.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Reel.GetPayoutResultData().Payouts);
            }

            // ボーナスごとに処理を変える
            switch (gameManager.Bonus.GetCurrentBonusStatus())
            {
                // 小役ゲーム中
                case BonusStatus.BonusBIGGames:

                    // 状態確認
                    gameManager.Bonus.CheckBigGameStatus(gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn);

                    // JAC-INまたは小役ゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
                    {
                        gameManager.Lots.ChangeTable(FlagLotMode.JacGame);
                        gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
                        gameManager.Medal.ChangeMaxBet(1);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
                    {
                        gameManager.Lots.ChangeTable(FlagLotMode.Normal);
                        gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.ResetCounter();
                    }
                    break;

                // ボーナスゲーム中
                case BonusStatus.BonusJACGames:

                    // 状態確認
                    gameManager.Bonus.CheckBonusGameStatus(gameManager.Reel.GetPayoutResultData().Payouts > 0);

                    // ボーナスゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
                    {
                        gameManager.Lots.ChangeTable(FlagLotMode.BigBonus);
                        gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                        gameManager.Medal.ChangeMaxBet(3);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
                    {
                        gameManager.Lots.ChangeTable(FlagLotMode.Normal);
                        gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.ResetCounter();
                    }
                    break;

                // 通常時はリプレイの処理
                // またフラグテーブルの更新
                default:
                    // ボーナスがあればボーナス開始
                    if (gameManager.Reel.GetPayoutResultData().BonusID != (int)BonusType.BonusNone)
                    {
                        StartBonus();
                    }
                    // 取りこぼした場合はストックさせる
                    else
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

            // 払い出しがあったらフラッシュを開始させる
            if (gameManager.Reel.GetPayoutResultData().Payouts != 0)
            {
                // フラッシュさせる
                gameManager.Reel.StartPayoutFlash(0);
            }

            // 通常時のリプレイだった場合は1秒待たせる。
            else if(gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn)
            {
                //音再生
                gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Replay);
                // フラッシュさせる
                gameManager.Reel.StartPayoutFlash(1.0f);
            }

            // 通常時はずれの場合は一定確率(1/6)でフラッシュさせる
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                gameManager.Bonus.GetHoldingBonusID() != BonusType.BonusNone &&
                Random.Range(0, 5) == 0)
            {
                gameManager.Reel.StartReelFlash(FlashID.V_Flash);
            }

            // ボーナス中はビタハズシかでフラッシュさせる
            else if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                gameManager.Lots.GetCurrentFlag() == FlagId.FlagReplayJacIn)
            {
                // 11番、17番を押した場合はフラッシュ
                if (gameManager.Reel.GetLastStopped().LastPos[(int)ReelID.ReelLeft] + 1 == 11 ||
                        gameManager.Reel.GetLastStopped().LastPos[(int)ReelID.ReelLeft] + 1 == 17)
                {
                    gameManager.Reel.StartReelFlash(FlashID.V_Flash);
                }
            }
        }

        public void StateUpdate()
        {
            // 払い出し、各種演出(フラッシュ、BGMなど)の待機処理が終わっていたら投入状態へ
            if(gameManager.Medal.GetPayoutAmounts() == 0 && !gameManager.Reel.GetHasFlashWait() &&
                gameManager.Sound.GetBGMStopped())
            {
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;

                // リプレイ処理
                if (gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone &&
                    gameManager.Reel.GetPayoutResultData().IsReplayOrJacIn)
                {
                    // 最後に賭けた枚数をOUTに反映
                    gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal(gameManager.Medal.GetLastBetAmounts());
                    gameManager.Medal.EnableReplay();
                }
                else if (gameManager.Medal.GetHasReplay())
                {
                    gameManager.Medal.DisableReplay();
                }

                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.InsertState);
            }
        }

        public void StateEnd()
        {
            //Debug.Log("End Payout State");
            //Debug.Log("HasReplay:" + gameManager.Medal.GetHasReplay());

            // リプレイの処理
            if(gameManager.Medal.GetHasReplay())
            {
                gameManager.Status.TurnOnReplayLamp();
            }
            else
            {
                gameManager.Status.TurnOffReplayLamp();
            }

            // ボーナス中のランプ処理
            gameManager.Bonus.UpdateSegments();
            // ループサウンド停止
            gameManager.Sound.StopLoopSound();
        }

        // 払い出し確認
        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.GetIsReelWorking())
            {
                gameManager.Reel.StartCheckPayouts(betAmounts);
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // ビッグチャンス
            if(gameManager.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusBIG)
            {
                // リールから揃ったボーナス図柄の色を得る
                gameManager.Bonus.StartBigChance(CheckBigChanceColor(gameManager.Medal.GetLastBetAmounts()));
                gameManager.Lots.ChangeTable(FlagLotMode.BigBonus);
                gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gameManager.PlayerData.IncreaseBigChance();
            }

            // ボーナスゲーム
            else if (gameManager.Reel.GetPayoutResultData().BonusID == (int)BonusType.BonusREG)
            {
                gameManager.Bonus.StartBonusGame();
                gameManager.Medal.ChangeMaxBet(1);
                gameManager.Lots.ChangeTable(FlagLotMode.JacGame);
                gameManager.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
                gameManager.PlayerData.IncreaseBonusGame();
            }

            // 15枚の払い出しを加算
            //Debug.Log(gameManager.Payout.LastPayoutResult.Payouts);
            gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Reel.GetPayoutResultData().Payouts);
            gameManager.Lots.ResetCounter();
            gameManager.PlayerData.SetLastBonusStart();
        }

        // 払い出し音
        private void PayoutSound()
        {
            // JAC中の払い出し音
            if(gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.JacPayout);
            }
            // 15枚の払い出し音
            else if (gameManager.Reel.GetPayoutResultData().Payouts >= 15)
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.MaxPayout);
            }
            //　それ以外は通常の払い出し音
            else
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.NormalPayout);
            }
        }

        // 当選したBIGの種類を調べる
        private BigColor CheckBigChanceColor(int betAmounts)
        {
            // 赤7
            if (gameManager.Reel.CountBonusSymbols(BigColor.Red, betAmounts) == 3)
            {
                return BigColor.Red;
            }

            // 青7
            if (gameManager.Reel.CountBonusSymbols(BigColor.Blue, betAmounts) == 3)
            {
                return BigColor.Blue;
            }

            // BB7
            if (gameManager.Reel.CountBonusSymbols(BigColor.Black, betAmounts) == 3)
            {
                return BigColor.Black;
            }

            return BigColor.None;
        }
    }
}
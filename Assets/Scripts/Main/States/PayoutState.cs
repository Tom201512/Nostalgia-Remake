using ReelSpinGame_Interface;
using ReelSpinGame_Medal;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
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
            Debug.Log("Start Payout State");
            Debug.Log("Start Payout Check");

            StartCheckPayout(gameManager.Medal.GetLastBetAmounts());
            Debug.Log("Payouts result" + gameManager.Payout.LastPayoutResult.Payouts);
            Debug.Log("Bonus.Data." + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

            // 払い出しの為にプレイヤーのメダルをアタッチする
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;

            gameManager.Medal.StartPayout(gameManager.Payout.LastPayoutResult.Payouts);

            // 払い出し音再生
            if(gameManager.Payout.LastPayoutResult.Payouts > 0)
            {
                // 払い出し音再生
                PayoutSound();
            }

            // ボーナス中なら各ボーナスの払い出しを増やす
            if (gameManager.Bonus.Data.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Payout.LastPayoutResult.Payouts);
            }

            // ボーナスごとに処理を変える
            switch (gameManager.Bonus.Data.CurrentBonusStatus)
            {
                // 小役ゲーム中
                case BonusStatus.BonusBIGGames:

                    // 状態確認
                    gameManager.Bonus.Data.CheckBigGameStatus(gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

                    // JAC-INまたは小役ゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusJACGames)
                    {
                        gameManager.Lots.Data.ChangeTable(FlagLotMode.JacGame);
                        gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutJAC);
                        gameManager.Medal.ChangeMaxBet(1);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone)
                    {
                        gameManager.Lots.Data.ChangeTable(FlagLotMode.Normal);
                        gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.Data.FlagCounter.ResetCounter();
                    }
                    break;

                // ボーナスゲーム中
                case BonusStatus.BonusJACGames:

                    // 状態確認
                    gameManager.Bonus.Data.CheckBonusGameStatus(gameManager.Payout.LastPayoutResult.Payouts > 0);

                    // ボーナスゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusBIGGames)
                    {
                        gameManager.Lots.Data.ChangeTable(FlagLotMode.BigBonus);
                        gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutBIG);
                        gameManager.Medal.ChangeMaxBet(3);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone)
                    {
                        gameManager.Lots.Data.ChangeTable(FlagLotMode.Normal);
                        gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.Data.FlagCounter.ResetCounter();
                    }
                    break;

                // 通常時はリプレイの処理
                // またフラグテーブルの更新

                default:
                    // ボーナスがあればボーナス開始
                    if (gameManager.Payout.LastPayoutResult.BonusID != (int)BonusType.BonusNone)
                    {
                        StartBonus();
                    }
                    // 取りこぼした場合はストックさせる
                    else
                    {
                        // いずれかのボーナスが引かれた場合はストック(BIGまたはREGのどちらかをストックする)
                        // これにより常に成立後出目が出るようになる。
                        if (gameManager.Bonus.Data.HoldingBonusID == BonusType.BonusNone)
                        {
                            // BIG
                            if (gameManager.Lots.Data.CurrentFlag == FlagId.FlagBig)
                            {
                                gameManager.Bonus.Data.SetBonusStock(BonusType.BonusBIG);
                            }
                            // REG
                            if (gameManager.Lots.Data.CurrentFlag == FlagId.FlagReg)
                            {
                                gameManager.Bonus.Data.SetBonusStock(BonusType.BonusREG);
                            }
                        }
                    }

                    // フラグ管理
                    // 小役が当選していたら増加させる
                    // リプレイでは増やさない(0増加)
                    if (gameManager.Payout.LastPayoutResult.Payouts > 0 || gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
                    {
                        gameManager.Lots.Data.FlagCounter.IncreaseCounter(gameManager.Payout.LastPayoutResult.Payouts);
                    }
                    // それ以外は減少
                    else
                    {
                        gameManager.Lots.Data.FlagCounter.DecreaseCounter(
                            gameManager.Setting, gameManager.Medal.GetLastBetAmounts());
                    }
                    break;
            }

            // フラッシュを開始させる
            if (gameManager.Payout.LastPayoutResult.Payouts != 0)
            {
                // フラッシュさせる
                gameManager.Reel.FlashManager.StartPayoutFlash(gameManager.Payout.LastPayoutResult.PayoutLines, false);
            }

            // 通常時のリプレイだった場合は3秒待たせる。
            else if(gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone &&
                gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
            {
                //音再生
                gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Replay);
                // フラッシュさせる
                gameManager.Reel.FlashManager.StartPayoutFlash(gameManager.Payout.LastPayoutResult.PayoutLines,true);
            }

            // 通常時はずれの場合は一定確率(1/6)でフラッシュさせる
            else if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone &&
                gameManager.Bonus.Data.HoldingBonusID != BonusType.BonusNone &&
                Random.Range(0, 5) == 0)
            {
                gameManager.Reel.FlashManager.StartFlash((int)FlashManager.FlashID.V_Flash);
            }

            // ボーナス中はビタハズシかでフラッシュさせる
            else if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusBIGGames &&
                gameManager.Lots.Data.CurrentFlag == FlagId.FlagReplayJacIn)
            {
                // 11番、17番を押した場合はフラッシュ
                if (gameManager.Reel.Data.LastStopped.LastPos[(int)ReelID.ReelLeft] + 1 == 11 ||
                        gameManager.Reel.Data.LastStopped.LastPos[(int)ReelID.ReelLeft] + 1 == 17)
                {
                    gameManager.Reel.FlashManager.StartFlash((int)FlashManager.FlashID.V_Flash);
                }
            }
        }

        public void StateUpdate()
        {
            // 払い出し、リプレイの待機処理が終わっていたら投入状態へ
            if(gameManager.Medal.GetPayoutAmounts() == 0 && !gameManager.Reel.FlashManager.HasReplayWait)
            {
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;

                // リプレイ処理
                if (gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusNone &&
                    gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
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
            Debug.Log("End Payout State");
            Debug.Log("HasReplay:" + gameManager.Medal.GetHasReplay());

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

        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.Data.IsWorking)
            {
                gameManager.Payout.CheckPayoutLines(betAmounts, gameManager.Reel.Data.LastStopped);
            }
            else
            {
                Debug.Log("Failed to check payout because reels are spinning");
            }
        }

        //　ボーナス開始
        private void StartBonus()
        {
            // ビッグチャンス
            if(gameManager.Payout.LastPayoutResult.BonusID == (int)BonusType.BonusBIG)
            {
                // リールから揃ったボーナス図柄の色を得る
                gameManager.Bonus.Data.StartBigChance(gameManager.Payout.LastPayoutResult.PayoutLines, gameManager.Reel.Data.LastStopped);
                gameManager.Lots.Data.ChangeTable(FlagLotMode.BigBonus);
                gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutBIG);
                gameManager.PlayerData.IncreaseBigChance();
            }

            // ボーナスゲーム
            else if (gameManager.Payout.LastPayoutResult.BonusID == (int)BonusType.BonusREG)
            {
                gameManager.Bonus.Data.StartBonusGame();
                gameManager.Medal.ChangeMaxBet(1);
                gameManager.Lots.Data.ChangeTable(FlagLotMode.JacGame);
                gameManager.Payout.ChangePayoutCheckMode(PayoutChecker.PayoutCheckMode.PayoutJAC);
                gameManager.PlayerData.IncreaseBonusGame();
            }

            // 15枚の払い出しを加算
            Debug.Log(gameManager.Payout.LastPayoutResult.Payouts);
            gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Payout.LastPayoutResult.Payouts);
            gameManager.Lots.Data.FlagCounter.ResetCounter();
            gameManager.PlayerData.SetLastBonusStart();
        }

        // 払い出し音
        private void PayoutSound()
        {
            // JAC中の払い出し音
            if(gameManager.Bonus.Data.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.JacPayout);
            }
            // 15枚の払い出し音
            else if(gameManager.Payout.LastPayoutResult.Payouts >= 15)
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.MaxPayout);
            }
            else
            {
                gameManager.Sound.PlaySoundLoop(gameManager.Sound.SoundEffectList.NormalPayout);
            }
        }
    }
}
using ReelSpinGame_Bonus;
using ReelSpinGame_Interface;
using System;
using UnityEngine;

namespace ReelSpinGame_State.PayoutState
{
    public class PayoutState : IGameStatement
    {
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

            StartCheckPayout(gameManager.Medal.LastBetAmounts);
            Debug.Log("Payouts result" + gameManager.Payout.LastPayoutResult.Payouts);
            Debug.Log("Bonus:" + gameManager.Payout.LastPayoutResult.BonusID + "ReplayOrJac" + gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

            gameManager.Medal.ResetMedal();

            // 払い出し
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
            gameManager.Medal.HasMedalPayout += gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;
            gameManager.Medal.StartPayout(gameManager.Payout.LastPayoutResult.Payouts);

            // フラッシュを開始させる
            if(gameManager.Payout.LastPayoutResult.Payouts != 0)
            {
                gameManager.Reel.FlashManager.StartPayoutFlash(gameManager.Payout.LastPayoutResult.PayoutLines);
            }

            // ボーナス中なら各ボーナスの払い出しを増やす
            if(gameManager.Bonus.CurrentBonusStatus != BonusManager.BonusStatus.BonusNone)
            {
                gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Payout.LastPayoutResult.Payouts);
            }

            // ボーナスごとに処理を変える
            switch (gameManager.Bonus.CurrentBonusStatus)
            {
                // 小役ゲーム中
                case BonusManager.BonusStatus.BonusBIGGames:

                    // 状態確認
                    gameManager.Bonus.CheckBigGameStatus(gameManager.Payout.LastPayoutResult.IsReplayOrJacIn);

                    // JAC-INまたは小役ゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.CurrentBonusStatus == BonusManager.BonusStatus.BonusJACGames)
                    {
                        gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.JacGame);
                        gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutJAC);
                        gameManager.Medal.ChangeMaxBet(1);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.CurrentBonusStatus == BonusManager.BonusStatus.BonusNone)
                    {
                        gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.Normal);
                        gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.FlagCounter.ResetCounter();
                    }
                    break;

                // ボーナスゲーム中
                case BonusManager.BonusStatus.BonusJACGames:

                    // 状態確認
                    gameManager.Bonus.CheckBonusGameStatus(gameManager.Payout.LastPayoutResult.Payouts > 0);

                    // ボーナスゲームが終わったら抽選テーブル変更
                    if (gameManager.Bonus.CurrentBonusStatus == BonusManager.BonusStatus.BonusBIGGames)
                    {
                        gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.BigBonus);
                        gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutBIG);
                        gameManager.Medal.ChangeMaxBet(3);
                    }
                    //　ボーナスが終了していたら
                    else if (gameManager.Bonus.CurrentBonusStatus == BonusManager.BonusStatus.BonusNone)
                    {
                        gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.Normal);
                        gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutNormal);
                        gameManager.Medal.ChangeMaxBet(3);
                        gameManager.Lots.FlagCounter.ResetCounter();
                    }
                    break;

                // 通常時はリプレイの処理
                // またフラグテーブルの更新

                default:
                    // ボーナスがあればボーナス開始
                    if (gameManager.Payout.LastPayoutResult.BonusID != (int)BonusManager.BonusType.BonusNone)
                    {
                        StartBonus();
                    }
                    // 取りこぼした場合はストックさせる
                    else
                    {
                        // いずれかのボーナスが引かれた場合はストック(BIGまたはREGのどちらかをストックする)
                        // これにより常に成立後出目が出るようになる。
                        if (gameManager.Bonus.HoldingBonusID == BonusManager.BonusType.BonusNone)
                        {
                            // BIG
                            if (gameManager.Lots.CurrentFlag == ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagBig)
                            {
                                gameManager.Bonus.SetBonusStock(BonusManager.BonusType.BonusBIG);
                            }
                            // REG
                            if (gameManager.Lots.CurrentFlag == ReelSpinGame_Lots.Flag.FlagLots.FlagId.FlagReg)
                            {
                                gameManager.Bonus.SetBonusStock(BonusManager.BonusType.BonusREG);
                            }
                        }
                    }

                    // リプレイ
                    if (gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
                    {
                        // 最後に賭けた枚数をOUTに反映
                        gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal(gameManager.Medal.LastBetAmounts);
                        gameManager.Medal.SetReplay();
                    }
                    else if (gameManager.Medal.HasReplay)
                    {
                        gameManager.Medal.DisableReplay();
                    }

                    // フラグ管理
                    // 小役が当選していたら増加させる
                    // リプレイでは増やさない(0増加)
                    if(gameManager.Payout.LastPayoutResult.Payouts > 0 || gameManager.Payout.LastPayoutResult.IsReplayOrJacIn)
                    {
                        gameManager.Lots.FlagCounter.IncreaseCounter(gameManager.Payout.LastPayoutResult.Payouts);
                    }
                    // それ以外は減少
                    else
                    {
                        gameManager.Lots.FlagCounter.DecreaseCounter(
                            gameManager.Setting, gameManager.Medal.LastBetAmounts);
                    }
                    break;
            }


            //if (TestFlag == false)
            //{
             //   TestFlag = true;
              //  gameManager.Medal.SetReplay(); // リプレイにする
            //}
        }

        public void StateUpdate()
        {
            if(gameManager.Medal.PayoutAmounts == 0)
            {
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreasePlayerMedal;
                gameManager.Medal.HasMedalPayout -= gameManager.PlayerData.PlayerMedalData.IncreaseOutMedal;
                gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.InsertState);
            }
        }

        public void StateEnd()
        {
            Debug.Log("End Payout State");
        }

        private void StartCheckPayout(int betAmounts)
        {
            if (!gameManager.Reel.IsWorking)
            {
                gameManager.Payout.CheckPayoutLines(betAmounts, gameManager.Reel.LastSymbols);
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
            if(gameManager.Payout.LastPayoutResult.BonusID == (int)BonusManager.BonusType.BonusBIG)
            {
                gameManager.Bonus.StartBigChance();
                gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.BigBonus);
                gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutBIG);
                gameManager.PlayerData.IncreaseBigChance();
            }

            // ボーナスゲーム
            else if (gameManager.Payout.LastPayoutResult.BonusID == (int)BonusManager.BonusType.BonusREG)
            {
                gameManager.Bonus.StartBonusGame();
                gameManager.Medal.ChangeMaxBet(1);
                gameManager.Lots.ChangeTable(ReelSpinGame_Lots.Flag.FlagLots.FlagLotMode.JacGame);
                gameManager.Payout.ChangePayoutCheckMode(ReelSpinGame_Medal.Payout.PayoutChecker.PayoutCheckMode.PayoutJAC);
                gameManager.PlayerData.IncreaseBonusGame();
            }

            // 15枚の払い出しを加算
            Debug.Log(gameManager.Payout.LastPayoutResult.Payouts);
            gameManager.PlayerData.ChangeBonusPayoutToLast(gameManager.Payout.LastPayoutResult.Payouts);
            gameManager.Lots.FlagCounter.ResetCounter();
            gameManager.PlayerData.SetLastBonusStart();
        }
    }
}
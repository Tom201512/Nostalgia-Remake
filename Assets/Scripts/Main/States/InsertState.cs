using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_State.InsertState
{
    public class InsertState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gameManager;

        // コンストラクタ
        public InsertState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Insert;
            this.gameManager = gameManager;
        }

        // func
        public void StateStart()
        {
            //Debug.Log("Start Medal Insert");

            gameManager.Medal.HasMedalInsert += BetSound;

            // リプレイでなければINSERTランプ表示
            if (!gameManager.Medal.GetHasReplay())
            {
                gameManager.Status.TurnOnInsertLamp();
            }
            // リプレイなら処理開始
            else
            {
                gameManager.Medal.StartReplayInsert();
            }
        }

        public void StateUpdate()
        {
            // MAX BET
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.MaxBet]))
            {
                gameManager.Medal.StartMAXBet();
                StopReelFlash();
            }

            // BET2
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetTwo]))
            {
                gameManager.Medal.StartBet(2);
                StopReelFlash();
            }

            // BET1
            if (OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.BetOne]))
            {
                gameManager.Medal.StartBet(1);
                StopReelFlash();
            }

            // ベット枚数がある場合
            if(gameManager.Medal.GetCurrentBet() > 0)
            {
                gameManager.Status.TurnOnStartLamp();

                // 獲得枚数を表示している場合はセグメントを消す
                if (gameManager.Bonus.DisplayingTotalCount)
                {
                    gameManager.Bonus.TurnOffSegments();
                    gameManager.PlayerData.ResetCurrentGame();
                }
            }

            // ベット終了 または MAXBET
            if(OriginalInput.CheckOneKeyInput(gameManager.KeyCodes[(int)GameManager.ControlSets.StartAndMax]))
            {
                // ベットが終了していたら
                if(gameManager.Medal.GetBetFinished())
                {
                    // 投入枚数を反映する
                    gameManager.PlayerData.PlayerMedalData.DecreasePlayerMedal(gameManager.Medal.GetLastBetAmounts());
                    gameManager.PlayerData.PlayerMedalData.IncreaseInMedal(gameManager.Medal.GetLastBetAmounts());

                    // すでにベットされている場合は抽選へ
                    if (gameManager.Medal.GetCurrentBet() > 0)
                    {
                        gameManager.MainFlow.stateManager.ChangeState(gameManager.MainFlow.LotsState);

                        // ボーナス中なら払い出し枚数を減らす
                        if (gameManager.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
                        {
                            gameManager.Bonus.ChangeBonusPayouts(-gameManager.Medal.GetLastBetAmounts());
                            gameManager.PlayerData.ChangeBonusPayoutToLast(-gameManager.Medal.GetLastBetAmounts());
                        }

                        // 連チャン区間にいる場合は連チャン区間枚数を減らす
                        if(gameManager.Bonus.GetHasZone())
                        {
                            gameManager.Bonus.ChangeZonePayouts(-gameManager.Medal.GetLastBetAmounts());
                        }
                    }
                }
                // そうでない場合はMAX BET
                else
                {
                    gameManager.Medal.StartMAXBet();
                    StopReelFlash();
                }
            }
        }

        public void StateEnd()
        {
            //Debug.Log("End Medal Insert");
            gameManager.Status.TurnOffInsertAndStartlamp();
            gameManager.Medal.HasMedalInsert -= BetSound;
            gameManager.Medal.FinishMedalInsert();
        }

        // フラッシュを止める
        private void StopReelFlash()
        {
            ////Debug.Log("Stop Flash");
            gameManager.Reel.StopReelFlash();

            // リール点灯
            // JAC GAME中なら点灯方法を少し変える
            //gameManager.Reel.TurnOnAllReels(true);
            gameManager.Reel.TurnOnAllReels(gameManager.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames);

        }

        // サウンド再生
        private void BetSound()
        {
            gameManager.Sound.PlaySoundOneShot(gameManager.Sound.SoundEffectList.Bet);
        }
    }
}
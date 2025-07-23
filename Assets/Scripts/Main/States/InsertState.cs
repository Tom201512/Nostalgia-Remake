using ReelSpinGame_Interface;
using ReelSpinGame_Util.OriginalInputs;
using static ReelSpinGame_Bonus.BonusSystemData;
using UnityEngine;

namespace ReelSpinGame_State.InsertState
{
    public class InsertState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }
        // ゲームマネージャ
        private GameManager gM;
        // コンストラクタ
        public InsertState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Insert;
            this.gM = gameManager;
        }

        // func
        public void StateStart()
        {
            //Debug.Log("Start Medal Insert");

            gM.Medal.HasMedalInsert += BetSound;

            // リプレイなら処理を開始しリプレイランプ点灯
            if (gM.Medal.GetHasReplay())
            {
                gM.Medal.StartReplayInsert();
                gM.Status.TurnOnReplayLamp();
            }
            // リプレイでなければINSERTランプ表示
            else
            {
                gM.Status.TurnOnInsertLamp();
                gM.Status.TurnOffReplayLamp();
            }
        }

        public void StateUpdate()
        {
            // オートの有無に合わせて操作受付を変える
            if (gM.Auto.HasAuto)
            {
                BetAndStartFunction();
            }
            else
            {
                PlayerControl();
            }
        }

        public void StateEnd()
        {
            //Debug.Log("End Medal Insert");
            gM.Status.TurnOffInsertAndStartlamp();
            gM.Medal.HasMedalInsert -= BetSound;
            gM.Medal.FinishMedalInsert();
        }

        // ベット処理
        private void BetAction(int amounts)
        {
            gM.Medal.StartBet(amounts);
            StopReelFlash();
            // ベットがある場合はランプを消す
            if (gM.Medal.GetCurrentBet() > 0)
            {
                gM.Status.TurnOnStartLamp();

                // 獲得枚数を表示している場合はセグメントを消す
                if (gM.Bonus.DisplayingTotalCount)
                {
                    gM.Bonus.TurnOffSegments();
                    gM.Player.ResetCurrentGame();
                }
            }
        }

        // フラッシュを止める
        private void StopReelFlash()
        {
            ////Debug.Log("Stop Flash");
            gM.Effect.StopReelFlash();
            // リール点灯(JAC中は中段のみ点灯させる)
            gM.Effect.TurnOnAllReels(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames);

        }

        // サウンド再生
        private void BetSound()
        {
            gM.Effect.StartBetEffect();
        }

        // ベット終了とMAXBETを押したときの制御
        private void BetAndStartFunction()
        {
            // ベットが終了していたら
            if (gM.Medal.GetBetFinished())
            {
                // 投入枚数を反映する
                gM.Save.Player.PlayerMedalData.DecreasePlayerMedal(gM.Medal.GetLastBetAmounts());
                gM.Save.Player.PlayerMedalData.IncreaseInMedal(gM.Medal.GetLastBetAmounts());

                // すでにベットされている場合は抽選へ
                if (gM.Medal.GetCurrentBet() > 0)
                {
                    gM.MainFlow.stateManager.ChangeState(gM.MainFlow.LotsState);

                    // ボーナス中なら払い出し枚数を減らす
                    if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
                    {
                        gM.Bonus.ChangeBonusPayouts(-gM.Medal.GetLastBetAmounts());
                        gM.Player.ChangeLastBonusPayouts(-gM.Medal.GetLastBetAmounts());
                    }

                    // 連チャン区間にいる場合は連チャン区間枚数を減らす
                    if (gM.Bonus.GetHasZone())
                    {
                        gM.Bonus.ChangeZonePayouts(-gM.Medal.GetLastBetAmounts());
                    }
                }
            }
            // そうでない場合はMAX BET
            else
            {
                BetAction(gM.Medal.GetMaxBet());
            }
        }

        // プレイヤー操作の管理
        private void PlayerControl()
        {
            // MAX BET
            if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.MaxBet]))
            {
                BetAction(gM.Medal.GetMaxBet());
            }

            // BET2
            if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.BetTwo]))
            {
                BetAction(2);
            }

            // BET1
            if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.BetOne]))
            {
                BetAction(1);
            }

            // ベット終了 または MAXBET
            if (OriginalInput.CheckOneKeyInput(gM.KeyCodes[(int)GameManager.ControlSets.StartAndMax]))
            {
                BetAndStartFunction();
            }
        }
    }
}
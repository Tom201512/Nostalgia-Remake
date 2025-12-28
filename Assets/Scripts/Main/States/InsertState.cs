using ReelSpinGame_AutoPlay;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_State.InsertState
{
    // 投入ステート
    public class InsertState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }        // このゲームの状態

        private GameManager gM;         // ゲームマネージャ

        public InsertState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Insert;
            gM = gameManager;
        }

        public void StateStart()
        {
            // イベント登録
            gM.Medal.HasMedalInsertEvent += OnMedalInserted;

            // リプレイなら処理を開始しリプレイランプ点灯
            if (gM.Medal.HasReplay)
            {
                gM.Medal.StartReplayInsert(gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
                gM.Status.TurnOnReplayLamp();
                gM.Status.TurnOnStartLamp();

                BetEffectCondition condition = new BetEffectCondition();
                condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                gM.Effect.StartBetEffect(condition);
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
            if (!gM.Option.hasOptionMode)
            {
                // オートの有無に合わせて操作受付を変える
                if (gM.Auto.HasAuto)
                {
                    AutoBetBehavior();
                }
                else
                {
                    PlayerControl();
                }
            }
        }

        public void StateEnd()
        {
            // イベント解除
            gM.Medal.HasMedalInsertEvent -= OnMedalInserted;
            // INSERT, STARTランプの消灯
            gM.Status.TurnOffInsertAndStart();
            // メダル処理を終了させる
            gM.Medal.FinishMedalInsert();

            // 設定画面を開けなくする
            gM.Option.ToggleOptionLock(true);
        }

        // ベット処理
        void BetAction(int amount, bool cutCoroutine)
        {
            gM.Medal.StartBet(amount, cutCoroutine);

            // 演出開始
            BetEffectCondition condition = new BetEffectCondition();
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartBetEffect(condition);

            // ベットがある場合はランプを消す
            if (gM.Medal.CurrentBet > 0)
            {
                gM.Status.TurnOnStartLamp();

                // 獲得枚数を表示している場合はセグメントを消す
                if (gM.Bonus.GetIsDisplayingPayout())
                {
                    gM.Bonus.TurnOffSegments();
                    gM.Player.ResetCurrentGame();
                }
            }
        }

        // ベット終了とMAXBETを押したときの制御
        void BetAndStartFunction(bool cutCoroutine)
        {
            // ベットが終了していたら
            if (gM.Medal.IsFinishedBet)
            {
                EndInsertState();
            }
            // そうでない場合はMAX BET
            else
            {
                BetAction(gM.Medal.MaxBetAmount, cutCoroutine);
            }
        }

        // オート中の制御
        void AutoBetBehavior()
        {
            // オート時サウンド再生設定を変更
            gM.Effect.ChangeSoundSettingByAuto(gM.Auto.HasAuto, gM.Auto.CurrentSpeed);

            // ボーナス成立後であれば1枚掛けをする
            if (gM.Bonus.GetHoldingBonusID() != BonusTypeID.BonusNone && gM.Medal.CurrentBet != 1)
            {
                BetAction(1, gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            }
            // その他の設定
            else
            {
                BetAndStartFunction(gM.Auto.CurrentSpeed > AutoSpeedName.Normal);
            }
        }

        // プレイヤー操作の管理
        void PlayerControl()
        {
            // MAX BET
            if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.MaxBet))
            {
                BetAction(gM.Medal.MaxBetAmount, false);
            }
            // BET1
            if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.BetOne))
            {
                BetAction(1, false);
            }
            // BET2
            if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.BetTwo))
            {
                BetAction(2, false);
            }
            // ベット終了 または MAXBET
            if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.StartAndMax))
            {
                BetAndStartFunction(false);
            }
        }

        // ベット終了処理
        void EndInsertState()
        {
            // 投入枚数を反映する(リプレイ時以外)
            if (!gM.Medal.HasReplay)
            {
                gM.Player.PlayerMedalData.DecreasePlayerMedal(gM.Medal.LastBetAmount);
            }

            // IN枚数反映
            gM.Player.PlayerMedalData.IncreaseInMedal(gM.Medal.LastBetAmount);

            // ボーナス中なら払い出し枚数を減らす
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.ChangeBonusPayout(-gM.Medal.LastBetAmount);
            }

            // 連チャン区間にいる場合は連チャン区間枚数を減らす
            if (gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(-gM.Medal.LastBetAmount);
            }

            // 小役カウンタ減少
            // リプレイでは増やさない(0増加)
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.DecreaseCounter(gM.Setting, gM.Medal.LastBetAmount);
            }

            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.LotsState);
        }

        void OnMedalInserted() => gM.Effect.StartPlayBetSound();
    }
}
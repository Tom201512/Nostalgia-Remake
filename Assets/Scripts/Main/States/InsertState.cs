using ReelSpinGame_AutoPlay;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_State.InsertState
{
    // 投入ステート
    public class InsertState : IGameStatement
    {
        private GameManager gM;                 // ゲームマネージャ
        private InputManager inputManager;      // 入力
        private bool hasAutoTurnOff;            // 自動消灯したか


        public InsertState(GameManager gameManager, InputManager inputManager)
        {
            gM = gameManager;
            this.inputManager = inputManager;
            hasAutoTurnOff = false;
        }

        public void StateStart()
        {
            // 入力処理を登録
            inputManager.ActionTriggeredEvent += OnActionTriggered;

            hasAutoTurnOff = false;
            // クレジットを表示
            gM.Medal.UpdateCreditSegment();
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

                // 通常時であればリールとベットランプ、払い出し表示を自動消灯させる
                if(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
                {
                    gM.Effect.StartAutoTurnOff();
                }
            }
        }

        public void StateUpdate()
        {
            // 自動消灯が有効になったらベットランプとリールを消灯
            if(!hasAutoTurnOff && gM.Effect.HasNoControl)
            {
                gM.Effect.StopReelFlash();
                gM.Effect.TurnOffAllReels();
                gM.Medal.DisableMedalBetLamp();
                hasAutoTurnOff = true;
            }

            if (!gM.Option.HasOptionMode)
            {
                // オートの有無に合わせて操作受付を変える
                if (gM.Auto.HasAuto)
                {
                    AutoBetBehavior();
                }
                else
                {
                    // 設定ロックがかかっていれば解除
                    if (gM.Option.LockOptionMode)
                    {
                        gM.Option.ToggleOptionLock(false);
                    }
                }
            }
        }

        public void StateEnd()
        {
            // イベント解除
            gM.Medal.HasMedalInsertEvent -= OnMedalInserted;
            // 入力処理を解除
            inputManager.ActionTriggeredEvent -= OnActionTriggered;
            // INSERT, STARTランプの消灯
            gM.Status.TurnOffInsertAndStart();
            // メダル処理を終了させる
            gM.Medal.FinishMedalInsert();
            // 設定画面を開けなくする
            gM.Option.ToggleOptionLock(true);
        }

        // ベット処理
        void BetAction(int amount, bool isFastAuto)
        {
            // 自動消灯を無効にする
            gM.Effect.StopAutoTurnOff();
            gM.Medal.StartBet(amount, isFastAuto);

            // 通常オートならセグメントを更新する
            if(!isFastAuto && gM.Medal.RemainingBet != 0)
            {
                gM.Medal.StartInsertSegmentUpdate();
            }

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
        void BetAndStartFunction(bool isFastAuto)
        {
            // ベットが終了していたら
            if (gM.Medal.IsFinishedBet)
            {
                EndInsertState();
            }
            // そうでない場合はMAX BET
            else
            {
                BetAction(gM.Medal.MaxBetAmount, isFastAuto);
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

            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.LotsState);
        }

        void OnMedalInserted() => gM.Effect.StartPlayBetSound();

        // 入力に応じた処理をする
        void OnActionTriggered(InputManager.ControlKeys controlKey)
        {
            switch(controlKey)
            {
                case InputManager.ControlKeys.MaxBet:
                    BetAction(gM.Medal.MaxBetAmount, false);
                    break;
                case InputManager.ControlKeys.BetOne:
                    BetAction(1, false);
                    break;
                case InputManager.ControlKeys.BetTwo:
                    BetAction(2, false);
                    break;
                case InputManager.ControlKeys.StartAndMax:
                    BetAndStartFunction(false);
                    break;
            }
        }
    }
}
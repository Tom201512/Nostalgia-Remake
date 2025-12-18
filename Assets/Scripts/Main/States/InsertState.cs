using ReelSpinGame_Interface;
using UnityEngine;
using ReelSpinGame_Effect.Data;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_Bonus.BonusSystemData;
using ReelSpinGame_Effect.Data.Condition;

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
            gM = gameManager;
        }

        // func
        public void StateStart()
        {
            // イベント登録
            gM.Medal.HasMedalInsertEvent += OnMedalInserted;
            // リプレイなら処理を開始しリプレイランプ点灯
            if (gM.Medal.GetHasReplay())
            {
                gM.Medal.StartReplayInsert(gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);
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
            if(!gM.Option.hasOptionMode)
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
            gM.Medal.HasMedalInsertEvent -= OnMedalInserted;
            gM.Status.TurnOffInsertAndStartlamp();
            gM.Medal.FinishMedalInsert();

            // 設定画面を開けなくする
            gM.Option.ToggleOptionLock(true);
            Debug.Log("Option locked");
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
            if (gM.Medal.GetCurrentBet() > 0)
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
            if (gM.Medal.GetBetFinished())
            {
                EndInsertState();
            }
            // そうでない場合はMAX BET
            else
            {
                BetAction(gM.Medal.GetMaxBet(), cutCoroutine);
            }
        }

        // オート中の制御
        void AutoBetBehavior()
        {
            // オート時サウンド再生設定を変更
            gM.Effect.ChangeSoundSettingByAuto(gM.Auto.HasAuto, gM.Auto.AutoSpeedID);

            // ボーナス成立後であれば1枚掛けをする
            if(gM.Bonus.GetHoldingBonusID() != BonusTypeID.BonusNone && gM.Medal.GetCurrentBet() != 1)
            {
                BetAction(1, gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);
            }
            // その他の設定
            else
            {
                BetAndStartFunction(gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal);
            }
        }

        // プレイヤー操作の管理
        void PlayerControl()
        {
            // MAX BET
            if (gM.InputManager.CheckOneKeyInput(InputManager.ControlKeys.MaxBet))
            {
                BetAction(gM.Medal.GetMaxBet(), false);
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
            if(!gM.Medal.GetHasReplay())
            {
                gM.Player.PlayerMedalData.DecreasePlayerMedal(gM.Medal.GetLastBetAmount());
            }

            // IN枚数反映
            gM.Player.PlayerMedalData.IncreaseInMedal(gM.Medal.GetLastBetAmount());

            // ボーナス中なら払い出し枚数を減らす
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.ChangeBonusPayout(-gM.Medal.GetLastBetAmount());
            }

            // 連チャン区間にいる場合は連チャン区間枚数を減らす
            if (gM.Bonus.GetHasZone())
            {
                gM.Bonus.ChangeZonePayout(-gM.Medal.GetLastBetAmount());
            }

            // 小役カウンタ減少
            // リプレイでは増やさない(0増加)
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.DecreaseCounter(gM.Setting, gM.Medal.GetLastBetAmount());
            }

            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.LotsState);
        }

        void OnMedalInserted() => gM.Effect.StartPlayBetSound();
    }
}
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Lots;
using ReelSpinGame_Medal;
using ReelSpinGame_System;
using static ReelSpinGame_Bonus.BonusModel;
using static ReelSpinGame_Payout.PayoutManager;

namespace ReelSpinGame_State.LotsState
{
    // 初期化ステート
    public class InitState : IGameStatement
    {
        private GameManager gM;         // ゲームマネージャ
        private MedalManager medalManager;      // メダル管理

        public InitState(GameManager gameManager, MedalManager medalManager)
        {
            gM = gameManager;
            this.medalManager = medalManager;
        }

        public void StateStart()
        {
            InitializeSlot();    // スロット情報初期化
            ApplyReplay();       // リプレイ状態割り当て
            ApplyBonusStatus();  // ボーナス状態割り当て
            BonusEffectUpdate(); // ボーナス演出の反映

            gM.PlayerUI.UpdatePlayerUI(gM.Player, medalManager); // UI反映
            gM.Option.SetForceFlagSetting(gM.BonusManager.CurrentBonusStatus, gM.BonusManager.HoldingBonusID);  // オプション設定反映
        }

        public void StateUpdate()
        {
            TurnOnBackLight();  // リールライトの点灯(リプレイ、ボーナス中でセーブした場合はつける)
            CheckError();
        }

        public void StateEnd()
        {

        }

        // スロットの初期化
        private void InitializeSlot()
        {
            gM.ChangeSetting(gM.PlayerSave.Setting);                // 設定反映
            gM.Player.LoadSaveData(gM.PlayerSave.Player);           // プレイヤー情報反映
            medalManager.LoadSaveData(gM.PlayerSave.Medal);             // メダル情報反映
            gM.Lots.SetCounterValue(gM.PlayerSave.FlagCounter);     // フラグ数値反映
            gM.Reel.SetReelPos(gM.PlayerSave.LastReelPos);          // リール位置反映
            gM.BonusManager.LoadSaveData(gM.PlayerSave.Bonus);             // ボーナス状態反映
        }

        // リプレイ状態の反映
        private void ApplyReplay()
        {
            if (medalManager.HasReplay)
            {
                medalManager.EnableReplay();
            }
        }

        // リールバックライトの点灯
        void TurnOnBackLight()
        {
            // リプレイ、またはボーナス中ならライトを点灯させる
            if (medalManager.HasReplay || gM.BonusManager.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                gM.Effect.TurnOnAllReels(gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusJACGames);
            }
            // リプレイでなければINSERTランプ表示
            else
            {
                gM.Effect.TurnOffAllReels();
            }
        }

        // ボーナス状態の反映
        void ApplyBonusStatus()
        {
            // ビッグチャンスの場合
            if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotTable.BigBonus);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                medalManager.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }
            // ボーナスゲームの場合
            else if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                medalManager.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotTable.JacGame);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }
            // 通常時の場合
            else if (gM.BonusManager.CurrentBonusStatus == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotTable.Normal);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                medalManager.ChangeMaxBet(3);
            }
        }

        // ボーナス関連演出の反映
        void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.BonusManager.UpdateSegments();
            // ボーナス中演出の開始
            BonusEffectCondition condition = new BonusEffectCondition();
            condition.BigType = gM.BonusManager.BigChanceType;
            condition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
            gM.Effect.StartBonusEffect(condition);
        }

        // エラーチェック
        void CheckError()
        {
            // 初回起動時なら初回起動画面へ移動
            if (gM.IsFirstLaunch)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.FirstLaunchState);
            }
            // 設定値が-1なら設定変更へ移行する
            else if (gM.PlayerSave.Setting == -1)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.ErrorState);
            }
            // 通常時で回転数が規定数に達していたら打ち止め画面に移行
            else if (gM.PlayerSave.Bonus.CurrentBonusStatus == BonusStatus.BonusNone &&
                gM.PlayerSave.Player.TotalGames >= PlayerDatabase.MaximumTotalGames)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.LimitReachedState);
            }
            // それ以外問題がなければ投入ステートへ
            else
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }
    }
}
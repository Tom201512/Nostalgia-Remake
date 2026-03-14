using ReelSpinGame_Bonus;
using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Flag;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using ReelSpinGame_Payout;
using ReelSpinGame_System;

namespace ReelSpinGame_State.LotsState
{
    // 初期化ステート
    public class InitState : IGameStatement
    {
        private GameManager gM;         // ゲームマネージャ

        public InitState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            InitializeSlot();       // スロット情報初期化
            ApplySlotSetting();     // 台設定反映
            ApplyReplay();          // リプレイ状態割り当て
            ApplyBonusStatus();     // ボーナス状態割り当て
            BonusEffectUpdate();    // ボーナス演出の反映
            // UI反映
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.MedalManager);
            // オプション設定反映
            gM.Option.SetForceFlagSetting(gM.BonusManager.CurrentBonusStatus, gM.BonusManager.HoldingBonusID);
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
            gM.Player.LoadSaveData(gM.PlayerSaveDatabase.Player);                   // プレイヤー情報反映
            gM.MedalManager.LoadSaveData(gM.PlayerSaveDatabase.Medal);              // メダル情報反映
            gM.FlagManager.SetCounterValue(gM.PlayerSaveDatabase.FlagCounter);      // フラグ数値反映
            gM.ReelManager.SetReelPos(gM.PlayerSaveDatabase.LastReelPos);           // リール位置反映
            gM.BonusManager.LoadSaveData(gM.PlayerSaveDatabase.Bonus);              // ボーナス状態反映
        }

        // 台設定の反映
        private void ApplySlotSetting()
        {
            if(gM.PlayerSaveDatabase.SlotSetting != FlagModel.SlotSettingErrorValue)
            {
                gM.FlagManager.SetSlotSetting(gM.PlayerSaveDatabase.SlotSetting);
                gM.FlagManager.IsUsingRandomSetting = gM.PlayerSaveDatabase.IsUsingRandom;

                // データの反映
                gM.PlayerSaveDatabase.SlotSetting = gM.FlagManager.CurrentSlotSetting;
                gM.PlayerSaveDatabase.IsUsingRandom = gM.FlagManager.IsUsingRandomSetting;
                gM.Option.UpdateSlotData(gM.PlayerSaveDatabase, gM.Player);
            }
        }

        // リプレイ状態の反映
        private void ApplyReplay()
        {
            if (gM.MedalManager.HasReplay)
            {
                gM.MedalManager.EnableReplay();
            }
        }

        // ボーナス状態の反映
        private void ApplyBonusStatus()
        {
            // ビッグチャンスの場合
            if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusBIGGames)
            {
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.BigBonus);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutBIG);
                gM.MedalManager.ChangeMaxBet(3);
                gM.FlagManager.ResetCounter();
            }
            // ボーナスゲームの場合
            else if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusJACGames)
            {
                gM.MedalManager.ChangeMaxBet(1);
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.JacGame);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutJAC);
            }
            // 通常時の場合
            else if (gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone)
            {
                gM.FlagManager.ChangeTable(FlagModel.FlagLotTable.Normal);
                gM.PayoutManager.ChangePayoutCheckMode(PayoutModel.PayoutCheckMode.PayoutNormal);
                gM.MedalManager.ChangeMaxBet(3);
            }
        }

        // ボーナス関連演出の反映
        private void BonusEffectUpdate()
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
        private void CheckError()
        {
            // 初回起動時なら初回起動画面へ移動
            if (gM.IsFirstLaunch)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.FirstLaunchState);
            }
            // 設定値が-1なら設定変更へ移行する
            else if (gM.PlayerSaveDatabase.SlotSetting == FlagModel.SlotSettingErrorValue)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.ErrorState);
            }
            // 通常時で回転数が規定数に達していたら打ち止め画面に移行
            else if (gM.PlayerSaveDatabase.Bonus.CurrentBonusStatus == BonusModel.BonusStatus.BonusNone &&
                gM.PlayerSaveDatabase.Player.TotalGames >= PlayerDatabase.MaximumTotalGames)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.LimitReachedState);
            }
            // それ以外問題がなければ投入ステートへ
            else
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }

        // リールバックライトの点灯
        private void TurnOnBackLight()
        {
            // リプレイ、またはボーナス中ならライトを点灯させる
            if (gM.MedalManager.HasReplay || gM.BonusManager.CurrentBonusStatus != BonusModel.BonusStatus.BonusNone)
            {
                gM.Effect.TurnOnAllReels(gM.BonusManager.CurrentBonusStatus == BonusModel.BonusStatus.BonusJACGames);
            }
            // リプレイでなければINSERTランプ表示
            else
            {
                gM.Effect.TurnOffAllReels();
            }
        }
    }
}
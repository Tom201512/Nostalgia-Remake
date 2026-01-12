using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Interface;
using ReelSpinGame_Lots;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Payout.PayoutManager;

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
            InitializeSlot();    // スロット情報初期化
            ApplyReplay();       // リプレイ状態割り当て
            ApplyBonusStatus();  // ボーナス状態割り当て
            BonusEffectUpdate(); // ボーナス演出の反映

            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal); // UI反映
            gM.Option.SetForceFlagSetting(gM.Bonus.GetCurrentBonusStatus(), gM.Bonus.GetHoldingBonusID());  // オプション設定反映
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
        void InitializeSlot()
        {
            gM.ChangeSetting(gM.PlayerSave.Setting);                // 設定反映
            gM.Player.LoadSaveData(gM.PlayerSave.Player);           // プレイヤー情報反映
            gM.Medal.LoadSaveData(gM.PlayerSave.Medal);             // メダル情報反映
            gM.Lots.SetCounterValue(gM.PlayerSave.FlagCounter);     // フラグ数値反映
            gM.Reel.SetReelPos(gM.PlayerSave.LastReelPos);          // リール位置反映
            gM.Bonus.LoadSaveData(gM.PlayerSave.Bonus);             // ボーナス状態反映
        }

        // リプレイ状態の反映
        void ApplyReplay()
        {
            if (gM.Medal.HasReplay)
            {
                gM.Medal.EnableReplay();
            }
        }

        // リールバックライトの点灯
        void TurnOnBackLight()
        {
            // リプレイ、またはボーナス中ならライトを点灯させる
            if (gM.Medal.HasReplay || gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Effect.TurnOnAllReels(gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames);
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
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotTable.BigBonus);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gM.Medal.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }

            // ボーナスゲームの場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gM.Medal.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotTable.JacGame);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }

            // 通常時の場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotTable.Normal);
                gM.Payout.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                gM.Medal.ChangeMaxBet(3);
            }
        }

        // ボーナス関連演出の反映
        void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();

            BonusEffectCondition condition = new BonusEffectCondition();
            condition.BigType = gM.Bonus.GetBigChanceType();
            condition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
            gM.Effect.StartBonusEffect(condition);
        }

        // エラーチェック
        void CheckError()
        {
            // 設定値が-1なら設定変更へ移行する
            if (gM.PlayerSave.Setting == -1)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.ErrorState);
            }

            // 回転数が99999Gに達していたらエラーにする
            else if (gM.PlayerSave.Player.TotalGames == MaxRecordPayout)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.ErrorState);
            }

            // それ以外問題がなければ投入ステートへ
            else
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
            }
        }
    }
}
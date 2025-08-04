using ReelSpinGame_Interface;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Payout.PayoutChecker;

namespace ReelSpinGame_State.LotsState
{
    public class InitState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gM;

        // コンストラクタ
        public InitState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Init;
            gM = gameManager;
        }

        public void StateStart()
        {
            //Debug.Log("Start Init State");

            // スロット情報初期化
            InitializeSlot();

            // リプレイ状態割り当て
            ApplyReplay();

            // ボーナス状態割り当て
            ApplyBonusStatus();

            // リールライトの点灯(リプレイ、ボーナス中でセーブした場合はつける)
            TurnOnBackLight();

            // ボーナス演出の反映
            BonusEffectUpdate();

            // UI反映
            gM.PlayerUI.UpdatePlayerUI(gM.Player, gM.Medal);

        }

        public void StateUpdate()
        {
            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.InsertState);
        }

        public void StateEnd()
        {

        }

        // スロットの初期化
        private void InitializeSlot()
        {
            // 設定反映
            gM.ChangeSetting(gM.Save.Setting);
            //Debug.Log("Setting:" + gM.Setting);

            // プレイヤー情報反映
            gM.Player.LoadSaveData(gM.Save.Player);
            // メダル情報反映
            gM.Medal.LoadSaveData(gM.Save.Medal);
            // フラグ数値反映
            gM.Lots.SetCounterValue(gM.Save.FlagCounter);
            // リール位置反映
            gM.Reel.SetReelPos(gM.Save.LastReelPos);
            // ボーナス状態反映
            gM.Bonus.LoadSaveData(gM.Save.Bonus);
            // 演出マネージャーにボーナスの色を割り当てる
            gM.Effect.SetBigColor(gM.Save.Bonus.BigChanceColor);
        }

        // リプレイ状態の反映
        private void ApplyReplay()
        {
            if(gM.Medal.GetHasReplay())
            {
                gM.Medal.EnableReplay();
            }
        }

        // リールバックライトの点灯
        private void TurnOnBackLight()
        {
            // リプレイ、またはボーナス中ならライトを点灯させる
            if (gM.Medal.GetHasReplay() || gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Effect.TurnOnAllReels(false);
            }
            // リプレイでなければINSERTランプ表示
            else
            {
                gM.Effect.TurnOffAllReels();
            }
        }

        // ボーナス状態の反映
        private void ApplyBonusStatus()
        {
            // ビッグチャンスの場合
            if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames)
            {
                gM.Lots.ChangeTable(FlagLotMode.BigBonus);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutBIG);
                gM.Medal.ChangeMaxBet(3);
                gM.Lots.ResetCounter();
            }

            // ボーナスゲームの場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusJACGames)
            {
                gM.Medal.ChangeMaxBet(1);
                gM.Lots.ChangeTable(FlagLotMode.JacGame);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutJAC);
            }

            // 通常時の場合
            else if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                gM.Lots.ChangeTable(FlagLotMode.Normal);
                gM.Reel.ChangePayoutCheckMode(PayoutCheckMode.PayoutNormal);
                gM.Medal.ChangeMaxBet(3);
            }
        }

        // ボーナス関連演出の反映
        private void BonusEffectUpdate()
        {
            // ボーナス中のランプ処理
            gM.Bonus.UpdateSegments();
            // ボーナス中のBGM処理
            gM.Effect.PlayBonusBGM(gM.Bonus.GetCurrentBonusStatus(), false);
        }
    }
}
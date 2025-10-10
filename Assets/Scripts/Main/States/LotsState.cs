using ReelSpinGame_Interface;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Bonus.BonusSystemData;
using UnityEngine;

namespace ReelSpinGame_State.LotsState
{
    public class LotsState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gM;

        // コンストラクタ
        public LotsState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.FlagLots;
            gM = gameManager;
        }

        public void StateStart()
        {
            //Debug.Log("Start Lots.FlagBehaviour.State");

            gM.Lots.StartFlagLots(gM.Setting, gM.Medal.GetLastBetAmount(), gM.Bonus.GetHoldingBonusID());

            // ボーナス中ならここでゲーム数を減らす
            if (gM.Bonus.GetCurrentBonusStatus() != BonusStatus.BonusNone)
            {
                gM.Bonus.DecreaseGames();
            }
            // そうでない場合は通常時のゲーム数を加算
            else
            {
                gM.Player.IncreaseGameValue();
            }

            // 総ゲーム数の加算
            gM.Player.PlayerAnalyticsData.IncreaseTotalAllGameCounts(gM.Bonus.GetCurrentBonusStatus());

            
            // ボーナス当選ならプレイヤー側にデータを作成(後で入賞時のゲーム数をカウントする)
            if (gM.Lots.GetCurrentFlag() == FlagId.FlagBig)
            {
                gM.Player.AddBonusResult(BonusTypeID.BonusBIG);
            }
            else if (gM.Lots.GetCurrentFlag() == FlagId.FlagReg)
            {
                gM.Player.AddBonusResult(BonusTypeID.BonusREG);
            }

            // オートモードがある場合、ここでオート停止位置の設定
            if (gM.Auto.HasAuto)
            {
                gM.Auto.GetAutoStopPos(gM.Lots.GetCurrentFlag(),gM.Bonus.GetHoldingBonusID(), 
                    gM.Bonus.GetRemainingBigGames(), gM.Bonus.GetRemainingJacIn(), gM.Medal.GetLastBetAmount());
            }

            // 設定画面を開けなくする
            gM.Option.ToggleOptionLock(true);
            Debug.Log("Option locked");

            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.WaitState);
        }

        public void StateUpdate()
        {
            //Debug.Log("Update Lots.FlagBehaviour.State");
        }

        public void StateEnd()
        {
            //Debug.Log("End Lots.FlagBehaviour.State");
            if (gM.Wait.HasWait)
            {
                gM.Status.TurnOnWaitLamp();
            }
        }
    }
}
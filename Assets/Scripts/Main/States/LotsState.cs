using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Interface;
using ReelSpinGame_Lots;
using System;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_State.LotsState
{
    // 抽選ステート
    public class LotsState : IGameStatement
    {
        public MainGameFlow.GameStates State { get; }   // ステート名

        private GameManager gM;        // ゲームマネージャ

        public LotsState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.FlagLots;
            gM = gameManager;
        }

        public void StateStart()
        {
            // 強制役フラグの指定があれば強制役を設定する
            if (gM.Option.GetForceFlagSelectID() != -1)
            {
                FlagID selectedFlagID = (FlagID)Enum.ToObject(typeof(FlagID), gM.Option.GetForceFlagSelectID());
                gM.Lots.SetForceFlag(selectedFlagID);
            }
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
            if (gM.Lots.GetCurrentFlag() == FlagID.FlagBig)
            {
                gM.Player.AddBonusResult(BonusTypeID.BonusBIG);
            }
            else if (gM.Lots.GetCurrentFlag() == FlagID.FlagReg)
            {
                gM.Player.AddBonusResult(BonusTypeID.BonusREG);
            }

            // オートモードがある場合、ここでオート停止位置の設定
            if (gM.Auto.HasAuto)
            {
                // 停止順の設定
                gM.Auto.CurrentStopOrder = gM.OptionSave.AutoOptionData.CurrentStopOrder;

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.Lots.GetCurrentFlag();
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)StopOrderID.First];
                autoAICondition.BonusStatus = gM.Bonus.GetCurrentBonusStatus();
                autoAICondition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
                autoAICondition.BigChanceGames = gM.Bonus.GetRemainingBigGames();
                autoAICondition.RemainingJacIn = gM.Bonus.GetRemainingJacIn();
                autoAICondition.BetAmount = gM.Medal.GetLastBetAmount();

                gM.Auto.SetAutoStopPos(autoAICondition);
            }

            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.WaitState);
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {
            if (gM.Wait.HasWait)
            {
                gM.Status.TurnOnWaitLamp();
            }
        }
    }
}
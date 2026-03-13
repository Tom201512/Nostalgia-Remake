using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Interface;
using ReelSpinGame_Lots;
using ReelSpinGame_Medal;
using System;
using static ReelSpinGame_Bonus.BonusModel;

namespace ReelSpinGame_State.LotsState
{
    // 抽選ステート
    public class LotsState : IGameStatement
    {
        private GameManager gM;        // ゲームマネージャ
        private MedalManager medalManager;  // メダル管理

        public LotsState(GameManager gameManager, MedalManager medalManager)
        {
            gM = gameManager;
            this.medalManager = medalManager;
        }

        public void StateStart()
        {
            // 強制役フラグの指定があれば強制役を設定する
            if (gM.Option.GetForceFlagSelectID() != -1)
            {
                FlagID selectedFlagID = (FlagID)Enum.ToObject(typeof(FlagID), gM.Option.GetForceFlagSelectID());
                gM.Lots.SetForceFlag(selectedFlagID);
            }
            gM.Lots.StartFlagLots(gM.Setting, medalManager.LastBetAmount, gM.BonusManager.HoldingBonusID);

            // ボーナス中ならここでゲーム数を減らす
            if (gM.BonusManager.CurrentBonusStatus != BonusStatus.BonusNone)
            {
                gM.BonusManager.DecreaseGames();
            }
            // そうでない場合は通常時のゲーム数を加算
            else
            {
                gM.Player.IncreaseGameValue();
            }

            // 総ゲーム数の加算
            gM.Player.PlayerAnalyticsData.IncreaseTotalAllGameCounts(gM.BonusManager.CurrentBonusStatus);


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
                autoAICondition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                autoAICondition.HoldingBonus = gM.BonusManager.HoldingBonusID;
                autoAICondition.BigChanceGames = gM.BonusManager.RemainingBigGames;
                autoAICondition.RemainingJacIn = gM.BonusManager.RemainingJacIn;
                autoAICondition.BetAmount = medalManager.LastBetAmount;

                gM.Auto.SetAutoStopPos(autoAICondition, gM.OptionSave.AutoOptionData.StopPosLockData);
            }

            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.WaitState);
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
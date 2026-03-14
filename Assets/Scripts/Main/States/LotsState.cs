using ReelSpinGame_AutoPlay;
using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Bonus;
using ReelSpinGame_Flag;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using System;

namespace ReelSpinGame_State.LotsState
{
    // 抽選ステート
    public class LotsState : IGameStatement
    {
        private GameManager gM;        // ゲームマネージャ

        public LotsState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            // 強制役フラグの指定があれば強制役を設定する
            if (gM.Option.GetForceFlagSelectID() != -1)
            {
                FlagModel.FlagID selectedFlagID = (FlagModel.FlagID)Enum.ToObject(typeof(FlagModel.FlagID), gM.Option.GetForceFlagSelectID());
                gM.FlagManager.SetForceFlag(selectedFlagID);
            }
            gM.FlagManager.StartFlagLots(gM.MedalManager.LastBetAmount, gM.BonusManager.HoldingBonusID);

            // ボーナス中ならここでゲーム数を減らす
            if (gM.BonusManager.CurrentBonusStatus != BonusModel.BonusStatus.BonusNone)
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
            if (gM.FlagManager.GetCurrentFlag() == FlagModel.FlagID.FlagBig)
            {
                gM.Player.AddBonusResult(BonusModel.BonusTypeID.BonusBIG);
            }
            else if (gM.FlagManager.GetCurrentFlag() == FlagModel.FlagID.FlagReg)
            {
                gM.Player.AddBonusResult(BonusModel.BonusTypeID.BonusREG);
            }

            // オートモードがある場合、ここでオート停止位置の設定
            if (gM.Auto.HasAuto)
            {
                // 停止順の設定
                gM.Auto.CurrentStopOrder = gM.OptionSave.AutoOptionData.CurrentStopOrder;

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.FlagManager.GetCurrentFlag();
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)StopOrderID.First];
                autoAICondition.BonusStatus = gM.BonusManager.CurrentBonusStatus;
                autoAICondition.HoldingBonus = gM.BonusManager.HoldingBonusID;
                autoAICondition.BigChanceGames = gM.BonusManager.RemainingBigGames;
                autoAICondition.RemainingJacIn = gM.BonusManager.RemainingJacIn;
                autoAICondition.BetAmount = gM.MedalManager.LastBetAmount;

                gM.Auto.SetAutoStopPos(autoAICondition, gM.OptionSave.AutoOptionData.StopPosLockData);
            }

            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.WaitState);
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {
            if (gM.WaitManager.HasWait)
            {
                gM.Status.TurnOnWaitLamp();
            }
        }
    }
}
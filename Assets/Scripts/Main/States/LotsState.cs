using ReelSpinGame_AutoPlay.AI;
using ReelSpinGame_Interface;
using System;
using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_AutoPlay.AutoManager.AutoStopOrder;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

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
            // 強制役フラグの指定があれば強制役を設定する
            if(gM.Option.GetForceFlagSelectID() != -1)
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
                // BIG中の場合、(JAC回数が残り1回, 残りゲーム数が9G以上)ならJACハズシをする
                if (gM.Bonus.GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                    gM.Bonus.GetRemainingBigGames() > 8 && gM.Bonus.GetRemainingJacIn() == 1)
                {
                    gM.Auto.SetAutoOrder(AutoStopOrderOptions.RML);
                }
                // ボーナス成立後であれば左押しに固定する
                else if (gM.Bonus.GetHoldingBonusID() != BonusTypeID.BonusNone)
                {
                    gM.Auto.SetAutoOrder(AutoStopOrderOptions.LMR);
                }
                // それ以外はオプションで設定した押し順を使う
                else
                {
                    gM.Auto.SetAutoOrder(gM.OptionSave.AutoOptionData.AutoStopOrdersID);
                }

                // 条件を作成
                AutoAIConditionClass autoAICondition = new AutoAIConditionClass();
                autoAICondition.Flag = gM.Lots.GetCurrentFlag();
                autoAICondition.FirstPush = gM.Auto.AutoStopOrders[(int)First];
                autoAICondition.HoldingBonus = gM.Bonus.GetHoldingBonusID();
                autoAICondition.BigChanceGames = gM.Bonus.GetRemainingBigGames();
                autoAICondition.RemainingJacIn = gM.Bonus.GetRemainingJacIn();
                autoAICondition.BetAmount = gM.Medal.GetLastBetAmount();

                gM.Auto.GetAutoStopPos(autoAICondition);
            }

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
﻿using ReelSpinGame_Interface;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
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

            gM.Lots.StartFlagLots(gM.Setting, gM.Medal.GetLastBetAmounts());

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

            // ボーナス当選ならプレイヤー側にデータを作成(後で入賞時のゲーム数をカウントする)
            if (gM.Lots.GetCurrentFlag() == FlagId.FlagBig)
            {
                gM.Player.AddBonusResult(BonusType.BonusBIG);
            }
            else if (gM.Lots.GetCurrentFlag() == FlagId.FlagReg)
            {
                gM.Player.AddBonusResult(BonusType.BonusREG);
            }

            gM.MainFlow.stateManager.ChangeState(gM.MainFlow.WaitState);

            // オートモードがある場合、ここでオート停止位置の設定
            if(gM.Auto.HasAuto)
            {
                gM.Auto.GetAutoStopPos(gM.Lots.GetCurrentFlag(),gM.Bonus.GetHoldingBonusID(), 
                    gM.Bonus.GetRemainingBigGames(), gM.Bonus.GetRemainingJacIn());
            }
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
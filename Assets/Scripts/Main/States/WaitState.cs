using UnityEngine;
using ReelSpinGame_Interface;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;

namespace ReelSpinGame_State.LotsState
{
    public class WaitState : IGameStatement
    {
        // var
        // このゲームの状態
        public MainGameFlow.GameStates State { get; }

        // ゲームマネージャ
        private GameManager gM;

        // コンストラクタ
        public WaitState(GameManager gameManager)
        {
            State = MainGameFlow.GameStates.Wait;
            this.gM = gameManager;
        }

        public void StateStart()
        {
            //Debug.Log("Start Wait State");
            // 高速オート時はウェイトを即無効にする
            // ウェイトカットがある場合も即無効にする
            if(gM.Auto.HasAuto && gM.Auto.AutoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                gM.Wait.DisableWaitTimer();
            }
            else if(gM.Wait.HasWaitCut)
            {
                gM.Wait.DisableWaitTimer();
            }

            // ウェイトランプ点灯
            if (gM.Wait.HasWait)
            {
                gM.Status.TurnOnWaitLamp();
                gM.Effect.StartWaitEffect();
            }
        }

        public void StateUpdate()
        {
            // ウェイトが切れるまで待つ
            //Debug.Log("Update Wait State");
            if(!gM.Wait.HasWait)
            {
                gM.MainFlow.stateManager.ChangeState(gM.MainFlow.PlayingState);
            }
        }

        public void StateEnd()
        {
            //Debug.Log("End Wait State");

            // オートの有無で条件を変える
            if(gM.Auto.HasAuto)
            {
                // オート速度が通常ならウェイトタイマーを起動(ウェイトカットあり時は無効)
                if (gM.Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal)
                {
                    gM.Wait.SetWaitTimer();
                }
            }
            else
            {
                gM.Wait.SetWaitTimer();
            }

            // ウェイトランプを切る
            gM.Status.TurnOffWaitLamp();
            gM.Effect.StopLoopSound();
        }
    }
}
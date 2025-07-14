using UnityEngine;
using ReelSpinGame_Interface;

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
            gM.Wait.SetWaitTimer();

            // ウェイトランプを切る
            gM.Status.TurnOffWaitLamp();
            gM.Effect.StopLoopSound();
        }
    }
}
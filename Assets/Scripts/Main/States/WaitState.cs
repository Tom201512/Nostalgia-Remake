using ReelSpinGame_AutoPlay;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;

namespace ReelSpinGame_State.LotsState
{
    // ウェイト状態ステート
    public class WaitState : IGameStatement
    {
        private GameManager gM;                              // ゲームマネージャ

        public WaitState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            // 高速オート時はウェイトを即無効にする
            // ウェイトカットがある場合も即無効にする
            if (gM.Auto.HasAuto && gM.Auto.CurrentSpeed > AutoSpeedName.Normal)
            {
                gM.WaitManager.DisableWaitTimer();
            }
            else if (gM.WaitManager.HasWaitCut)
            {
                gM.WaitManager.DisableWaitTimer();
            }

            // ウェイトランプ点灯
            if (gM.WaitManager.HasWait)
            {
                gM.Status.TurnOnWaitLamp();
                gM.Effect.StartWaitEffect();
            }
        }

        public void StateUpdate()
        {
            // ウェイトが切れるまで待つ
            if (!gM.WaitManager.HasWait)
            {
                gM.MainFlow.StateManager.ChangeState(gM.MainFlow.PlayingState);
            }
        }

        public void StateEnd()
        {
            // オートの有無で条件を変える
            if (gM.Auto.HasAuto)
            {
                // オート速度が通常ならウェイトタイマーを起動(ウェイトカットあり時は無効)
                if (gM.Auto.CurrentSpeed == AutoSpeedName.Normal)
                {
                    gM.WaitManager.SetWaitTimer();
                }
            }
            else
            {
                gM.WaitManager.SetWaitTimer();
            }

            // ウェイトランプを切る
            gM.Status.TurnOffWaitLamp();
            gM.Effect.StopLoopSound();
        }
    }
}
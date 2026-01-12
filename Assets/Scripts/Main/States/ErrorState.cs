using ReelSpinGame_Interface;

namespace ReelSpinGame_State.InsertState
{
    // エラー時のステート
    public class ErrorState : IGameStatement
    {
        private GameManager gM;         // ゲームマネージャ

        public ErrorState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            gM.Option.ToggleOptionLock(true);
            gM.Option.SetOpenButtonInteractive(false);
            gM.Effect.StartErrorEffect();
            gM.LotSetting.OpenSettingSelect();
            gM.LotSetting.SlotSettingChangedEvent += OnSettingChanged;
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {
            gM.Option.ToggleOptionLock(false);
            gM.Option.SetOpenButtonInteractive(true);
            gM.LotSetting.SlotSettingChangedEvent -= OnSettingChanged;
            gM.Bonus.TurnOffSegments();
        }

        void OnSettingChanged(int setting)
        {
            gM.Option.ToggleOptionLock(true);
            gM.ChangeSetting(setting);
            gM.Effect.StopLoopBGM();
            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
        }
    }
}
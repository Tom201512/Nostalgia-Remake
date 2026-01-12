using ReelSpinGame_Interface;
using ReelSpinGame_Save.Database.Option;
using System;

namespace ReelSpinGame_State.LotsState
{
    // 初回起動時のステート
    public class FirstLaunchState : IGameStatement
    {
        private GameManager gM;         // ゲームマネージャ

        public FirstLaunchState(GameManager gameManager)
        {
            gM = gameManager;
        }

        public void StateStart()
        {
            gM.Option.ToggleOptionLock(true);
            gM.Option.SetOpenButtonInteractive(false);
            gM.InitialSetting.OpenInitializeSetting();
            gM.InitialSetting.LanguageChangedEvent += OnLanguageSettingChanged;
            gM.InitialSetting.HowToPlayClosedEvent += OnHowToPlayClosed;
        }

        public void StateUpdate()
        {

        }

        public void StateEnd()
        {
            gM.InitialSetting.HowToPlayClosedEvent -= OnHowToPlayClosed;
            gM.IsFirstLaunch = false;
        }

        // 言語設定が反映された時の処理
        void OnLanguageSettingChanged(int optionID)
        {
            gM.SetFirstLaunchLanguage((LanguageOptionID)Enum.ToObject(typeof(LanguageOptionID), optionID));
        }

        // 遊び方ガイドが閉じられた後の処理
        void OnHowToPlayClosed()
        {
            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.ErrorState);
        }
    }
}
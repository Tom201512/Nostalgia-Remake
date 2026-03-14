using ReelSpinGame_Flag;
using ReelSpinGame_Interface;
using ReelSpinGame_Main;
using UnityEngine;

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
            gM.BonusManager.TurnOffSegments();
        }

        private void OnSettingChanged(int setting)
        {
            gM.Option.ToggleOptionLock(true);
            ApplySlotSetting(setting);
            gM.Effect.StopLoopBGM();
            gM.MainFlow.StateManager.ChangeState(gM.MainFlow.InsertState);
        }

        // 台設定の反映
        private void ApplySlotSetting(int setting)
        {
            gM.FlagManager.SetSlotSetting(setting);
            // ランダム設定を使用したか記録
            gM.FlagManager.IsUsingRandomSetting = setting == FlagModel.RandomSettingValue;

            // データの反映
            gM.PlayerSaveDatabase.SlotSetting = gM.FlagManager.CurrentSlotSetting;
            gM.PlayerSaveDatabase.IsUsingRandom = gM.FlagManager.IsUsingRandomSetting;
            gM.Option.UpdateSlotData(gM.PlayerSaveDatabase, gM.Player);
            Debug.Log("Slot:" + gM.FlagManager.CurrentSlotSetting);
            Debug.Log("Random:" + gM.FlagManager.IsUsingRandomSetting);
        }
    }
}
using ReelSpinGame_Option.Components;
using UnityEngine;

namespace ReelSpinGame_System
{
    // 設定変更ボタン
    public class SettingSelector : MonoBehaviour
    {
        public int CurrentSelect { get; private set; }          // 現在選択している設定値
        private ButtonComponent[] settingButtons;               // 選択ボタン

        // 設定項目が変わったときのイベント
        public delegate void ContentChanged();
        public event ContentChanged ContentChangedEvent;

        void Awake()
        {
            CurrentSelect = -1;
            settingButtons = GetComponentsInChildren<ButtonComponent>();

            foreach (ButtonComponent button in settingButtons)
            {
                button.ButtonPushedEvent += OnSettingButtonPushed;
            }
        }

        void OnDestroy()
        {
            foreach (ButtonComponent button in settingButtons)
            {
                button.ButtonPushedEvent -= OnSettingButtonPushed;
            }
        }

        // 全てのボタンの有効化設定を変更
        public void SetInteractive(bool value)
        {
            foreach (ButtonComponent button in settingButtons)
            {
                button.ToggleInteractive(value);
            }
        }

        // ボタンが押された時の処理
        void OnSettingButtonPushed(int signalID)
        {
            CurrentSelect = signalID;
            ContentChangedEvent?.Invoke();
        }
    }
}
using ReelSpinGame_Option.Button;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // リール配列選択
    public class ReelArraySelector : MonoBehaviour
    {
        public int CurrentSelectPos { get; set; }       // 現在の選択位置

        // リール図柄が選択された時のイベント
        public delegate void ArraySymbolSelected();
        public event ArraySymbolSelected OnArraySymbolSelected;

        private ButtonComponent[] buttons;      // 各種ボタン

        void Awake()
        {
            CurrentSelectPos = -1;

            buttons = GetComponentsInChildren<ButtonComponent>();

            // ボタン登録
            foreach (ButtonComponent button in buttons)
            {
                button.ButtonPushedEvent += OnReelSymbolPressed;
            }
        }

        void OnDestroy()
        {
            // ボタン登録
            foreach (ButtonComponent button in buttons)
            {
                button.ButtonPushedEvent -= OnReelSymbolPressed;
            }
        }

        // 全ての図柄ボタンの有効化設定を変更
        public void SetInteractive(bool value)
        {
            foreach (ButtonComponent button in buttons)
            {
                button.ToggleInteractive(value);
            }
        }

        // リール図柄が押された時の挙動
        void OnReelSymbolPressed(int signalID)
        {
            CurrentSelectPos = signalID;
            OnArraySymbolSelected?.Invoke();
        }
    }
}

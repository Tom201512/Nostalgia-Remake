using ReelSpinGame_Option.Components;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // リール配列選択
    public class ReelArraySelector : MonoBehaviour
    {
        [SerializeField] ReelCursor selectCursorObject;        // 選択カーソルのオブジェクト
        public int CurrentSelectPos { get; private set; }       // 現在の選択位置

        // リール図柄が選択された時のイベント
        public delegate void ArraySymbolSelected();
        public event ArraySymbolSelected ArraySymbolSelectedEvent;

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

        //指定位置にセット
        public void SetPosition(int selectPos)
        {
            CurrentSelectPos = selectPos;
            UpdateCursor();
        }

        // リール図柄が押された時の挙動
        void OnReelSymbolPressed(int signalID)
        {
            CurrentSelectPos = signalID;
            UpdateCursor();
            ArraySymbolSelectedEvent?.Invoke();
        }

        // カーソル位置を更新
        void UpdateCursor()
        {
            selectCursorObject.SetPosition(CurrentSelectPos, 0f);
        }
    }
}

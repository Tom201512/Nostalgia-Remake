using System;
using UnityEngine;

namespace ReelSpinGame_Input
{
    // プレイヤー用入力マネージャー
    public class PlayerInputManager : MonoBehaviour, IInputProvider
    {
        // キー設定
        [SerializeField] private KeyCode maxBetKey = KeyCode.Space;                 // MAXBET
        [SerializeField] private KeyCode betOneKey = KeyCode.Alpha1;                // 1BET
        [SerializeField] private KeyCode betTwoKey = KeyCode.Alpha2;                // 2BET
        [SerializeField] private KeyCode startAndMaxBetKey = KeyCode.UpArrow;       // リール始動(またはMAX BET)
        [SerializeField] private KeyCode stopLeftKey = KeyCode.LeftArrow;           // 左停止
        [SerializeField] private KeyCode stopMiddleKey = KeyCode.DownArrow;         // 中停止
        [SerializeField] private KeyCode stopRightKey = KeyCode.RightArrow;         // 右停止
        [SerializeField] private KeyCode toggleAutoKey = KeyCode.A;                 // オート開始/停止ボタン
        [SerializeField] private KeyCode toggleMenuKey = KeyCode.M;                 // オプションボタン

        public event Action<InputManager.ControlKeys> InputTriggered;            // 何かしらキーが押された時に動作するイベント
        private bool hasInputOnLastFrame;           // 最後のフレームでキー入力があったか

        private void Awake()
        {
            hasInputOnLastFrame = false;
        }

        private void Update()
        {
            // 何も入力がなければ入力を有効にする
            if (!Input.anyKey)
            {
                hasInputOnLastFrame = false;
            }

            // 各キーの操作判定を確認する
            if (CheckOneKeyInput(maxBetKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.MaxBet);
            }

            if (CheckOneKeyInput(betOneKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.BetOne);
            }

            if (CheckOneKeyInput(betTwoKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.BetTwo);
            }

            if (CheckOneKeyInput(startAndMaxBetKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.StartAndMax);
            }

            if (CheckOneKeyInput(stopLeftKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.StopLeft);
            }

            if (CheckOneKeyInput(stopMiddleKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.StopMiddle);
            }

            if (CheckOneKeyInput(stopRightKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.StopRight);
            }

            if (CheckOneKeyInput(toggleAutoKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.ToggleAuto);
            }

            if (CheckOneKeyInput(toggleMenuKey))
            {
                InputTriggered?.Invoke(InputManager.ControlKeys.ToggleMenu);
            }
        }

        // 指定したキーが一度押されたか確認する
        private bool CheckOneKeyInput(KeyCode key)
        {
            // 入力がなければチェック
            if (!hasInputOnLastFrame)
            {
                if (Input.GetKeyDown(key))
                {
                    // 以降はキー入力を受け付けない
                    hasInputOnLastFrame = true;
                    return true;
                }
            }

            return false;
        }
    }
}
using ReelSpinGame_Input;
using System;
using UnityEngine;

// 入力マネージャー
public class InputManager : MonoBehaviour
{
    // 各種操作のシリアライズ
    public enum ControlKeys 
    { 
        MaxBet, 
        BetOne, 
        BetTwo, 
        StartAndMax, 
        StopLeft, 
        StopMiddle, 
        StopRight, 
        ToggleAuto, 
        ToggleMenu
    }

    [SerializeField] PlayerInputManager playerInputManager;        // プレイヤーの操作管理

    public bool isKeyHeld;                         // キーが押されているか
    public event Action<ControlKeys> ActionTriggeredEvent;        // 何かしらの処理がされた時のイベント

    private IInputProvider currentInput;                // 現在の入力元

    private void Awake()
    {
        SwitchToPlayerControl();
    }

    private void Update()
    {
        isKeyHeld = Input.anyKey;
    }

    // 入力をプレイヤー操作に変更する
    public void SwitchToPlayerControl() => SwitchControl(playerInputManager);

    // 入力先の変更
    private void SwitchControl(IInputProvider input)
    {
        if (currentInput != null)
        {
            currentInput.InputTriggered -= OnActionTriggered;
        }

        currentInput = input;
        currentInput.InputTriggered += OnActionTriggered;
    }

    // 処理があった場合の挙動
    private void OnActionTriggered(ControlKeys controlKey)
    {
        ActionTriggeredEvent?.Invoke(controlKey);
    }
}

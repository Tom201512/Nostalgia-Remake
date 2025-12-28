using UnityEngine;

// 入力マネージャー
public class InputManager : MonoBehaviour
{
    // 各種操作のシリアライズ
    public enum ControlKeys { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight, ToggleAuto, ToggleOption }

    // var
    // キー設定
    [SerializeField] private KeyCode maxBetKey;             // MAXBET
    [SerializeField] private KeyCode betOneKey;             // 1BET
    [SerializeField] private KeyCode betTwoKey;             //2BET
    [SerializeField] private KeyCode startAndMaxBetKey;     // リール始動(またはMAX BET)
    [SerializeField] private KeyCode stopLeftKey;         // 左停止
    [SerializeField] private KeyCode stopMiddleKey;       // 中停止
    [SerializeField] private KeyCode stopRightKey;        // 右停止
    [SerializeField] private KeyCode toggleAutoKey;       // オート開始/停止ボタン
    [SerializeField] private KeyCode toggleOptionKey;     // オプションボタン

    private bool hasInputOnLastFrame = false;   // 最後のフレームでキー入力があったか

    // 指定したキーが一度押されたか確認する
    public bool CheckOneKeyInput(ControlKeys controlKey)
    {
        // 何も入力がなければ入力を有効にする
        if (!Input.anyKey)
        {
            hasInputOnLastFrame = false;
        }

        // 入力がなければチェック
        if (!hasInputOnLastFrame)
        {
            KeyCode key = GetKeyCode(controlKey);
            if (Input.GetKeyDown(key))
            {
                hasInputOnLastFrame = true;
                return true;
            }
        }

        return false;
    }

    // 指定したキーを返す
    KeyCode GetKeyCode(ControlKeys controlKeys)
    {
        switch (controlKeys)
        {
            case ControlKeys.MaxBet:
                return maxBetKey;

            case ControlKeys.BetOne:
                return betOneKey;

            case ControlKeys.BetTwo:
                return betTwoKey;

            case ControlKeys.StartAndMax:
                return startAndMaxBetKey;

            case ControlKeys.StopLeft:
                return stopLeftKey;

            case ControlKeys.StopMiddle:
                return stopMiddleKey;

            case ControlKeys.StopRight:
                return stopRightKey;

            case ControlKeys.ToggleAuto:
                return toggleAutoKey;

            case ControlKeys.ToggleOption:
                return toggleOptionKey;

            default:
                return KeyCode.None;
        }
    }
}

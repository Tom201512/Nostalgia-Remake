using UnityEngine;

namespace ReelSpinGame_Util.OriginalInputs
{
    // 本ゲームで使う入力の処理
    public static class OriginalInput
    {
        // 最後のフレームでキー入力があったか
        private static bool hasInputOnLastFrame = false;

        public static bool CheckOneKeyInput(KeyCode keyCode)
        {
            if (!Input.anyKey) { hasInputOnLastFrame = false; }

            if (!hasInputOnLastFrame)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    hasInputOnLastFrame = true;
                    return true;
                }
            }

            return false;
        }
    }
}
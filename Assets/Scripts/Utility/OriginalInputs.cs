using UnityEngine;

namespace ReelSpinGame_Util.OriginalInputs
{
    public static class OriginalInput
    {
        public static bool hasInputOnLastFrame = false;
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
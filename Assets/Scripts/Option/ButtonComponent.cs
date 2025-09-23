using UnityEngine;

namespace ReelSpinGame_Option.Button
{
    public class ButtonComponent : MonoBehaviour
    {
        // ボタンを押したときのイベント
        public delegate void ButtonPushed();
        public event ButtonPushed buttonPushedEvent;

        // ボタンのコンポーネント
        private UnityEngine.UI.Button buttonObj;

        private void Awake()
        {
            buttonObj = GetComponent<UnityEngine.UI.Button>();
        }

        public void TriggerEvent() => buttonPushedEvent?.Invoke();

        public void ToggleInteractive(bool value) => buttonObj.interactable = value;
    }
}


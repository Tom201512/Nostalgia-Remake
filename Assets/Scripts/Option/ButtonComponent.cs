using UnityEngine;

namespace ReelSpinGame_Option.Button
{
    public class ButtonComponent : MonoBehaviour
    {
        // �{�^�����������Ƃ��̃C�x���g
        public delegate void ButtonPushed();
        public event ButtonPushed buttonPushedEvent;

        // �{�^���̃R���|�[�l���g
        private UnityEngine.UI.Button buttonObj;

        private void Awake()
        {
            buttonObj = GetComponent<UnityEngine.UI.Button>();
        }

        public void TriggerEvent() => buttonPushedEvent?.Invoke();

        public void ToggleInteractive(bool value) => buttonObj.interactable = value;
    }
}


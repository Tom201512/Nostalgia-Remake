using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReelSpinGame_Option.Button
{
    public class ButtonComponent : MonoBehaviour, IPointerDownHandler
    {
        // �{�^���p

        // const

        // var

        // �A�C�R��
        [SerializeField] private Image iconImage;
        // �e�L�X�g
        [SerializeField] private TextMeshProUGUI text;

        // �{�^�����������Ƃ��̃C�x���g
        public delegate void ButtonPushed();
        public event ButtonPushed ButtonPushedEvent;

        // �{�^�����������Ԃ�
        public bool CanInteractable {  get; private set; }

        void Awake()
        {
            CanInteractable = false;
            ChangeButtonContentColor(Color.white);
        }

        private void OnMouseOver()
        {
            Debug.Log("Mouse entered");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Interactable :" + CanInteractable);
            if (CanInteractable)
            {
                ButtonPushedEvent?.Invoke();
                //ChangeButtonContentColor(Color.yellow);
                Debug.Log(name + " Pushed");
            }
        }

        private void OnMouseUp()
        {
            ChangeButtonContentColor(Color.white);
        }

        public void ToggleInteractive(bool value) => CanInteractable = value;

        private void ChangeButtonContentColor(Color color)
        {
            if (iconImage != null)
            {
                iconImage.color = color;
                Debug.Log("Changed material color");
            }

            if (text != null)
            {
                text.material.color = color;
                Debug.Log("Changed text color");
            }
        }
    }
}


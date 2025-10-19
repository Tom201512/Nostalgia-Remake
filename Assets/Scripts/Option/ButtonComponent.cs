using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReelSpinGame_Option.Button
{
    public class ButtonComponent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        // ボタン用

        // const
        // アイコン
        [SerializeField] private Image iconImage;
        // テキスト
        [SerializeField] private TextMeshProUGUI text;

        // ボタンを押したときのイベント
        public delegate void ButtonPushed();
        public event ButtonPushed ButtonPushedEvent;

        // ボタンが押せる状態か
        public bool CanInteractable {  get; private set; }

        void Awake()
        {
            CanInteractable = false;
            ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Mouse entered");
            ChangeButtonContentColor(new Color(1,1,1,1f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Mouse leaved");
            ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer:" + eventData.button);
            if(eventData.button == 0)
            {
                Debug.Log("Interactable :" + CanInteractable);
                if (CanInteractable)
                {
                    ButtonPushedEvent?.Invoke();
                    Debug.Log(name + " Pushed");
                }

                ChangeButtonContentColor(new Color(0.5f, 0.5f, 0.5f, 1f));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Mouse up");
            ChangeButtonContentColor(new Color(1, 1, 1, 1f));
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
                text.color = color;
                Debug.Log("Changed text color");
            }
        }
    }
}


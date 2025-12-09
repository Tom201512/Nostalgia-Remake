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
        // 送る信号番号
        [SerializeField] int signalID;

        // ボタンを押したときのイベント
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalID">信号番号</param>
        public delegate void ButtonPushed(int signalID);
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
            if(CanInteractable)
            {
                Debug.Log("Mouse entered");
                ChangeButtonContentColor(new Color(1, 1, 1, 1f));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Mouse leaved");
                ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Pointer:" + eventData.button);
                if (eventData.button == 0)
                {
                    ButtonPushedEvent?.Invoke(signalID);
                    Debug.Log(name + " Pushed");
                    ChangeButtonContentColor(new Color(0.5f, 0.5f, 0.5f, 1f));
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Mouse up");
                ChangeButtonContentColor(new Color(1, 1, 1, 1f));
            }
        }

        public void ToggleInteractive(bool value)
        {
            CanInteractable = value;

            if(CanInteractable)
            {
                ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
            else
            {
                ChangeButtonContentColor(new Color(0.4f, 0.4f, 0.4f, 1f));
            }
        }

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


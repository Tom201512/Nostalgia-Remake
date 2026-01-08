using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReelSpinGame_Option.Components
{
    // ボタンコンポーネント
    public class ButtonComponent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        [SerializeField] private Image iconImage;       // アイコン
        [SerializeField] private TextMeshProUGUI text;  // テキスト
        [SerializeField] int signalID;                  // 送る信号番号

        public bool CanInteractable { get; private set; }      // ボタンが押せる状態か

        float holdingTimes;     // 長押し時の時間

        // ボタンを押したときのイベント
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalID">信号番号</param>
        public delegate void ButtonPushed(int signalID);
        public event ButtonPushed ButtonPushedEvent;

        void Awake()
        {
            CanInteractable = false;
            ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                ChangeButtonContentColor(new Color(1, 1, 1, 1f));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                if (eventData.button == 0)
                {
                    ButtonPushedEvent?.Invoke(signalID);
                    ChangeButtonContentColor(new Color(0.5f, 0.5f, 0.5f, 1f));
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                ChangeButtonContentColor(new Color(1, 1, 1, 1f));
            }
        }

        // 操作可能状態を変更する
        public void ToggleInteractive(bool value)
        {
            CanInteractable = value;

            if (CanInteractable)
            {
                ChangeButtonContentColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
            else
            {
                ChangeButtonContentColor(new Color(0.4f, 0.4f, 0.4f, 1f));
            }
        }

        // ボタンの色を変える
        private void ChangeButtonContentColor(Color color)
        {
            if (iconImage != null)
            {
                iconImage.color = color;
            }

            if (text != null)
            {
                text.color = color;
            }
        }
    }
}


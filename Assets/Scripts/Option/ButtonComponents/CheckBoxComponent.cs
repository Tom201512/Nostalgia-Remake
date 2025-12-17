using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReelSpinGame_Option.Button
{
    // チェックボックス用
    public class CheckBoxComponent : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        // const

        // var
        [SerializeField] private Sprite unselectedImage; // 非選択時のアイコン
        [SerializeField] private Sprite selectedImage; // 選択時のアイコン

        [SerializeField] Image checkBoxImage; // チェックボックスのアイコン画像
        [SerializeField] private TextMeshProUGUI text; // テキスト
        [SerializeField] int signalID; // 送る信号番号

        public bool CanInteractable { get; private set; } // ボタンが押せる状態か
        public bool IsSelected { get; private set; } // 選択状態にあるか
        public int SignalID { get { return signalID; } } // 信号番号

        // ボタンを押したときのイベント
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalID">信号番号</param>
        public delegate void CheckBoxPushed(int signalID);
        public event CheckBoxPushed ButtonPushedEvent;

        void Awake()
        {
            CanInteractable = false;
            IsSelected = false;
            ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            ChangeCheckBoxImageColor(new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        void Start()
        {
            UpdateCheckBoxIcon();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Mouse entered");

                if(IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(1, 1, 0, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(1, 1, 1, 1f));
                }

                ChangeCheckBoxImageColor(new Color(1, 1, 1, 1f));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Mouse leaved");

                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0f, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0.7f, 1f));
                }

                ChangeCheckBoxImageColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Pointer:" + eventData.button);
                if (eventData.button == 0)
                {
                    IsSelected = !IsSelected;
                    ButtonPushedEvent?.Invoke(signalID);
                    Debug.Log(name + " Pushed");
                    Debug.Log("Selected:" + IsSelected);

                    if (IsSelected)
                    {
                        ChangeCheckBoxTextColor(new Color(0.5f, 0.5f, 0f, 1f));
                    }
                    else
                    {
                        ChangeCheckBoxTextColor(new Color(0.5f, 0.5f, 0.5f, 1f));
                    }

                    ChangeCheckBoxImageColor(new Color(0.5f, 0.5f, 0.5f, 1f));
                    UpdateCheckBoxIcon();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CanInteractable)
            {
                Debug.Log("Mouse up");
                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(1, 1, 0, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(1, 1, 1, 1f));
                }

                ChangeCheckBoxImageColor(new Color(1, 1, 1, 1f));
            }
        }

        // 操作可能状態を変更する
        public void ToggleInteractive(bool value)
        {
            CanInteractable = value;

            if (CanInteractable)
            {
                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0f, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0.7f, 1f));
                }

                ChangeCheckBoxImageColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
            else
            {
                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(0.4f, 0.4f, 0f, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(0.4f, 0.4f, 0.4f, 1f));
                }

                ChangeCheckBoxImageColor(new Color(0.4f, 0.4f, 0.4f, 1f));
            }
        }

        // 選択状態を変更する
        public void ToggleSelecting(bool value)
        {
            IsSelected = value;

            if (CanInteractable)
            {
                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0f, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(0.7f, 0.7f, 0.7f, 1f));
                }

                ChangeCheckBoxImageColor(new Color(0.7f, 0.7f, 0.7f, 1f));
            }
            else
            {
                if (IsSelected)
                {
                    ChangeCheckBoxTextColor(new Color(0.4f, 0.4f, 0f, 1f));
                }
                else
                {
                    ChangeCheckBoxTextColor(new Color(0.4f, 0.4f, 0.4f, 1f));
                }

                ChangeCheckBoxImageColor(new Color(0.4f, 0.4f, 0.4f, 1f));
            }

            UpdateCheckBoxIcon();
        }

        // 画像の更新
        void UpdateCheckBoxIcon()
        {
            if(IsSelected)
            {
                checkBoxImage.sprite = selectedImage;
            }
            else
            {
                checkBoxImage.sprite = unselectedImage;
            }
        }

        // 説明文の色を変更する
        void ChangeCheckBoxTextColor(Color color)
        {
            if (text != null)
            {
                text.color = color;
                Debug.Log("Changed text color");
            }
        }

        // 画像の色を変更する
        void ChangeCheckBoxImageColor(Color color)
        {
            if (checkBoxImage != null)
            {
                checkBoxImage.color = color;
                Debug.Log("Changed icon color");
            }
        }
    }
}


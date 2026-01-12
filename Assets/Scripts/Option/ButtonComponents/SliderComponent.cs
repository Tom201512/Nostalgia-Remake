using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option.Components
{
    // スライダー部分のコンポーネント
    public class SliderComponent : MonoBehaviour
    {
        [SerializeField] Slider slider;                 // スライダー部分
        [SerializeField] TextMeshProUGUI valueText;     // 数値テキスト
        [SerializeField] ButtonComponent upButton;      // 増加ボタン
        [SerializeField] ButtonComponent downButton;      // 減少ボタン

        // スライダー数値が変わったときのイベント
        public delegate void SliderValueChanged();
        public event SliderValueChanged SliderValueChangedEvent;

        public int CurrentSliderValue { get; private set; }         // スライダーの数値
        public bool CanInteractable { get; private set; }           // 操作可能か

        void Awake()
        {
            upButton.ButtonPushedEvent += OnUpPressedBehavior;
            upButton.ButtonHoldEvent += OnUpPressedBehavior;
            downButton.ButtonPushedEvent += OnDownPressedBehavior;
            downButton.ButtonHoldEvent += OnDownPressedBehavior;
            slider.onValueChanged.AddListener(delegate { SliderChangeEvent(); });
            CanInteractable = false;
        }

        void Start()
        {
            UpdateScreen();
        }

        void OnDestroy()
        {
            upButton.ButtonPushedEvent -= OnUpPressedBehavior;
            upButton.ButtonHoldEvent -= OnUpPressedBehavior;
            downButton.ButtonPushedEvent -= OnDownPressedBehavior;
            downButton.ButtonHoldEvent -= OnDownPressedBehavior;
            slider.onValueChanged.RemoveListener(delegate { SliderChangeEvent(); });
        }

        // 選択ボタンの有効化設定
        public void SetInteractive(bool value)
        {
            upButton.ToggleInteractive(value);
            downButton.ToggleInteractive(value);
            CanInteractable = value;
        }

        public void SetSliderBarValue(int value)
        {
            CurrentSliderValue = value;
            slider.value = value;
            UpdateScreen();
        }

        // 増加ボタンの処理
        void OnUpPressedBehavior(int signalID)
        {
            if (slider.value < slider.maxValue)
            {
                slider.value += 1;
                CurrentSliderValue = (int)slider.value;
                UpdateScreen();
                SliderValueChangedEvent?.Invoke();
            }
        }

        // 減少ボタンの処理
        void OnDownPressedBehavior(int signalID)
        {
            if (slider.value > slider.minValue)
            {
                slider.value -= 1;
                CurrentSliderValue = (int)slider.value;
                UpdateScreen();
                SliderValueChangedEvent?.Invoke();
            }
        }

        // スライダー本体が変更された時のイベント
        void SliderChangeEvent()
        {
            CurrentSliderValue = (int)slider.value;
            UpdateScreen();
            if (CanInteractable)
            {
                SliderValueChangedEvent?.Invoke();
            }
        }

        // テキストの更新
        void UpdateScreen()
        {
            valueText.text = CurrentSliderValue + "%";
        }
    }
}

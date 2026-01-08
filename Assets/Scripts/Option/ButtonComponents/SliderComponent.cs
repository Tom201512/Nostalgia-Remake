using System.Collections;
using System.Collections.Generic;
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

        void Awake()
        {
            
        }

        void OnDestroy()
        {
            
        }

        // スライダーの数値
        public int CurrentSliderValue
        {
            get => (int)slider.value;
            set => slider.value = value;
        }

        // 増加ボタンの挙動
        void OnUpPressedBehavior(int signalID) => slider.value += 1;

        // 減少ボタンの挙動
        void OnDownPressedBehavior(int signalID) => slider.value -= 1;
    }
}

using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Reels;
using ReelSpinGame_UI.Reel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.AutoSetting
{
    // オート停止位置の設定
    public class AutoStopPosLockManager : MonoBehaviour, IOptionScreenBase
    {
        [SerializeField] ReelArraySelector reelArrayLeft;       // リール配列セレクタ(左)
        [SerializeField] ReelArraySelector reelArrayMiddle;     // リール配列セレクタ(中)
        [SerializeField] ReelArraySelector reelArrayRight;      // リール配列セレクタ(右)
        [SerializeField] TextMeshProUGUI selectingPosText;       // 選択位置のテキスト

        [SerializeField] ButtonComponent confirmButton;         // 決定ボタン
        [SerializeField] ButtonComponent resetButton;           // リセットボタン

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 選択位置
        public int CurrentLeftSelect { get => reelArrayLeft.CurrentSelectPos; }         // 左
        public int CurrentMiddleSelect { get => reelArrayMiddle.CurrentSelectPos; }     // 中
        public int CurrentRightSelect { get => reelArrayRight.CurrentSelectPos; }       // 右

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            reelArrayLeft.OnArraySymbolSelected += OnArraySettingChanged;
            reelArrayMiddle.OnArraySymbolSelected += OnArraySettingChanged;
            reelArrayRight.OnArraySymbolSelected += OnArraySettingChanged;
            confirmButton.OnButtonPushedEvent += OnConfirmPressed;
            resetButton.OnButtonPushedEvent += OnResetPressed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnDestroy()
        {
            reelArrayLeft.OnArraySymbolSelected -= OnArraySettingChanged;
            reelArrayMiddle.OnArraySymbolSelected -= OnArraySettingChanged;
            reelArrayRight.OnArraySymbolSelected -= OnArraySettingChanged;
            confirmButton.OnButtonPushedEvent -= OnConfirmPressed;
            resetButton.OnButtonPushedEvent -= OnResetPressed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            StartCoroutine(nameof(FadeInBehavior));
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                confirmButton.ToggleInteractive(false);
                resetButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 設定のセット
        public void LoadOptionData(List<int> stopPosLockSetting)
        {
            reelArrayLeft.SetPosition(stopPosLockSetting[(int)ReelID.ReelLeft]);
            reelArrayMiddle.SetPosition(stopPosLockSetting[(int)ReelID.ReelMiddle]);
            reelArrayRight.SetPosition(stopPosLockSetting[(int)ReelID.ReelRight]);
            UpdateScreen();
        }

        // 画面の更新
        void UpdateScreen()
        {
            selectingPosText.text = "L:" + CurrentLeftSelect + " M:" + CurrentMiddleSelect + " R:" + CurrentRightSelect;
        }

        // 決定ボタンを押したときの挙動
        void OnConfirmPressed(int signalID) => CloseScreen();

        // リセットボタンを押したときの挙動
        void OnResetPressed(int signalID)
        {
            if (CanInteract)
            {
                reelArrayLeft.SetPosition(-1);
                reelArrayMiddle.SetPosition(-1);
                reelArrayRight.SetPosition(-1);
                UpdateScreen();
            }
        }

        // 設定が変更された時の挙動
        void OnArraySettingChanged()
        {
            UpdateScreen();
            SettingChangedEvent?.Invoke();
        }

        // フェードイン
        IEnumerator FadeInBehavior()
        {
            canvasGroup.alpha = 0;
            float fadeSpeed = Time.deltaTime / OptionScreenFade.FadeTime;

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha = Math.Clamp(canvasGroup.alpha + fadeSpeed, 0f, 1f);
                yield return new WaitForEndOfFrame();
            }

            CanInteract = true;
            confirmButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
            reelArrayLeft.SetInteractive(true);
            reelArrayMiddle.SetInteractive(true);
            reelArrayRight.SetInteractive(true);
        }

        // フェードアウト
        IEnumerator FadeOutBehavior()
        {
            canvasGroup.alpha = 1;
            float fadeSpeed = Time.deltaTime / OptionScreenFade.FadeTime;

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = Math.Clamp(canvasGroup.alpha - fadeSpeed, 0f, 1f);
                yield return new WaitForEndOfFrame();
            }

            ClosedScreenEvent?.Invoke();
        }
    }
}

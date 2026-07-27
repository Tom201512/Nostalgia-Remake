using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.MenuContent;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_System
{
    // 設定変更用
    public class LanguageSelectScreen : MonoBehaviour
    {
        [SerializeField] ButtonSelector languageSelector;               // 言語選択
        [SerializeField] TextMeshProUGUI selectLanguageText;            // 選択中のテキスト
        [SerializeField] ButtonComponent confirmButton;                 // 決定ボタン
        [SerializeField] TextMeshProUGUI confirmButtonText;             // 決定ボタンのテキスト

        [SerializeField] List<String> selectDisplayList;                // 言語選択テキストの内容
        [SerializeField] List<String> confirmDisplayList;               // 決定ボタンテキストの内容

        public int CurrentSetting { get => languageSelector.CurrentSelect; }     // 現在の設定値

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        private bool CanInteract;                   // 操作ができる状態か(アニメーション中などはつけないこと)
        private CanvasGroup canvasGroup;            // フェードイン、アウト用

        void Awake()
        {
            CanInteract = false;
            canvasGroup = GetComponent<CanvasGroup>();
            languageSelector.ContentChangedEvent += SettingChangedBehavior;
            confirmButton.ButtonPushedEvent += ConfirmPressedBehavior;
        }

        void Start()
        {
            UpdateScreen();
            confirmButton.ToggleInteractive(false);
        }

        void OnDestroy()
        {
            languageSelector.ContentChangedEvent -= SettingChangedBehavior;
            confirmButton.ButtonPushedEvent -= ConfirmPressedBehavior;
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
                languageSelector.SetInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 画面更新
        void UpdateScreen()
        {
            if (CurrentSetting == -1)
            {
                selectLanguageText.gameObject.SetActive(false);
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                // 対応するテキストがあれば表示する
                if (CurrentSetting < selectDisplayList.Count)
                {
                    selectLanguageText.gameObject.SetActive(true);
                    selectLanguageText.text = selectDisplayList[CurrentSetting];
                    confirmButton.gameObject.SetActive(true);
                    confirmButtonText.text = confirmDisplayList[CurrentSetting];
                }
            }
        }

        // 設定変更ボタンが押された時の処理
        void SettingChangedBehavior()
        {
            confirmButton.ToggleInteractive(true);
            UpdateScreen();
        }

        // 決定ボタンが押された時の処理
        void ConfirmPressedBehavior(int signalID)
        {
            CloseScreen();
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
            languageSelector.SetInteractive(true);
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

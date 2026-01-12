using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.MenuContent;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ReelSpinGame_System
{
    // 設定変更用
    public class SettingSelectScreen : MonoBehaviour
    {
        [SerializeField] SettingSelector settingSelector;               // 設定値選択のボタン
        [SerializeField] TextMeshProUGUI settingProbabilityText;        // 設定ごとの確率を表示
        [SerializeField] ButtonComponent confirmButton;                 // 決定ボタン
        [SerializeField] List<LocalizedString> textDisplayList;         // テキストのローカライズリスト

        public int CurrentSetting { get => settingSelector.CurrentSelect; }     // 現在の設定値

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        private bool CanInteract;                   // 操作ができる状態か(アニメーション中などはつけないこと)
        private CanvasGroup canvasGroup;            // フェードイン、アウト用

        // ローカライズ
        LocalizeStringEvent selectTextLocalize;    // 選択項目のテキスト

        void Awake()
        {
            CanInteract = false;
            selectTextLocalize = settingProbabilityText.GetComponent<LocalizeStringEvent>();
            canvasGroup = GetComponent<CanvasGroup>();
            settingSelector.ContentChangedEvent += SettingChangedBehavior;
            confirmButton.ButtonPushedEvent += ConfirmPressedBehavior;
        }

        void Start()
        {
            UpdateScreen();
            confirmButton.ToggleInteractive(false);
        }

        void OnDestroy()
        {
            settingSelector.ContentChangedEvent -= SettingChangedBehavior;
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
                settingSelector.SetInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 画面更新
        void UpdateScreen()
        {
            if (CurrentSetting == -1)
            {
                settingProbabilityText.gameObject.SetActive(false);
            }
            else
            {
                // 対応するテキストがあれば表示する
                if (CurrentSetting < textDisplayList.Count)
                {
                    settingProbabilityText.gameObject.SetActive(true);
                    selectTextLocalize.StringReference = textDisplayList[CurrentSetting];
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
            settingSelector.SetInteractive(true);
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

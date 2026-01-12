using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.OtherSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ReelSpinGame_Option.MenuContent
{
    // 遊び方ガイド画面
    public class HowToPlayScreen : MonoBehaviour, IOptionScreenBase
    {
        [SerializeField] ButtonComponent nextButton;        // 次ボタン
        [SerializeField] ButtonComponent previousButton;    // 前ボタン
        [SerializeField] ButtonComponent closeButton;       // クローズボタン
        [SerializeField] TextMeshProUGUI pageCount;         // ページ表記

        // ローカライズ用
        [SerializeField] List<LocalizedString> titleTextList;   // テキストリスト
        [SerializeField] List<LocalizedSprite> screenList;      // 画像リスト
        [SerializeField] LocalizeStringEvent titleText;         // タイトルテキスト
        [SerializeField] LocalizeSpriteEvent screen;            // 表示する画面

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        private int currentPage;            // 表示中のページ番号
        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            currentPage = 0;
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            UpdateScreen();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            nextButton.ButtonPushedEvent -= OnNextPushed;
            previousButton.ButtonPushedEvent -= OnPreviousPushed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            currentPage = 0;
            UpdateScreen();
            StartCoroutine(nameof(FadeInBehavior));
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                SetInteractiveButtons(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 全てのボタンの操作をコントロールする
        void SetInteractiveButtons(bool value)
        {
            closeButton.ToggleInteractive(value); ;
            nextButton.ToggleInteractive(value);
            previousButton.ToggleInteractive(value);
        }

        // 次ボタンを押したときの処理
        void OnNextPushed(int signalID)
        {
            if (currentPage + 1 == screenList.Count)
            {
                currentPage = 0;
            }
            else
            {
                currentPage += 1;
            }

            UpdateScreen();
        }

        // 前ボタンを押したときの処理
        void OnPreviousPushed(int signalID)
        {
            if (currentPage - 1 < 0)
            {
                currentPage = screenList.Count - 1;
            }
            else
            {
                currentPage -= 1;
            }

            UpdateScreen();
        }

        // 閉じるボタンを押したときの処理
        void OnClosedPressed(int signalID) => CloseScreen();

        // 画像の反映処理
        void UpdateScreen()
        {
            titleText.StringReference = titleTextList[currentPage];
            screen.AssetReference = screenList[currentPage];
            pageCount.text = (currentPage + 1) + "/" + screenList.Count;
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
            SetInteractiveButtons(true);
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

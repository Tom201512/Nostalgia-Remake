using ReelSpinGame_Option.Components;
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
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

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
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 次ボタンを押したときの挙動
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

        // 前ボタンを押したときの挙動
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

        // 閉じるボタンを押したときの挙動
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
            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
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

            OnClosedScreenEvent?.Invoke();
        }
    }
}

using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace ReelSpinGame_Option.MenuContent
{
    // 遊び方ガイド画面
    public class HowToPlayScreen : MonoBehaviour, IOptionScreenBase
    {
        // const

        // var
        [SerializeField] ButtonComponent nextButton;        // 次ボタン
        [SerializeField] ButtonComponent previousButton;    // 前ボタン
        [SerializeField] ButtonComponent closeButton;       // クローズボタン
        [SerializeField] TextMeshProUGUI pageCount;         // ページ表記

        // ローカライズ用
        [SerializeField] List<LocalizedString> titleTextList; // テキストリスト
        [SerializeField] List<LocalizedSprite> screenList; // 画像リスト
        [SerializeField] LocalizeStringEvent titleText; // タイトルテキスト
        [SerializeField] LocalizeSpriteEvent screen;    // 表示する画面

        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

        private int currentPage;    // 表示中のページ番号

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func
        void Awake()
        {
            currentPage = 0;
            // ボタン登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
        }

        void Start()
        {
            UpdateScreen();
        }

        void OnDestroy()
        {
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            nextButton.ButtonPushedEvent -= OnNextPushed;
            previousButton.ButtonPushedEvent -= OnPreviousPushed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            CanInteract = true;
            currentPage = 0;

            UpdateScreen();

            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
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
        void OnClosedPressed(int signalID) => OnClosedScreenEvent?.Invoke();

        // 画像の反映処理
        void UpdateScreen()
        {
            titleText.StringReference = titleTextList[currentPage];
            screen.AssetReference = screenList[currentPage];
        }
    }
}

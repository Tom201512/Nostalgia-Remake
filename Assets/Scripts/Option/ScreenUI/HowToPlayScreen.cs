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
    public class HowToPlayScreen : MonoBehaviour, IOptionScreenBase
    {
        // 遊び方ガイド画面

        // const

        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

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

        private int currentPage;    // 表示中のページ番号

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func

        private void Awake()
        {
            currentPage = 0;
            // ボタン登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
        }

        private void Start()
        {
            UpdateScreen();
        }

        private void OnDestroy()
        {
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            nextButton.ButtonPushedEvent -= OnNextPushed;
            previousButton.ButtonPushedEvent -= OnPreviousPushed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            Debug.Log("Initialized How To Play");
            CanInteract = true;
            Debug.Log("Interact :" + CanInteract);
            currentPage = 0;

            UpdateScreen();

            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if (CanInteract)
            {
                Debug.Log("Closed How To Play");
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
            }
        }

        // 次ボタンを押したときの挙動
        private void OnNextPushed(int signalID)
        {
            Debug.Log("Next pressed");
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
        private void OnPreviousPushed(int signalID)
        {
            Debug.Log("Previous pressed");
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
        private void OnClosedPressed(int signalID) => OnClosedScreenEvent?.Invoke();

        // 画像の反映処理
        private void UpdateScreen()
        {
            Debug.Log("Page:" + currentPage + 1);
            titleText.StringReference = titleTextList[currentPage];
            screen.AssetReference = screenList[currentPage];
            
            pageCount.text = (currentPage + 1) + "/" + screenList.Count;
        }
    }
}

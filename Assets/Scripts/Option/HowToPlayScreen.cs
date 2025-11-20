using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        // スクリーン
        [SerializeField] private Image screen;
        // タイトル部分
        [SerializeField] private TextMeshProUGUI titleText;
        // 表示する画面
        [SerializeField] private List<GameObject> screenList;
        // 次ボタン
        [SerializeField] private ButtonComponent nextButton;
        // 前ボタン
        [SerializeField] private ButtonComponent previousButton;
        // クローズボタン
        [SerializeField] private ButtonComponent closeButton;
        // ページ表記
        [SerializeField] private TextMeshProUGUI pageCount;

        // 表示中のページ番号
        private int currentPage = 0;

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func

        private void Awake()
        {
            // ボタン登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
        }

        private void Update()
        {
            if (CanInteract)
            {
                if (OriginalInput.CheckOneKeyInput(KeyCode.RightArrow))
                {
                    OnNextPushed();
                }
                if (OriginalInput.CheckOneKeyInput(KeyCode.LeftArrow))
                {
                    OnPreviousPushed();
                }
            }
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
        private void OnNextPushed()
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
        private void OnPreviousPushed()
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
        private void OnClosedPressed() => OnClosedScreenEvent?.Invoke();

        // 画像の反映処理
        private void UpdateScreen()
        {
            Debug.Log("Page:" + currentPage + 1);
            CloseAllScreen();
            screenList[currentPage].SetActive(true);
            
            pageCount.text = (currentPage + 1) + "/" + screenList.Count;
        }

        // 全てのページを非表示にする
        private void CloseAllScreen()
        {
            foreach(GameObject screen in screenList)
            {
                screen.SetActive(false);
            }
        }
    }
}

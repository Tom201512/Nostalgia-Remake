using ReelSpinGame_Datas;
using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotDataScreen : MonoBehaviour, IOptionScreenBase
    {
        // スロット情報画面

        // const
        private const int maxPage = 4;

        // var

        // 各種画面
        // 情報表示
        [SerializeField] private SlotMainDataUI slotMainDataUI;


        // 次ボタン
        [SerializeField] private ButtonComponent nextButton;
        // 前ボタン
        [SerializeField] private ButtonComponent previousButton;
        // クローズボタン
        [SerializeField] private ButtonComponent closeButton;
        // ページ表記
        [SerializeField] private TextMeshProUGUI pageCount;

        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // 表示中のページ番号
        private int currentPage = 0;

        // プレイヤーデータのアドレス
        private PlayerDatabase playerData;

        // func
        private void Awake()
        {
            // ボタン登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
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
            Debug.Log("Initialized SlotData");
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
                Debug.Log("Closed SlotData");
                closeButton.ToggleInteractive(false); ;
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
            }
        }

        // データを受け渡す
        public void SendData(PlayerDatabase player)
        {
            playerData = player;
        }

        // 次ボタンを押したときの挙動
        private void OnNextPushed()
        {
            Debug.Log("Next pressed");
            if (currentPage + 1 == maxPage)
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
                currentPage = maxPage - 1;
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
            // ページごとに処理を行う

            switch (currentPage)
            {
                case 0:
                    slotMainDataUI.UpdateText(playerData);
                    break;

                default:
                    break;
            }
        }
    }
}

using ReelSpinGame_Option.Components;
using ReelSpinGame_System;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // スロット情報画面
    public class SlotDataScreen : MonoBehaviour, IOptionScreenBase
    {
        const int maxPage = 5;      // 最大ページ数

        // 各種画面
        [SerializeField] private SlotMainDataUI slotMainDataUI;             // メイン情報
        [SerializeField] private ProbabilityDataUI probabilityDataUI;       // 通常時小役確率
        [SerializeField] private BonusDataUI bonusDataUI;                   // 直近ボーナス情報、JACハズシ確率など
        [SerializeField] private SlumpGraphDataUI slumpGraphDataUI;         // スランプグラフ
        [SerializeField] private BonusRecordDataUI bonusRecordDataUI;       // ボーナス履歴

        [SerializeField] private ButtonComponent nextButton;        // 次ボタン
        [SerializeField] private ButtonComponent previousButton;    // 前ボタン
        [SerializeField] private ButtonComponent closeButton;       // クローズボタン
        [SerializeField] private TextMeshProUGUI pageCount;         // ページ表記

        public PlayerDatabase PlayerData;        // プレイヤーデータのアドレス

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        private int currentPage = 0;        // 表示中のページ番号
        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            // ボタン登録
            closeButton.OnButtonPushedEvent += OnClosedPressed;
            nextButton.OnButtonPushedEvent += OnNextPushed;
            previousButton.OnButtonPushedEvent += OnPreviousPushed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            UpdateScreen();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.OnButtonPushedEvent -= OnClosedPressed;
            nextButton.OnButtonPushedEvent -= OnNextPushed;
            previousButton.OnButtonPushedEvent -= OnPreviousPushed;
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

        // データを受け渡す
        public void SendData(PlayerDatabase player)
        {
            PlayerData = player;
        }

        // 次ボタンを押したときの挙動
        void OnNextPushed(int signalID)
        {
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
        void OnPreviousPushed(int signalID)
        {
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
        void OnClosedPressed(int signalID) => CloseScreen();

        // 画像の反映処理
        void UpdateScreen()
        {
            DisactivateAllScreen();
            // ページごとに処理を行う

            switch (currentPage)
            {
                case 0:
                    slotMainDataUI.gameObject.SetActive(true);
                    slotMainDataUI.UpdateText(PlayerData);
                    break;

                case 1:
                    probabilityDataUI.gameObject.SetActive(true);
                    probabilityDataUI.UpdateText(PlayerData);
                    break;

                case 2:
                    bonusDataUI.gameObject.SetActive(true);
                    bonusDataUI.UpdateText(PlayerData);
                    bonusDataUI.DisplayWinningPattern(PlayerData);
                    break;

                case 3:
                    slumpGraphDataUI.gameObject.SetActive(true);
                    slumpGraphDataUI.UpdateData(PlayerData);
                    break;

                case 4:
                    bonusRecordDataUI.gameObject.SetActive(true);
                    bonusRecordDataUI.GenerateBonusResult(PlayerData.BonusHitRecord);
                    break;

                default:
                    break;
            }

            pageCount.text = (currentPage + 1) + "/" + maxPage;
        }

        // 全ての画面を非アクティブにする
        void DisactivateAllScreen()
        {
            bonusRecordDataUI.CloseBonusResult();

            slotMainDataUI.gameObject.SetActive(false);
            probabilityDataUI.gameObject.SetActive(false);
            bonusDataUI.gameObject.SetActive(false);
            slumpGraphDataUI.gameObject.SetActive(false);
            bonusRecordDataUI.gameObject.SetActive(false);
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

            DisactivateAllScreen();
            OnClosedScreenEvent?.Invoke();
        }
    }
}

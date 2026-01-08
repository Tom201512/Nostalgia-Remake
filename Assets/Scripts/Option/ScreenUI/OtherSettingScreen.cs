using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.OtherSetting;
using ReelSpinGame_Save.Database.Option;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // その他設定画面
    public class OtherSettingScreen : MonoBehaviour, IOptionScreenBase
    {
        const int maxPage = 2;      // 最大ページ数

        // 各種操作
        [SerializeField] OtherSettingManager otherSettingManager;           // 設定変更マネージャー
        [SerializeField] OtherExitGameManager otherExitGameManager;         // 終了画面
        //[SerializeField] AutoStopPosLockManager autoStopPosLockManager;   // 設定位置固定設定マネージャー
        [SerializeField] ButtonComponent markerSettingButton;               // マーカー表示位置設定ボタン

        [SerializeField] private ButtonComponent nextButton;                // 次ボタン
        [SerializeField] private ButtonComponent previousButton;            // 前ボタン
        [SerializeField] private ButtonComponent closeButton;               // クローズボタン
        [SerializeField] private TextMeshProUGUI pageCount;                 // ページ表記

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen OnClosedScreenEvent;

        private int currentPage = 0;        // 表示中のページ番号
        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            closeButton.OnButtonPushedEvent += OnClosedPressed;
            nextButton.OnButtonPushedEvent += OnNextPushed;
            previousButton.OnButtonPushedEvent += OnPreviousPushed;
            otherSettingManager.OnSettingChangedEvent += OnSettingChanged;
            markerSettingButton.OnButtonPushedEvent += OnPosLockSettingButtonPressed;
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
            otherSettingManager.OnSettingChangedEvent -= OnSettingChanged;
            markerSettingButton.OnButtonPushedEvent -= OnPosLockSettingButtonPressed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            currentPage = 0;
            StartCoroutine(nameof(FadeInBehavior));
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                otherSettingManager.SetInteractiveButtons(false);
                closeButton.ToggleInteractive(false);
                nextButton.ToggleInteractive(false);
                previousButton.ToggleInteractive(false);
                markerSettingButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // データを得る
        public OtherOptionData GetSettingData() => otherSettingManager.CurrentOptionData;

        // 設定を読み込む
        public void LoadSettingData(OtherOptionData otherOption) => otherSettingManager.LoadOptionData(otherOption);

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

        // 設定変更時の挙動
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // オート位置設定移行ボタンを押したときの挙動
        void OnPosLockSettingButtonPressed(int signalID)
        {
            otherSettingManager.SetInteractiveButtons(false);
            closeButton.ToggleInteractive(false);
            nextButton.ToggleInteractive(false);
            previousButton.ToggleInteractive(false);
            markerSettingButton.ToggleInteractive(false);
            //autoStopPosLockManager.gameObject.SetActive(true);
            //autoStopPosLockManager.OpenScreen();
        }

        // オート位置設定が閉じられた時の挙動
        void OnPosLockSettingClosed()
        {
            otherSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
            //posLockSettingButton.ToggleInteractive(true);
            //autoStopPosLockManager.gameObject.SetActive(false);
        }

        // 画像の反映処理
        void UpdateScreen()
        {
            DisactivateAllScreen();
            // ページごとに処理を行う

            switch (currentPage)
            {
                case 0:
                    otherSettingManager.gameObject.SetActive(true);
                    break;

                case 1:
                    otherExitGameManager.gameObject.SetActive(true);
                    break;

                default:
                    break;
            }

            pageCount.text = (currentPage + 1) + "/" + maxPage;
        }

        // 全ての画面を非アクティブにする
        void DisactivateAllScreen()
        {
            otherSettingManager.gameObject.SetActive(false);
            otherExitGameManager.gameObject.SetActive(false);
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
            otherSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            nextButton.ToggleInteractive(true);
            previousButton.ToggleInteractive(true);
            //posLockSettingButton.ToggleInteractive(true);
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
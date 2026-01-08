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
        // 各種操作
        [SerializeField] OtherSettingManager otherSettingManager;           // 設定変更マネージャー
        //[SerializeField] AutoStopPosLockManager autoStopPosLockManager;   // 設定位置固定設定マネージャー
        [SerializeField] ButtonComponent markerSettingButton;               // マーカー表示位置設定ボタン
        [SerializeField] ButtonComponent resetButton;                       // リセットボタン

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

        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            //autoStopPosLockManager.gameObject.SetActive(false);

            closeButton.ButtonPushedEvent += OnClosedPressed;
            otherSettingManager.OnSettingChangedEvent += OnSettingChanged;
            resetButton.ButtonPushedEvent += OnResetButtonPressed;
            markerSettingButton.ButtonPushedEvent += OnPosLockSettingButtonPressed;
            //autoStopPosLockManager.ClosedScreenEvent += OnPosLockSettingClosed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            otherSettingManager.OnSettingChangedEvent -= OnSettingChanged;
            resetButton.ButtonPushedEvent -= OnResetButtonPressed;
            markerSettingButton.ButtonPushedEvent -= OnPosLockSettingButtonPressed;

            //autoStopPosLockManager.ClosedScreenEvent -= OnPosLockSettingClosed;
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
                otherSettingManager.SetInteractiveButtons(false);
                closeButton.ToggleInteractive(false);
                resetButton.ToggleInteractive(false);
                markerSettingButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // データを得る
        public OtherOptionData GetSettingData() => otherSettingManager.CurrentOptionData;

        // 設定を読み込む
        public void LoadSettingData(OtherOptionData otherOption) => otherSettingManager.LoadOptionData(otherOption);

        // 閉じるボタンを押したときの挙動
        void OnClosedPressed(int signalID) => CloseScreen();

        // 設定変更時の挙動
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // リセットボタンを押したときの挙動
        void OnResetButtonPressed(int signalID) => otherSettingManager.ResetOptionData();

        // オート位置設定移行ボタンを押したときの挙動
        void OnPosLockSettingButtonPressed(int signalID)
        {
            otherSettingManager.SetInteractiveButtons(false);
            closeButton.ToggleInteractive(false);
            resetButton.ToggleInteractive(false);
            markerSettingButton.ToggleInteractive(false);
            //autoStopPosLockManager.gameObject.SetActive(true);
            //autoStopPosLockManager.OpenScreen();
        }

        // オート位置設定が閉じられた時の挙動
        void OnPosLockSettingClosed()
        {
            otherSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
            //posLockSettingButton.ToggleInteractive(true);
            //autoStopPosLockManager.gameObject.SetActive(false);
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
            resetButton.ToggleInteractive(true);
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
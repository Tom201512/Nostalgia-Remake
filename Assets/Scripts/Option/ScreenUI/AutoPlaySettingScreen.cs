using ReelSpinGame_Option.AutoSetting;
using ReelSpinGame_Option.Button;
using ReelSpinGame_Save.Database.Option;
using System;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // オート設定画面
    public class AutoPlaySettingScreen : MonoBehaviour, IOptionScreenBase
    {
        // 各種操作
        [SerializeField] AutoSettingManager autoSettingManager;             // オート設定変更マネージャー
        [SerializeField] AutoStopPosLockManager autoStopPosLockManager;     // 設定位置固定設定マネージャー
        [SerializeField] ButtonComponent posLockSettingButton;              // オート位置設定ボタン
        [SerializeField] ButtonComponent closeButton;                       // クローズボタン
        [SerializeField] ButtonComponent resetButton;                       // リセットボタン

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
            autoStopPosLockManager.gameObject.SetActive(false);

            closeButton.ButtonPushedEvent += OnClosedPressed;
            autoSettingManager.OnSettingChangedEvent += OnSettingChanged;
            resetButton.ButtonPushedEvent += OnResetButtonPressed;
            posLockSettingButton.ButtonPushedEvent += OnPosLockSettingButtonPressed;
            autoStopPosLockManager.ClosedScreenEvent += OnPosLockSettingClosed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            autoSettingManager.OnSettingChangedEvent -= OnSettingChanged;
            resetButton.ButtonPushedEvent -= OnResetButtonPressed;
            posLockSettingButton.ButtonPushedEvent -= OnPosLockSettingButtonPressed;

            autoStopPosLockManager.ClosedScreenEvent -= OnPosLockSettingClosed;
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
                autoSettingManager.SetInteractiveButtons(false);
                closeButton.ToggleInteractive(false);
                resetButton.ToggleInteractive(false);
                posLockSettingButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // データを得る
        public AutoOptionData GetAutoSettingData() => autoSettingManager.CurrentAutoOptionData;

        // 設定を読み込む
        public void LoadSettingData(AutoOptionData autoOption) => autoSettingManager.LoadOptionData(autoOption);

        // 閉じるボタンを押したときの挙動
        void OnClosedPressed(int signalID) => CloseScreen();

        // 設定変更時の挙動
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // リセットボタンを押したときの挙動
        void OnResetButtonPressed(int signalID) => autoSettingManager.ResetOptionData();

        // オート位置設定移行ボタンを押したときの挙動
        void OnPosLockSettingButtonPressed(int signalID)
        {
            autoSettingManager.SetInteractiveButtons(false);
            closeButton.ToggleInteractive(false);
            resetButton.ToggleInteractive(false);
            posLockSettingButton.ToggleInteractive(false);
            autoStopPosLockManager.gameObject.SetActive(true);
            autoStopPosLockManager.OpenScreen();
        }

        // オート位置設定が閉じられた時の挙動
        void OnPosLockSettingClosed()
        {
            autoSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
            posLockSettingButton.ToggleInteractive(true);
            autoStopPosLockManager.gameObject.SetActive(false);
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
            autoSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
            posLockSettingButton.ToggleInteractive(true);
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



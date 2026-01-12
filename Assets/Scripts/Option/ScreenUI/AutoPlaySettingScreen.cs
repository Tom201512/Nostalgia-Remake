using ReelSpinGame_Option.AutoSetting;
using ReelSpinGame_Option.Components;
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
        [SerializeField] ReelPosSelectManager autoStopPosLockManager;       // 設定位置固定設定マネージャー
        [SerializeField] ButtonComponent posLockSettingButton;              // オート位置設定ボタン
        [SerializeField] ButtonComponent closeButton;                       // クローズボタン
        [SerializeField] ButtonComponent resetButton;                       // リセットボタン

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            closeButton.ButtonPushedEvent += OnClosedPressed;
            autoSettingManager.SettingChangedEvent += OnSettingChanged;
            resetButton.ButtonPushedEvent += OnResetButtonPressed;
            posLockSettingButton.ButtonPushedEvent += OnPosLockSettingButtonPressed;
            autoStopPosLockManager.ClosedScreenEvent += OnPosLockSettingClosed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            autoStopPosLockManager.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            autoSettingManager.SettingChangedEvent -= OnSettingChanged;
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
                SetInteractiveButtons(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // データを得る
        public AutoOptionData GetAutoSettingData() => autoSettingManager.CurrentAutoOptionData;

        // 設定を読み込む
        public void LoadSettingData(AutoOptionData autoOption) => autoSettingManager.LoadOptionData(autoOption);

        // 全てのボタンの操作をコントロールする
        void SetInteractiveButtons(bool value)
        {
            autoSettingManager.SetInteractiveButtons(value);
            closeButton.ToggleInteractive(value);
            resetButton.ToggleInteractive(value);
            posLockSettingButton.ToggleInteractive(value);
        }

        // 閉じるボタンを押したときの処理
        void OnClosedPressed(int signalID) => CloseScreen();

        // 設定変更時の処理
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // リセットボタンを押したときの処理
        void OnResetButtonPressed(int signalID) => autoSettingManager.ResetOptionData();

        // オート位置設定移行ボタンを押したときの処理
        void OnPosLockSettingButtonPressed(int signalID)
        {
            SetInteractiveButtons(false);
            autoStopPosLockManager.gameObject.SetActive(true);
            autoStopPosLockManager.OpenScreen();
        }

        // オート位置設定が閉じられた時の処理
        void OnPosLockSettingClosed()
        {
            SetInteractiveButtons(true);
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



using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.MenuContent;
using System;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Option.OtherSetting
{
    // 終了時の警告画面
    public class WarningScreen : MonoBehaviour
    {
        const float yesButtonEnableTime = 5f;           // 決定ボタンが有効になるまでの時間

        [SerializeField] ButtonComponent noButton;      // キャンセルボタン
        [SerializeField] ButtonComponent yesButton;     // 決定ボタン

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        // 決定ボタンが押された時のイベント
        public delegate void YesPressed();
        public event YesPressed YesPressedEvent;

        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            noButton.ButtonPushedEvent += OnNoPressed;
            yesButton.ButtonPushedEvent += OnYesPressed;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            noButton.ToggleInteractive(false);
            yesButton.ToggleInteractive(false);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            noButton.ButtonPushedEvent -= OnNoPressed;
            yesButton.ButtonPushedEvent -= OnYesPressed;
        }

        // 画面表示&初期化
        public void OpenScreen() => StartCoroutine(nameof(FadeInBehavior));

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                noButton.ToggleInteractive(false);
                yesButton.ToggleInteractive(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 次ボタンを押したときの処理
        void OnYesPressed(int signalID) => YesPressedEvent?.Invoke();

        // 閉じるボタンを押したときの処理
        void OnNoPressed(int signalID) => CloseScreen();

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
            noButton.ToggleInteractive(true);

            // 5秒経過で決定は押せるようにする
            yield return new WaitForSeconds(yesButtonEnableTime);
            yesButton.ToggleInteractive(true);
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

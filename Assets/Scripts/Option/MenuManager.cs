using ReelSpinGame_Option.Components;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Option.OtherSetting;
using System;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Option.MenuBar
{
    // メニューリストの管理マネージャー
    public class MenuManager : MonoBehaviour
    {
        // 各種ボタン
        [SerializeField] ButtonComponent howToPlayButton;       // 遊び方ガイド
        [SerializeField] ButtonComponent slotDataButton;        // スロット情報画面
        [SerializeField] ButtonComponent forceFlagButton;       // 強制フラグ
        [SerializeField] ButtonComponent autoSettingButton;     // オート設定
        [SerializeField] ButtonComponent otherSettingButton;    // その他設定 

        // 各種画面
        [SerializeField] HowToPlayScreen howToPlayScreen;               // 遊び方ガイドの画面
        [SerializeField] SlotDataScreen slotDataScreen;                 // スロット情報画面
        [SerializeField] ForceFlagScreen forceFlagScreen;               // 強制役設定画面
        [SerializeField] AutoPlaySettingScreen autoPlaySettingScreen;   // オート設定画面
        [SerializeField] OtherSettingScreen otherSettingScreen;         // その他設定画面

        // 何かしらのボタンを押したときのイベント
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // 何かしらの画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        private CanvasGroup canvasGroup;        // フェードイン、アウト用

        void Awake()
        {
            // ボタン登録
            howToPlayButton.OnButtonPushedEvent += HowToPlayOpen;
            slotDataButton.OnButtonPushedEvent += SlotDataOpen;
            forceFlagButton.OnButtonPushedEvent += ForceFlagOpen;
            autoSettingButton.OnButtonPushedEvent += AutoPlayOpen;
            otherSettingButton.OnButtonPushedEvent += OtherSettingOpen;

            howToPlayScreen.OnClosedScreenEvent += HowToPlayClose;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
            forceFlagScreen.OnClosedScreenEvent += ForceFlagClose;
            autoPlaySettingScreen.OnClosedScreenEvent += AutoPlayClose;
            otherSettingScreen.OnClosedScreenEvent += OtherSettingClose;
            CanInteract = false;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            // 画面を非表示にする
            howToPlayScreen.gameObject.SetActive(false);
            slotDataScreen.gameObject.SetActive(false);
            forceFlagScreen.gameObject.SetActive(false);
            autoPlaySettingScreen.gameObject.SetActive(false);
            otherSettingScreen.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            // ボタン登録解除
            howToPlayButton.OnButtonPushedEvent -= HowToPlayOpen;
            slotDataButton.OnButtonPushedEvent -= SlotDataOpen;
            forceFlagButton.OnButtonPushedEvent -= ForceFlagOpen;
            autoSettingButton.OnButtonPushedEvent -= AutoPlayOpen;
            otherSettingButton.OnButtonPushedEvent -= OtherSettingOpen;

            howToPlayScreen.OnClosedScreenEvent -= HowToPlayClose;
            slotDataScreen.OnClosedScreenEvent -= SlotDataClose;
            forceFlagScreen.OnClosedScreenEvent -= ForceFlagClose;
            autoPlaySettingScreen.OnClosedScreenEvent -= AutoPlayClose;
            otherSettingScreen.OnClosedScreenEvent -= OtherSettingClose;

            StopAllCoroutines();
        }

        // 全メニューのロック設定
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
            slotDataButton.ToggleInteractive(value);
            forceFlagButton.ToggleInteractive(value);
            autoSettingButton.ToggleInteractive(value);
            otherSettingButton.ToggleInteractive(value);
        }


        // メニューを開く
        public void OpenScreen()
        {
            StartCoroutine(nameof(FadeInBehavior));
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            if (CanInteract)
            {
                SetInteractiveAllButton(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // 遊び方ガイドを開いた時の処理
        void HowToPlayOpen(int signalID)
        {
            howToPlayScreen.gameObject.SetActive(true);
            howToPlayScreen.OpenScreen();
            OnPressedBehaviour();
        }

        // 遊び方ガイドを閉じた時の処理
        void HowToPlayClose()
        {
            howToPlayScreen.CloseScreen();
            howToPlayScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
        }

        // スロット情報を開いた時の処理
        void SlotDataOpen(int signalID)
        {
            slotDataScreen.gameObject.SetActive(true);
            slotDataScreen.OpenScreen();
            OnPressedBehaviour();
        }

        // スロット情報を閉じた時の処理
        void SlotDataClose()
        {
            slotDataScreen.CloseScreen();
            slotDataScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
        }

        // 強制役設定画面を開いた時の処理
        void ForceFlagOpen(int signalID)
        {
            forceFlagScreen.gameObject.SetActive(true);
            forceFlagScreen.OpenScreen();
            OnPressedBehaviour();
        }

        // 強制役設定画面を閉じた時の処理
        void ForceFlagClose()
        {
            forceFlagScreen.CloseScreen();
            forceFlagScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
        }

        // オートプレイ設定画面を開いた時の処理
        void AutoPlayOpen(int signalID)
        {
            autoPlaySettingScreen.gameObject.SetActive(true);
            autoPlaySettingScreen.OpenScreen();
            OnPressedBehaviour();
        }

        // オートプレイ設定画面を閉じた時の処理
        void AutoPlayClose()
        {
            autoPlaySettingScreen.CloseScreen();
            autoPlaySettingScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
        }

        // オートプレイ設定画面を開いた時の処理
        void OtherSettingOpen(int signalID)
        {
            otherSettingScreen.gameObject.SetActive(true);
            otherSettingScreen.OpenScreen();
            OnPressedBehaviour();
        }

        // オートプレイ設定画面を閉じた時の処理
        void OtherSettingClose()
        {
            otherSettingScreen.CloseScreen();
            otherSettingScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
        }

        // 画面を開いたときの処理
        void OnPressedBehaviour()
        {
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
        }

        // 画面を閉じたときの処理
        void OnClosedBehaviour()
        {
            SetInteractiveAllButton(true);
            OnClosedScreenEvent?.Invoke();
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
            SetInteractiveAllButton(true);
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

            CanInteract = false;
        }
    }
}

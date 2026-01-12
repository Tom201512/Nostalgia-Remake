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
        [SerializeField] ReelPosSelectManager reelMarkerSelectManager;      // リールマーカー設定マネージャー
        [SerializeField] ButtonComponent markerSettingButton;               // マーカー表示位置設定ボタン

        [SerializeField] private ButtonComponent nextButton;                // 次ボタン
        [SerializeField] private ButtonComponent previousButton;            // 前ボタン
        [SerializeField] private ButtonComponent closeButton;               // クローズボタン
        [SerializeField] private TextMeshProUGUI pageCount;                 // ページ表記

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        public bool DeletePlayerSave { get => otherExitGameManager.DeletePlayerSave; }  // プレイヤーセーブを消すか
        public bool DeleteOptionSave { get => otherExitGameManager.DeleteOptionSave; }  // 設定セーブを消すか

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        // 終了が実行された時の処理
        public delegate void GameExit();
        public event GameExit GameExitEvent;

        private int currentPage = 0;        // 表示中のページ番号
        private CanvasGroup canvasGroup;    // フェードイン、アウト用

        void Awake()
        {
            closeButton.ButtonPushedEvent += OnClosedPressed;
            nextButton.ButtonPushedEvent += OnNextPushed;
            previousButton.ButtonPushedEvent += OnPreviousPushed;
            otherSettingManager.SettingChangeEvent += OnSettingChanged;
            markerSettingButton.ButtonPushedEvent += OnReelMarkerSelectPushed;
            reelMarkerSelectManager.ClosedScreenEvent += OnReelMarkerSelectClosed;
            otherExitGameManager.WarningScreenOpenEvent += OnWarningScreenOpened;
            otherExitGameManager.WarningScreenCloseEvent += OnWarningScreenClosed;
            otherExitGameManager.GameExitEvent += OnExitGame;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            UpdateScreen();
            reelMarkerSelectManager.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            nextButton.ButtonPushedEvent -= OnNextPushed;
            previousButton.ButtonPushedEvent -= OnPreviousPushed;
            otherSettingManager.SettingChangeEvent -= OnSettingChanged;
            markerSettingButton.ButtonPushedEvent -= OnReelMarkerSelectPushed;
            reelMarkerSelectManager.ClosedScreenEvent -= OnReelMarkerSelectClosed;
            otherExitGameManager.WarningScreenOpenEvent -= OnWarningScreenOpened;
            otherExitGameManager.WarningScreenCloseEvent -= OnWarningScreenClosed;
            otherExitGameManager.GameExitEvent -= OnExitGame;
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
                SetInteractiveButtons(false);
                StartCoroutine(nameof(FadeOutBehavior));
            }
        }

        // データを得る
        public OtherOptionData GetSettingData() => otherSettingManager.CurrentOptionData;

        // 設定を読み込む
        public void LoadSettingData(OtherOptionData otherOption) => otherSettingManager.LoadOptionData(otherOption);

        // 全てのボタンの操作をコントロールする
        void SetInteractiveButtons(bool value)
        {
            otherSettingManager.SetInteractiveButtons(value);
            otherExitGameManager.SetInteractiveButtons(value);
            closeButton.ToggleInteractive(value);
            nextButton.ToggleInteractive(value);
            previousButton.ToggleInteractive(value);
            markerSettingButton.ToggleInteractive(value);
        }

        // 次ボタンを押したときの処理
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

        // 前ボタンを押したときの処理
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

        // 閉じるボタンを押したときの処理
        void OnClosedPressed(int signalID) => CloseScreen();

        // 設定変更時の処理
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // マーカー位置設定を押したときの処理
        void OnReelMarkerSelectPushed(int signalID)
        {
            SetInteractiveButtons(false);
            reelMarkerSelectManager.gameObject.SetActive(true);
            reelMarkerSelectManager.OpenScreen();
        }

        // マーカー位置設定が閉じられた時の処理
        void OnReelMarkerSelectClosed()
        {
            SetInteractiveButtons(true);
            reelMarkerSelectManager.gameObject.SetActive(false);
        }

        // 警告画面が表示された時の処理
        void OnWarningScreenOpened() => SetInteractiveButtons(false);

        // 警告画面が閉じられた時の処理
        void OnWarningScreenClosed() => SetInteractiveButtons(true);

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

        // ゲームが終了された時の処理
        void OnExitGame() => GameExitEvent?.Invoke();

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
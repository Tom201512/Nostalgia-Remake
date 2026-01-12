using ReelSpinGame_Lamps;
using ReelSpinGame_Option.MenuContent;
using UnityEngine;

namespace ReelSpinGame_System.Setting
{
    // 初期設定
    public class InitialSettingManager : MonoBehaviour
    {
        [SerializeField] LanguageSelectScreen languageSelectScreen;         // 設定変更画面
        [SerializeField] HowToPlayScreen howToPlayScreen;                   // 初回起動時の遊び方画面

        public bool IsSettingChanging { get; private set; }      // 設定変更中か

        // 言語設定が変更された時のイベント
        public delegate void LanguageChanged(int setting);
        public event LanguageChanged LanguageChangedEvent;

        // 遊び方画面が閉じられた時のイベント
        public delegate void HowToPlayClosed();
        public event HowToPlayClosed HowToPlayClosedEvent;

        void Awake()
        {
            IsSettingChanging = false;
            languageSelectScreen.ClosedScreenEvent += OnLanguageClosed;
        }

        void Start()
        {
            languageSelectScreen.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            languageSelectScreen.ClosedScreenEvent -= OnLanguageClosed;
            HowToPlayClosedEvent -= OnHowToPlayClosed;
        }

        // 設定変更画面を開く
        public void OpenInitializeSetting()
        {
            IsSettingChanging = true;
            languageSelectScreen.gameObject.SetActive(true);
            languageSelectScreen.OpenScreen();
        }

        // 言語設定画面が閉じたとき
        void OnLanguageClosed()
        {
            IsSettingChanging = false;
            languageSelectScreen.gameObject.SetActive(false);
            LanguageChangedEvent?.Invoke(languageSelectScreen.CurrentSetting);
            howToPlayScreen.ClosedScreenEvent += OnHowToPlayClosed;
            howToPlayScreen.gameObject.SetActive(true);
            howToPlayScreen.OpenScreen();
        }

        // 遊び方画面が閉じられた時
        void OnHowToPlayClosed()
        {
            howToPlayScreen.ClosedScreenEvent -= OnHowToPlayClosed;
            HowToPlayClosedEvent?.Invoke();
        }
    }
}

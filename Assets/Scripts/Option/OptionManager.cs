using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuBar;
using UnityEngine;

namespace ReelSpinGame_Option
{
    public class OptionManager : MonoBehaviour
    {
        // オプションマネージャー

        // const

        // var
        // メニュー開閉ボタン
        [SerializeField] private ButtonComponent openButton;
        // メニューバーのUI
        [SerializeField] private MenuManager menuBarUI;

        // オプション画面を開いているか(UIボタンの表示に使用する)
        public bool hasOptionScreen { get; private set; }
        // 設定変更中か(ゲームの操作ができなくなる)
        public bool hasOptionMode { get; private set; }
        // 設定が開けない状態か(リール回転中やオート実行中は設定を開けない)
        public bool lockOptionMode { get; private set; }

        private void Awake()
        {
            hasOptionScreen = false;
            hasOptionMode = false;
            lockOptionMode = false;
            openButton.ButtonPushedEvent += ToggleOptionScreen;
            menuBarUI.OnPressedMenuEvent += EnterOptionMode;
            menuBarUI.OnClosedScreenEvent += DisableOptionMode;
        }

        private void Start()
        {
            openButton.ToggleInteractive(true);
        }

        private void OnDestroy()
        {
            openButton.ButtonPushedEvent -= ToggleOptionScreen;
            menuBarUI.OnPressedMenuEvent -= EnterOptionMode;
            menuBarUI.OnClosedScreenEvent -= DisableOptionMode;
        }

        // func
        // オプション画面を開く
        public void ToggleOptionScreen()
        {
            Debug.Log("option clicked");
            menuBarUI.gameObject.SetActive(!menuBarUI.gameObject.activeSelf);
        }

        // ロック状態の設定
        public void ToggleOptionLock(bool value)
        {
            // 遊技中はボタンを押せないようにする(有効になるのは回転時)
            lockOptionMode = value;
            menuBarUI.SetInteractiveAllButton(!value);
            Debug.Log("Lock:" + value);
        }

        // オプションモードに入れる
        void EnterOptionMode()
        {
            hasOptionMode = true;
            openButton.ToggleInteractive(false);
        }

        // オプションモード解除
        void DisableOptionMode()
        {
            hasOptionMode = false;
            openButton.ToggleInteractive(true);
        }
    }
}

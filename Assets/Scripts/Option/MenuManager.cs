using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuContent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Option.MenuBar
{
    public class MenuManager : MonoBehaviour
    {
        // メニューリストの管理マネージャー

        // const

        // var

        // 各種ボタン
        // 遊び方のボタン
        [SerializeField] private ButtonComponent howToPlayButton;

        // 各種画面
        // 遊び方表示画面
        // テスト用
        [SerializeField] private HowToPlayScreen howToPlayScreen; 

        // 何かしらのボタンを押したときのイベント
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // 何かしらの画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // オプション

        void Awake()
        {
            howToPlayButton.ButtonPushedEvent += TestOpen;
            howToPlayScreen.OnClosedScreenEvent += TestClose;
        }

        void Start()
        {
            howToPlayButton.ToggleInteractive(true);
        }

        void OnDestroy()
        {
            howToPlayButton.ButtonPushedEvent -= TestOpen;
            howToPlayScreen.OnClosedScreenEvent -= TestClose;
        }

        // func
        // (テスト用)
        // 何かのボタンを開いた時の関数
        public void TestOpen()
        {
            howToPlayScreen.gameObject.SetActive(true);
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
            Debug.Log("Open Dialog");
        }

        // 画面を閉じたときのイベント
        public void TestClose()
        {
            howToPlayScreen.gameObject.SetActive(false);
            SetInteractiveAllButton(true);
            OnClosedScreenEvent?.Invoke();
            Debug.Log("Close");
        }

        // 全メニューのロック設定
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
        }
    }
}

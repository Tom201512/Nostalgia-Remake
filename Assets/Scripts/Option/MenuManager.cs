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
        // 遊び方ガイドを開くボタン
        [SerializeField] private ButtonComponent howToPlayButton;
        // スロット情報画面を開くボタン
        [SerializeField] private ButtonComponent slotDataButton;

        // 各種画面
        // 遊び方ガイドの画面
        [SerializeField] private HowToPlayScreen howToPlayScreen;
        // スロット情報画面
        [SerializeField] private SlotDataScreen slotDataScreen;

        // 何かしらのボタンを押したときのイベント
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // 何かしらの画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // オプション

        private void Awake()
        {
            howToPlayButton.ButtonPushedEvent += HowToPlayOpen;
            howToPlayScreen.OnClosedScreenEvent += HowToPlayClose;
            slotDataButton.ButtonPushedEvent += SlotDataOpen;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
        }

        private void Start()
        {
            howToPlayScreen.gameObject.SetActive(false);
            slotDataScreen.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            howToPlayButton.ButtonPushedEvent -= HowToPlayOpen;
            howToPlayScreen.OnClosedScreenEvent -= HowToPlayClose;
            slotDataButton.ButtonPushedEvent += SlotDataOpen;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
        }

        // func
        // 遊び方ガイドを開いた時の処理
        public void HowToPlayOpen()
        {
            howToPlayScreen.gameObject.SetActive(true);
            howToPlayScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open HowToPlay");
        }

        // 遊び方ガイドを閉じた時の処理
        public void HowToPlayClose()
        {
            howToPlayScreen.CloseScreen();
            howToPlayScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close HowToPlay");
        }

        // スロット情報を開いた時の処理
        public void SlotDataOpen()
        {
            slotDataScreen.gameObject.SetActive(true);
            slotDataScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open SlotData");
        }

        // スロット情報を閉じた時の処理
        public void SlotDataClose()
        {
            slotDataScreen.CloseScreen();
            slotDataScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close SlotData");
        }

        // 画面を開いたときの処理
        private void OnPressedBehaviour()
        {
            SetInteractiveAllButton(false);
            OnPressedMenuEvent?.Invoke();
        }

        // 画面を閉じたときの処理
        private void OnClosedBehaviour()
        {
            SetInteractiveAllButton(true);
            OnClosedScreenEvent?.Invoke();
        }

        // 全メニューのロック設定
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
            slotDataButton.ToggleInteractive(value);
        }
    }
}

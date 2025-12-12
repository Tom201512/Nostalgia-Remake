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
        [SerializeField] ButtonComponent howToPlayButton;       // 遊び方ガイド
        [SerializeField] ButtonComponent slotDataButton;        // スロット情報画面
        [SerializeField] ButtonComponent forceFlagButton;       // 強制フラグ
        [SerializeField] ButtonComponent autoSettingButton;     // オート設定

        // 各種画面
        [SerializeField] HowToPlayScreen howToPlayScreen;               // 遊び方ガイドの画面
        [SerializeField] SlotDataScreen slotDataScreen;                 // スロット情報画面
        [SerializeField] ForceFlagScreen forceFlagScreen;               // 強制役設定画面
        [SerializeField] AutoPlaySettingScreen autoPlaySettingScreen;   // オート設定画面

        // 何かしらのボタンを押したときのイベント
        public delegate void OnPressedMenu();
        public event OnPressedMenu OnPressedMenuEvent;

        // 何かしらの画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // オプションデータ

        private void Awake()
        {
            // ボタン登録
            howToPlayButton.ButtonPushedEvent += HowToPlayOpen;
            slotDataButton.ButtonPushedEvent += SlotDataOpen;
            forceFlagButton.ButtonPushedEvent += ForceFlagOpen;
            autoSettingButton.ButtonPushedEvent += AutoPlayOpen;

            howToPlayScreen.OnClosedScreenEvent += HowToPlayClose;
            slotDataScreen.OnClosedScreenEvent += SlotDataClose;
            forceFlagScreen.OnClosedScreenEvent += ForceFlagClose;
            autoPlaySettingScreen.OnClosedScreenEvent += AutoPlayClose;
        }

        private void Start()
        {
            // 画面を非表示にする
            howToPlayScreen.gameObject.SetActive(false);
            slotDataScreen.gameObject.SetActive(false);
            forceFlagScreen.gameObject.SetActive(false);
            autoPlaySettingScreen.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // ボタン登録解除
            howToPlayButton.ButtonPushedEvent -= HowToPlayOpen;
            slotDataButton.ButtonPushedEvent -= SlotDataOpen;
            forceFlagButton.ButtonPushedEvent -= ForceFlagOpen;
            autoSettingButton.ButtonPushedEvent -= AutoPlayOpen;

            howToPlayScreen.OnClosedScreenEvent -= HowToPlayClose;
            slotDataScreen.OnClosedScreenEvent -= SlotDataClose;
            forceFlagScreen.OnClosedScreenEvent -= ForceFlagClose;
            autoPlaySettingScreen.OnClosedScreenEvent -= AutoPlayClose;
        }

        // func(public)
        // 全メニューのロック設定
        public void SetInteractiveAllButton(bool value)
        {
            howToPlayButton.ToggleInteractive(value);
            slotDataButton.ToggleInteractive(value);
            forceFlagButton.ToggleInteractive(value);
            autoSettingButton.ToggleInteractive(value);
        }

        // func(private)
        // 遊び方ガイドを開いた時の処理
        void HowToPlayOpen(int signalID)
        {
            howToPlayScreen.gameObject.SetActive(true);
            howToPlayScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open HowToPlay");
        }

        // 遊び方ガイドを閉じた時の処理
        void HowToPlayClose()
        {
            howToPlayScreen.CloseScreen();
            howToPlayScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close HowToPlay");
        }

        // スロット情報を開いた時の処理
        void SlotDataOpen(int signalID)
        {
            slotDataScreen.gameObject.SetActive(true);
            slotDataScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open SlotData");
        }

        // スロット情報を閉じた時の処理
        void SlotDataClose()
        {
            slotDataScreen.CloseScreen();
            slotDataScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close SlotData");
        }

        // 強制役設定画面を開いた時の処理
        void ForceFlagOpen(int signalID)
        {
            forceFlagScreen.gameObject.SetActive(true);
            forceFlagScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open ForceFlag");
        }

        // 強制役設定画面を閉じた時の処理
        void ForceFlagClose()
        {
            forceFlagScreen.CloseScreen();
            forceFlagScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close ForceFlag");
        }

        // オートプレイ設定画面を開いた時の処理
        void AutoPlayOpen(int signalID)
        {
            autoPlaySettingScreen.gameObject.SetActive(true);
            autoPlaySettingScreen.OpenScreen();
            OnPressedBehaviour();
            Debug.Log("Open ForceFlag");
        }

        // オートプレイ設定画面を閉じた時の処理
        void AutoPlayClose()
        {
            autoPlaySettingScreen.CloseScreen();
            autoPlaySettingScreen.gameObject.SetActive(false);
            OnClosedBehaviour();
            Debug.Log("Close ForceFlag");
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
    }
}

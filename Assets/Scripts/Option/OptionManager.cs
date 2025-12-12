using ReelSpinGame_Option.Button;
using ReelSpinGame_Option.MenuBar;
using ReelSpinGame_Option.MenuContent;
using UnityEngine;

using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Option
{
    public class OptionManager : MonoBehaviour
    {
        // オプションマネージャー

        // const

        // var
        [SerializeField] private ButtonComponent openButton; // メニュー開閉ボタン
        [SerializeField] private MenuManager menuBarUI; // メニューバーのUI

        // 各種設定画面のデータ
        [SerializeField] ForceFlagScreen forceFlagScreen; // 強制フラグ
        [SerializeField] AutoPlaySettingScreen autoPlaySettingScreen; // オートプレイ

        public bool hasOptionScreen { get; private set; } // オプション画面を開いているか(UIボタンの表示に使用する)
        public bool hasOptionMode { get; private set; } // 設定変更中か(ゲームの操作ができなくなる)
        public bool lockOptionMode { get; private set; } // 設定が開けない状態か(リール回転中やオート実行中は設定を開けない)

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
            menuBarUI.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            openButton.ButtonPushedEvent -= ToggleOptionScreen;
            menuBarUI.OnPressedMenuEvent -= EnterOptionMode;
            menuBarUI.OnClosedScreenEvent -= DisableOptionMode;
        }

        // func
        // オプション画面を開く
        public void ToggleOptionScreen(int signalID)
        {
            if(openButton.CanInteractable)
            {
                Debug.Log("option clicked");
                menuBarUI.gameObject.SetActive(!menuBarUI.gameObject.activeSelf);
                menuBarUI.SetInteractiveAllButton(!lockOptionMode);
            }
        }

        // ロック状態の設定
        public void ToggleOptionLock(bool value)
        {
            // 遊技中はメニューバー操作ができないようにする
            lockOptionMode = value;
            menuBarUI.SetInteractiveAllButton(!value);
            Debug.Log("Lock:" + value);
        }

        // 各画面の設定情報取得
        public int GetForceFlagSelectID() => forceFlagScreen.CurrentSelectFlagID; // 選択したフラグ値
        public int GetForceFlagRandomID() => forceFlagScreen.CurrentSelectRandomID; // 選択したランダム値

        // 各画面の設定情報変更

        // 強制フラグのボタン有効化設定を変更
        public void SetForceFlagSetting(BonusStatus currentBonusStatus, BonusTypeID holdingBonusID)
        {
            forceFlagScreen.SetBonusStatus(currentBonusStatus, holdingBonusID);
        }

        // 強制フラグの設定リセット
        public void ResetForceFlagSetting() => forceFlagScreen.ResetFlagSetting();

        // オート設定変更

        public void SetAutoSetting()
        {

        }

        // オプションモードに入れる
        void EnterOptionMode()
        {
            hasOptionMode = true;
            openButton.ToggleInteractive(false);
            menuBarUI.SetInteractiveAllButton(!lockOptionMode);
        }

        // オプションモード解除
        void DisableOptionMode()
        {
            hasOptionMode = false;
            openButton.ToggleInteractive(true);
        }
    }
}

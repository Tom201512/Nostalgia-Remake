using ReelSpinGame_Option.AutoSetting;
using ReelSpinGame_Option.Button;
using ReelSpinGame_Save.Database.Option;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // オート設定画面
    public class AutoPlaySettingScreen : MonoBehaviour, IOptionScreenBase
    {
        // 各種操作
        [SerializeField] AutoSettingManager autoSettingManager; // オート設定変更マネージャー
        [SerializeField] ButtonComponent closeButton;           // クローズボタン
        [SerializeField] ButtonComponent resetButton;           // リセットボタン

        public bool CanInteract { get; set; }        // 操作ができる状態か(アニメーション中などはつけないこと)

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        // 画面を閉じたときのイベント
        public delegate void ClosedScreen();
        public event ClosedScreen ClosedScreenEvent;

        void Awake()
        {
            // イベント登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
            autoSettingManager.OnSettingChangedEvent += OnSettingChanged;
            resetButton.ButtonPushedEvent += OnResetButtonPressed;
        }

        void OnDestroy()
        {
            // イベント解除
            closeButton.ButtonPushedEvent -= OnClosedPressed;
            autoSettingManager.OnSettingChangedEvent -= OnSettingChanged;
            resetButton.ButtonPushedEvent -= OnResetButtonPressed;
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            Debug.Log("Initialized AutoSetting");
            CanInteract = true;
            Debug.Log("Interact :" + CanInteract);
            // ボタン有効化
            autoSettingManager.SetInteractiveButtons(true);
            closeButton.ToggleInteractive(true);
            resetButton.ToggleInteractive(true);
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if (CanInteract)
            {
                Debug.Log("Closed AutoSetting");
                autoSettingManager.SetInteractiveButtons(false);
                closeButton.ToggleInteractive(false);
                resetButton.ToggleInteractive(false);
            }
        }

        // データを得る
        public AutoOptionData GetAutoSettingData() => autoSettingManager.CurrentAutoOptionData;

        // 設定を読み込む
        public void LoadSettingData(AutoOptionData autoOption) => autoSettingManager.LoadOptionData(autoOption);

        // 閉じるボタンを押したときの挙動
        void OnClosedPressed(int signalID) => ClosedScreenEvent?.Invoke();

        // 設定変更時の挙動
        void OnSettingChanged() => SettingChangedEvent?.Invoke();

        // リセットボタンを押したときの挙動
        void OnResetButtonPressed(int signalID) => autoSettingManager.ResetOptionData();
    }
}



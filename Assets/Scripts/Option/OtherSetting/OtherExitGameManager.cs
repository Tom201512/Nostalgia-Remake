using ReelSpinGame_Option.Components;
using UnityEngine;

namespace ReelSpinGame_Option.OtherSetting
{
    // その他設定マネージャー
    public class OtherExitGameManager : MonoBehaviour
    {
        // 言語設定
        [SerializeField] ButtonComponent exitGameButton;            // ゲーム終了ボタン
        [SerializeField] ButtonComponent deletePlayerButton;        // プレイヤーセーブ削除ボタン
        [SerializeField] ButtonComponent deleteAllButton;           // 全セーブ削除ボタン

        // 警告画面
        [SerializeField] WarningScreen deletePlayerWarning;         // プレイヤーセーブを削除するかの警告画面
        [SerializeField] WarningScreen deleteAllWarning;            // 全セーブを削除するかの警告画面

        public bool DeletePlayerSave { get; private set; }          // プレイヤーセーブを消すか
        public bool DeleteOptionSave { get; private set; }          // オプションセーブを消すか

        // 警告画面を開いた時のイベント
        public delegate void WarningScreenOpen();
        public event WarningScreenOpen WarningScreenOpenEvent;

        // ゲームが終了したときのイベント
        public delegate void WarningScreenClosee();
        public event WarningScreenClosee WarningScreenCloseEvent;

        // ゲームが終了したときのイベント
        public delegate void GameExit();
        public event GameExit GameExitEvent;

        void Awake()
        {
            DeletePlayerSave = false;
            DeleteOptionSave = false;

            exitGameButton.ButtonPushedEvent += OnExitButtonPressed;
            deletePlayerButton.ButtonPushedEvent += OnDeletePlayerSavePressed;
            deleteAllButton.ButtonPushedEvent += OnDeleteAllSavePressed;

            deletePlayerWarning.ClosedScreenEvent += OnPlayerSaveDeleteWarningClosed;
            deleteAllWarning.ClosedScreenEvent += OnAllSaveDeleteWarningClosed;

            deletePlayerWarning.YesPressedEvent += OnPlayerSaveDeleted;
            deleteAllWarning.YesPressedEvent += OnAllSaveDeleted;
        }

        void Start()
        {
            deletePlayerWarning.gameObject.SetActive(false);
            deleteAllWarning.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            exitGameButton.ButtonPushedEvent -= OnExitButtonPressed;
            deletePlayerButton.ButtonPushedEvent -= OnDeletePlayerSavePressed;
            deleteAllButton.ButtonPushedEvent -= OnDeleteAllSavePressed;

            deletePlayerWarning.ClosedScreenEvent -= OnPlayerSaveDeleteWarningClosed;
            deleteAllWarning.ClosedScreenEvent -= OnAllSaveDeleteWarningClosed;

            deletePlayerWarning.YesPressedEvent -= OnPlayerSaveDeleted;
            deleteAllWarning.YesPressedEvent -= OnAllSaveDeleted;
        }

        // 各種選択ボタンの有効化設定
        public void SetInteractiveButtons(bool value)
        {
            exitGameButton.ToggleInteractive(value);
            deletePlayerButton.ToggleInteractive(value);
            deleteAllButton.ToggleInteractive(value);
        }

        // プレイヤーセーブ削除の警告画面が閉じられた時の処理
        void OnPlayerSaveDeleteWarningClosed()
        {
            SetInteractiveButtons(true);
            WarningScreenCloseEvent?.Invoke();
            deletePlayerWarning.gameObject.SetActive(false);
        }

        // 全セーブ削除の警告画面が閉じられた時の処理
        void OnAllSaveDeleteWarningClosed()
        {
            SetInteractiveButtons(true);
            WarningScreenCloseEvent?.Invoke();
            deleteAllWarning.gameObject.SetActive(false);
        }

        // プレイヤーセーブを削除するボタンが押された時の処理
        void OnDeletePlayerSavePressed(int signalID)
        {
            SetInteractiveButtons(false);
            WarningScreenOpenEvent?.Invoke();
            deletePlayerWarning.gameObject.SetActive(true);
            deletePlayerWarning.OpenScreen();
        }

        // 全セーブを削除するボタンが押された時の処理
        void OnDeleteAllSavePressed(int signalID)
        {
            SetInteractiveButtons(false);
            WarningScreenOpenEvent?.Invoke();
            deleteAllWarning.gameObject.SetActive(true);
            deleteAllWarning.OpenScreen();
        }

        // ゲームが終了された時の処理
        void OnExitButtonPressed(int signalID) => GameExitEvent?.Invoke();

        // プレイヤーセーブが消去された時の処理
        void OnPlayerSaveDeleted()
        {
            DeletePlayerSave = true;
            GameExitEvent?.Invoke();
        }

        // 全てのセーブが消去された時の処理
        void OnAllSaveDeleted()
        {
            DeletePlayerSave = true;
            DeleteOptionSave = true;
            GameExitEvent?.Invoke();
        }
    }
}

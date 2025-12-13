using ReelSpinGame_Option.AutoSetting;
using ReelSpinGame_Option.Button;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Option.MenuContent
{
    // オート設定画面
    public class AutoPlaySettingScreen : MonoBehaviour, IOptionScreenBase
    {
        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

        // var
        // 各種操作
        [SerializeField] AutoSettingManager autoSettingManager; // オート設定変更マネージャー
        [SerializeField] ButtonComponent closeButton; // クローズボタン

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func
        private void Awake()
        {
            // ボタン登録
            closeButton.ButtonPushedEvent += OnClosedPressed;
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            // 登録解除
            closeButton.ButtonPushedEvent -= OnClosedPressed;
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
            }
        }

        // 閉じるボタンを押したときの挙動
        void OnClosedPressed(int signalID) => OnClosedScreenEvent?.Invoke();
    }
}



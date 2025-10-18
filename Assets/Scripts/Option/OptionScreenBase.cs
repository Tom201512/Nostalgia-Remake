using ReelSpinGame_Option.Button;
using System;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    [Serializable]
    public abstract class OptionScreenBase : MonoBehaviour
    {
        // var
        // 操作ができる状態か(アニメーション中などはつけないこと)
        public bool CanInteract { get; set; }

        // クローズボタン
        [SerializeField] private ButtonComponent closeButton;

        // 画面を閉じたときのイベント
        public delegate void OnClosedScreen();
        public event OnClosedScreen OnClosedScreenEvent;

        // func

        private void Awake()
        {
            CanInteract = true;
            closeButton.ButtonPushedEvent += CloseScreen;
        }

        void Start()
        {
            closeButton.ToggleInteractive(true);
        }

        // 操作可能状態のコントロール
        public void SetCanInteract(bool value) => CanInteract = value;

        // 画面表示&初期化
        public void OpenScreen()
        {
            InitializeScreen();
            Debug.Log(name + " Opened");
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            Debug.Log("Interact :" + CanInteract);
            if(CanInteract)
            {
                CloseScreenBehavior();
                OnClosedScreenEvent?.Invoke();
                Debug.Log(name + " Closed");
            }
        }

        // 初期化用
        protected abstract void InitializeScreen();

        // 終了用
        protected abstract void CloseScreenBehavior();
    }
}

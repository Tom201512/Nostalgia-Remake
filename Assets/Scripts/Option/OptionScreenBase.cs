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
        public bool CanInteract {  get; private set; }

        // クローズボタン
        [SerializeField] private ButtonComponent closeButton;

        // 画面が閉じたときのイベント
        public delegate void OnClosedWindow();
        public event OnClosedWindow OnClosedWindowEvent;

        // func

        private void Awake()
        {
            CanInteract = false;
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
            gameObject.SetActive(true);
            InitializeScreen();
            Debug.Log(name + " Opened");
        }

        // 画面を閉じる
        public void CloseScreen()
        {
            gameObject.SetActive(false);
            Debug.Log(name + " Closed");
            OnClosedWindowEvent.Invoke();
        }

        // 初期化用
        protected abstract void InitializeScreen();
    }
}

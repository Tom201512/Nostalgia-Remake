using ReelSpinGame_Option.Button;
using System;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    [Serializable]
    public abstract class OptionScreenBase : MonoBehaviour
    {
        // var
        // 操作ができない状態か
        public bool CanInteract {  get; private set; }

        // クローズボタン
        [SerializeField] private ButtonComponent closeButton;

        // func

        private void Awake()
        {
            CanInteract = false;
            closeButton.ButtonPushedEvent += CloseScreen;
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
            InitializeScreen();
            Debug.Log(name + " Closed");
        }

        // 初期化用
        protected abstract void InitializeScreen();
    }
}

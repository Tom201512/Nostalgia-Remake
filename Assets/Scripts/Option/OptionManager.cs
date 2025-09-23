using ReelSpinGame_Option.Button;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_Option
{
    public class OptionManager : MonoBehaviour
    {
        // オプションマネージャー

        // const

        // var

        // オプションメニュー開閉ボタン
        [SerializeField] private ButtonComponent openButton;
        // オプションメニューUI
        [SerializeField] private GameObject optionUIScreen;

        // オプション画面を開いているか(UIボタンの表示に使用する)
        public bool hasOptionScreen { get; private set; }
        // 設定変更中か(ゲームの操作ができなくなる)
        public bool hasOptionMode { get; private set; }
        // 設定が開けない状態か(リール回転中やオート実行中は設定を開けない)
        public bool lockOptionMode { get; private set; }

        private void Awake()
        {
            hasOptionScreen = false;
            hasOptionMode = false;
            lockOptionMode = false;
        }

        private void Start()
        {
            openButton.buttonPushedEvent += ToggleOptionScreen;
        }

        // func
        // オプション画面を開く
        public void ToggleOptionScreen()
        {
            if (!lockOptionMode)
            {
                hasOptionScreen = !hasOptionScreen;
                hasOptionMode = !hasOptionMode;
                optionUIScreen.SetActive(hasOptionScreen);

                Debug.Log("Option:" + hasOptionMode);
            }
            else
            {
                Debug.LogWarning("Can't activate option because option mode is locked");
            }
        }

        // オプションロック設定
        public void LockOptionButton(bool value)
        {
            lockOptionMode = value;
            openButton.ToggleInteractive(lockOptionMode);

            Debug.Log("Lock:" + lockOptionMode);
        }
    }
}

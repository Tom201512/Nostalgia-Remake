using UnityEngine;

namespace ReelSpinGame_Option.Components
{
    // チェックボックス管理マネージャー
    public class CheckBoxManager : MonoBehaviour
    {
        CheckBoxComponent[] checkBoxList;       // チェックボックスリスト
        public byte CurrentSelectFlag { get; private set; }         // 現在の選択フラグ

        // チェックボックスの更新が行われた時のイベント
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signalID">信号番号</param>
        public delegate void CheckBoxUpdated();
        public event CheckBoxUpdated CheckBoxUpdatedEvent;

        void Awake()
        {
            checkBoxList = GetComponentsInChildren<CheckBoxComponent>();
            // チェックボックスボタンのイベント登録
            foreach (CheckBoxComponent check in checkBoxList)
            {
                check.ButtonPushedEvent += OnCheckBoxUpdated;
            }
        }

        void OnDestroy()
        {
            // チェックボックスボタンのイベント解除
            foreach (CheckBoxComponent check in checkBoxList)
            {
                check.ButtonPushedEvent -= OnCheckBoxUpdated;
            }
        }

        // データの読み込み
        public void LoadOptionData(byte value)
        {
            CurrentSelectFlag = value;
            // 対応するシグナルのチェックボックスを有効化する
            foreach (CheckBoxComponent check in checkBoxList)
            {
                if ((check.SignalID & CurrentSelectFlag) == check.SignalID)
                {
                    check.ToggleSelecting(true);
                }
                else
                {
                    check.ToggleSelecting(false);
                }
            }
        }

        // チェックボックスの有効化設定を変更する
        public void ChangeCheckBoxInteractive(bool value)
        {
            foreach (CheckBoxComponent check in checkBoxList)
            {
                check.ToggleInteractive(value);
            }
        }

        // チェックボックス更新時のイベント
        void OnCheckBoxUpdated(int signalID)
        {
            CurrentSelectFlag ^= (byte)signalID;
            CheckBoxUpdatedEvent?.Invoke();
        }
    }
}



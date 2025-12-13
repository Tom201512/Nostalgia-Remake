using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Option.Button
{
    // チェックボックス管理マネージャー
    public class CheckBoxManager : MonoBehaviour
    {
        // const

        // var
        CheckBoxComponent[] checkBoxList; // チェックボックスリスト
        byte currentSelectFlag; // 現在の選択フラグ

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
            foreach(CheckBoxComponent check in checkBoxList)
            {
                check.ButtonPushedEvent += OnCheckBoxUpdated;
            }
        }

        void OnDestroy()
        {
            // チェックボックスボタンのイベント解除
            foreach(CheckBoxComponent check in checkBoxList)
            {
                check.ButtonPushedEvent -= OnCheckBoxUpdated;
            }
        }

        // func public
        // チェックボックスの有効化設定を変更する
        public void ChangeCheckBoxInteractive(bool value)
        {
            foreach (CheckBoxComponent check in checkBoxList)
            {
                check.ToggleInteractive(value);
            }
        }

        // func private

        // チェックボックス更新時のイベント
        void OnCheckBoxUpdated(int signalID)
        {
            currentSelectFlag ^= (byte)signalID;
            Debug.Log("CurrentSelectFlagData:" + currentSelectFlag);
            CheckBoxUpdatedEvent?.Invoke();
        }
    }
}



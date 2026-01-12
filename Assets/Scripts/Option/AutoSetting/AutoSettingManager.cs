using ReelSpinGame_AutoPlay;
using ReelSpinGame_Option.Components;
using ReelSpinGame_Reels;
using ReelSpinGame_Save.Database.Option;
using System;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Option.AutoSetting
{
    // オート設定マネージャー
    public class AutoSettingManager : MonoBehaviour
    {
        // 選択ボタン
        [SerializeField] SelectButtonComponent speedSelect;                 // スピード変更
        [SerializeField] SelectButtonComponent orderSelect;                 // 押し順変更
        [SerializeField] SelectButtonComponent bigColorSelect;              // BIG時図柄変更
        [SerializeField] SelectButtonComponent technicalSelect;             // 技術介入変更 
        [SerializeField] CheckBoxManager autoEndTimingCheckBoxes;           // オート終了条件チェックボックス
        [SerializeField] SelectButtonComponent spinConditionSelect;         // オート回転数変更
        [SerializeField] ReelPosSelectManager autoStopPosLockManager;     // オート停止位置指定

        public AutoOptionData CurrentAutoOptionData { get; private set; } // 現在のオート設定

        // 設定が変更された時のイベント
        public delegate void SettingChanged();
        public event SettingChanged SettingChangedEvent;

        void Awake()
        {
            CurrentAutoOptionData = new AutoOptionData();

            // 設定変更時の処理登録
            speedSelect.ContentChangedEvent += UpdateOptionData;
            orderSelect.ContentChangedEvent += UpdateOptionData;
            bigColorSelect.ContentChangedEvent += UpdateOptionData;
            technicalSelect.ContentChangedEvent += UpdateOptionData;
            autoEndTimingCheckBoxes.CheckBoxUpdatedEvent += UpdateOptionData;
            spinConditionSelect.ContentChangedEvent += UpdateOptionData;
            autoStopPosLockManager.SettingChangedEvent += UpdateOptionData;
        }

        void OnDestroy()
        {
            // 設定変更時の処理登録解除
            speedSelect.ContentChangedEvent -= UpdateOptionData;
            orderSelect.ContentChangedEvent -= UpdateOptionData;
            bigColorSelect.ContentChangedEvent -= UpdateOptionData;
            technicalSelect.ContentChangedEvent -= UpdateOptionData;
            autoEndTimingCheckBoxes.CheckBoxUpdatedEvent += UpdateOptionData;
            spinConditionSelect.ContentChangedEvent -= UpdateOptionData;
            autoStopPosLockManager.SettingChangedEvent -= UpdateOptionData;
        }

        // 各種選択ボタンの有効化設定
        public void SetInteractiveButtons(bool value)
        {
            speedSelect.SetInteractive(value);
            orderSelect.SetInteractive(value);
            bigColorSelect.SetInteractive(value);
            technicalSelect.SetInteractive(value);
            autoEndTimingCheckBoxes.ChangeCheckBoxInteractive(value);
            spinConditionSelect.SetInteractive(value);
        }

        // 設定を読み込む
        public void LoadOptionData(AutoOptionData autoOption)
        {
            speedSelect.LoadOptionData((int)autoOption.CurrentSpeed);
            orderSelect.LoadOptionData((int)autoOption.CurrentStopOrder);
            bigColorSelect.LoadOptionData((int)autoOption.BigLineUpSymbol);
            technicalSelect.LoadOptionData(autoOption.HasTechnicalPlay ? 1 : 0);
            autoEndTimingCheckBoxes.LoadOptionData(autoOption.EndConditionFlag);
            spinConditionSelect.LoadOptionData((int)autoOption.SpinConditionID);
            autoStopPosLockManager.LoadOptionData(autoOption.StopPosLockData);
            UpdateOptionData();
        }

        // 設定値リセット
        public void ResetOptionData()
        {
            CurrentAutoOptionData.InitializeData();
            speedSelect.LoadOptionData((int)CurrentAutoOptionData.CurrentSpeed);
            orderSelect.LoadOptionData((int)CurrentAutoOptionData.CurrentStopOrder);
            bigColorSelect.LoadOptionData((int)CurrentAutoOptionData.BigLineUpSymbol);
            technicalSelect.LoadOptionData(CurrentAutoOptionData.HasTechnicalPlay ? 1 : 0);
            autoEndTimingCheckBoxes.LoadOptionData(CurrentAutoOptionData.EndConditionFlag);
            spinConditionSelect.LoadOptionData((int)CurrentAutoOptionData.SpinConditionID);
            autoStopPosLockManager.LoadOptionData(CurrentAutoOptionData.StopPosLockData);
            UpdateOptionData();
        }

        // データ更新
        void UpdateOptionData()
        {
            CurrentAutoOptionData.CurrentSpeed = ((AutoSpeedName)Enum.ToObject(typeof(AutoSpeedName), speedSelect.CurrentSettingID));
            CurrentAutoOptionData.CurrentStopOrder = ((StopOrderOptionName)Enum.ToObject(typeof(StopOrderOptionName), orderSelect.CurrentSettingID));
            CurrentAutoOptionData.BigLineUpSymbol = (BigType)Enum.ToObject(typeof(BigType), bigColorSelect.CurrentSettingID);
            CurrentAutoOptionData.HasTechnicalPlay = technicalSelect.CurrentSettingID == 1 ? true : false;
            CurrentAutoOptionData.EndConditionFlag = autoEndTimingCheckBoxes.CurrentSelectFlag;
            CurrentAutoOptionData.SpinConditionID = (SpinTimeConditionName)Enum.ToObject(typeof(SpinTimeConditionName), spinConditionSelect.CurrentSettingID);
            CurrentAutoOptionData.StopPosLockData[(int)ReelID.ReelLeft] = autoStopPosLockManager.CurrentLeftSelect;
            CurrentAutoOptionData.StopPosLockData[(int)ReelID.ReelMiddle] = autoStopPosLockManager.CurrentMiddleSelect;
            CurrentAutoOptionData.StopPosLockData[(int)ReelID.ReelRight] = autoStopPosLockManager.CurrentRightSelect;
            SettingChangedEvent?.Invoke();
        }
    }
}

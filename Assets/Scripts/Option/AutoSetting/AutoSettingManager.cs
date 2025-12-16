using ReelSpinGame_Option.Button;
using ReelSpinGame_Save.Database.Option;
using System;
using UnityEngine;

using static ReelSpinGame_AutoPlay.AutoManager;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Option.AutoSetting
{
    // オート設定マネージャー
    public class AutoSettingManager : MonoBehaviour
    {
        // const

        // var
        // 選択ボタン
        [SerializeField] SelectButtonComponent speedSelect;         // スピード変更
        [SerializeField] SelectButtonComponent orderSelect;         // 押し順変更
        [SerializeField] SelectButtonComponent bigColorSelect;      // BIG時図柄変更
        [SerializeField] SelectButtonComponent technicalSelect;     // 技術介入変更 
        [SerializeField] CheckBoxManager autoEndTimingCheckBoxes;   // オート終了条件チェックボックス
        [SerializeField] SelectButtonComponent spinConditionSelect; // オート回転数変更

        public AutoOptionData CurrentAutoOptionData { get; private set; } // 現在のオート設定

        // 設定が変更された時のイベント
        public delegate void OnSettingChanged();
        public event OnSettingChanged OnSettingChangedEvent;
        

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
        }

        // func(public)
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
            speedSelect.LoadOptionData((int)autoOption.AutoSpeedID);
            orderSelect.LoadOptionData((int)autoOption.AutoStopOrdersID);
            bigColorSelect.LoadOptionData((int)autoOption.BigColorLineUpID);
            technicalSelect.LoadOptionData(autoOption.HasTechnicalPlay ? 1 : 0);
            autoEndTimingCheckBoxes.LoadOptionData(autoOption.SpecificConditionBinary);
            spinConditionSelect.LoadOptionData((int)autoOption.SpinConditionID);
            UpdateOptionData();
        }

        // 設定値リセット
        public void ResetOptionData()
        {
            CurrentAutoOptionData.InitializeData();
            speedSelect.LoadOptionData((int)CurrentAutoOptionData.AutoSpeedID);
            orderSelect.LoadOptionData((int)CurrentAutoOptionData.AutoStopOrdersID);
            bigColorSelect.LoadOptionData((int)CurrentAutoOptionData.BigColorLineUpID);
            technicalSelect.LoadOptionData(CurrentAutoOptionData.HasTechnicalPlay ? 1 : 0);
            autoEndTimingCheckBoxes.LoadOptionData(CurrentAutoOptionData.SpecificConditionBinary);
            spinConditionSelect.LoadOptionData((int)CurrentAutoOptionData.SpinConditionID);
            UpdateOptionData();
        }

        // func(private)
        // データ更新
        void UpdateOptionData()
        {
            CurrentAutoOptionData.SetAutoSpeed((AutoPlaySpeed)Enum.ToObject(typeof(AutoPlaySpeed), speedSelect.CurrentSettingID));
            CurrentAutoOptionData.SetAutoStopOrder((AutoStopOrderOptions)Enum.ToObject(typeof(AutoStopOrderOptions), orderSelect.CurrentSettingID));
            CurrentAutoOptionData.SetBigColor((BigColor)Enum.ToObject(typeof(BigColor), bigColorSelect.CurrentSettingID));
            CurrentAutoOptionData.SetTechnicalPlay(technicalSelect.CurrentSettingID == 1 ? true : false);
            CurrentAutoOptionData.SetSpecificCondition(autoEndTimingCheckBoxes.CurrentSelectFlag);
            CurrentAutoOptionData.SetSpinCondition((AutoSpinTimeConditionID)Enum.ToObject(typeof(AutoSpinTimeConditionID), spinConditionSelect.CurrentSettingID));
            OnSettingChangedEvent?.Invoke();
        }
    }
}

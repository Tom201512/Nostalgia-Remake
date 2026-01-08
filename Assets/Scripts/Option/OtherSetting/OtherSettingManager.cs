using ReelSpinGame_Option.Components;
using ReelSpinGame_Save.Database.Option;
using System;
using UnityEngine;

namespace ReelSpinGame_Option.OtherSetting
{
    // その他設定マネージャー
    public class OtherSettingManager : MonoBehaviour
    {
        // 音量調整
        [SerializeField] SliderComponent musicVolumeChanger;                // BGM変更
        [SerializeField] SliderComponent soundVolumeChanger;                // SE変更

        // 選択ボタン
        [SerializeField] SelectButtonComponent resolutionSelect;            // 解像度変更
        [SerializeField] SelectButtonComponent cameraSelect;                // カメラモード変更
        [SerializeField] SelectButtonComponent miniReelSelect;              // ミニリール設定変更
        [SerializeField] SelectButtonComponent waitCutSelect;               // ウェイトカット変更
        [SerializeField] SelectButtonComponent showDelaySelect;             // スベリコマ表示変更

        // 言語設定
        [SerializeField] ButtonComponent languageJPN;                       // 日本語に変更
        [SerializeField] ButtonComponent languageENG;                       // 英語に変更

        public OtherOptionData CurrentOptionData { get; private set; }   // 現在の設定

        // 設定が変更された時のイベント
        public delegate void OnSettingChanged();
        public event OnSettingChanged OnSettingChangedEvent;

        void Awake()
        {
            CurrentOptionData = new OtherOptionData();

            // 設定変更時の処理登録
            musicVolumeChanger.OnSliderValueChanged += UpdateOptionData;
            soundVolumeChanger.OnSliderValueChanged += UpdateOptionData;
            resolutionSelect.ContentChangedEvent += UpdateOptionData;
            cameraSelect.ContentChangedEvent += UpdateOptionData;
            miniReelSelect.ContentChangedEvent += UpdateOptionData;
            waitCutSelect.ContentChangedEvent += UpdateOptionData;
            showDelaySelect.ContentChangedEvent += UpdateOptionData;
            //languageJPN.ContentChangedEvent += UpdateOptionData;
            //languageENG.ContentChangedEvent += UpdateOptionData;
        }

        void OnDestroy()
        {
            // 設定変更時の処理登録解除
            musicVolumeChanger.OnSliderValueChanged -= UpdateOptionData;
            soundVolumeChanger.OnSliderValueChanged -= UpdateOptionData;
            resolutionSelect.ContentChangedEvent -= UpdateOptionData;
            cameraSelect.ContentChangedEvent -= UpdateOptionData;
            miniReelSelect.ContentChangedEvent -= UpdateOptionData;
            waitCutSelect.ContentChangedEvent -= UpdateOptionData;
            showDelaySelect.ContentChangedEvent -= UpdateOptionData;
            //languageJPN.ContentChangedEvent -= UpdateOptionData;
            //languageENG.ContentChangedEvent -= UpdateOptionData;
        }

        // 各種選択ボタンの有効化設定
        public void SetInteractiveButtons(bool value)
        {
            musicVolumeChanger.SetInteractive(value);
            soundVolumeChanger.SetInteractive(value);
            resolutionSelect.SetInteractive(value);
            cameraSelect.SetInteractive(value);
            miniReelSelect.SetInteractive(value);
            waitCutSelect.SetInteractive(value);
            showDelaySelect.SetInteractive(value);
            languageJPN.ToggleInteractive(value);
            languageENG.ToggleInteractive(value);
        }

        // 設定を読み込む
        public void LoadOptionData(OtherOptionData otherOption)
        {
            musicVolumeChanger.SetSliderBarValue(otherOption.MusicVolumeSetting);
            soundVolumeChanger.SetSliderBarValue(otherOption.SoundVolumeSetting);
            resolutionSelect.LoadOptionData((int)otherOption.ResolutionSetting);
            cameraSelect.LoadOptionData(otherOption.UseOrthographicCamera ? 1 : 0);
            miniReelSelect.LoadOptionData(otherOption.ShowMiniReelSetting ? 1 : 0);
            waitCutSelect.LoadOptionData(otherOption.HasWaitCut ? 1 : 0);
            showDelaySelect.LoadOptionData(otherOption.HasDelayDisplay ? 1 : 0);
            UpdateOptionData();
        }

        // データ更新
        void UpdateOptionData()
        {
            CurrentOptionData.MusicVolumeSetting = musicVolumeChanger.CurrentSliderValue;
            CurrentOptionData.SoundVolumeSetting = soundVolumeChanger.CurrentSliderValue;
            CurrentOptionData.ResolutionSetting = (ResolutionOptionID)Enum.ToObject(typeof(ResolutionOptionID), resolutionSelect.CurrentSettingID);
            CurrentOptionData.UseOrthographicCamera = cameraSelect.CurrentSettingID == 1 ? true : false;
            CurrentOptionData.ShowMiniReelSetting = miniReelSelect.CurrentSettingID == 1 ? true : false;
            CurrentOptionData.HasWaitCut = waitCutSelect.CurrentSettingID == 1 ? true : false;
            CurrentOptionData.HasDelayDisplay = showDelaySelect.CurrentSettingID == 1 ? true : false;
            OnSettingChangedEvent?.Invoke();
        }
    }
}

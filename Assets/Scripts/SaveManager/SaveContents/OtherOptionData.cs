using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;

namespace ReelSpinGame_Save.Database.Option
{
    // その他設定のデータ
    public class OtherOptionData
    {
        // const
        // var
        public int MusicVolumeSetting {get; private set;}           // BGM音量 (0~7)
        public int SoundVolumeSetting {get; private set;}           // SE音量 (0~7)
        public bool ShowMiniReelSetting {get; private set;}         // ミニリール表示
        public List<int> AssistMarkerPos {get; private set;}        // アシストマーカー位置
        public bool HasWaitCut {get; private set;}                  // ウェイトカット
        public bool HasDelayDisplay { get; private set;}            // スベリコマ表示設定

        public OtherOptionData() 
        {
            MusicVolumeSetting = 5;
            SoundVolumeSetting = 5;
            ShowMiniReelSetting = false;
            AssistMarkerPos = new List<int>()
            {
                -1,
                -1,
                -1,
            };
            HasWaitCut = false;
            HasDelayDisplay = false;
        }

        // func(public)
        // 各種数値設定

        public void SetMusicVolume(int volume) => MusicVolumeSetting = volume;
        public void SetSoundVolume(int volume) => SoundVolumeSetting = volume;
        public void SetMiniReel(bool isEnabled) => ShowMiniReelSetting = isEnabled;
        public void SetMarkerPos(int leftPos, int middlePos, int rightPos)
        {
            AssistMarkerPos.Clear();
            AssistMarkerPos.Add(leftPos);
            AssistMarkerPos.Add(middlePos);
            AssistMarkerPos.Add(rightPos);
        }
        public void SetHasWaitCut(bool isEnabled) => HasWaitCut = isEnabled;
        public void SetHasDelayDisplay(bool isEnabled) => HasWaitCut = isEnabled;

        // データ初期化
        public void InitializeData()
        {
            MusicVolumeSetting = 5;
            SoundVolumeSetting = 5;
            ShowMiniReelSetting = false;
            AssistMarkerPos.Clear();
            for (int i = 0; i < ReelManagerModel.ReelAmount; i++)
            {
                AssistMarkerPos.Add(-1);
            }
            HasWaitCut = false;
            HasDelayDisplay = false;
        }
    }
}
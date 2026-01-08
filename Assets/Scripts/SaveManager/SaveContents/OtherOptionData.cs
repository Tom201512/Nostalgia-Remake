using System.Collections.Generic;
using static ReelSpinGame_Reels.ReelLogicManager;

namespace ReelSpinGame_Save.Database.Option
{
    // 解像度のオプションID
    public enum ResolutionOptionID
    {
        W960H540 = 0,
        W1024H576 = 1,
        W1280H720 = 2,
        W1366H768 = 3,
        W1600H900 = 4,
        W1920H1080 = 5,
    }

    // 言語オプションのID
    public enum LanguageOptionID
    {
        Japanese = 0,
        English = 1,
    }

    // その他設定のデータ
    public class OtherOptionData
    {
        public int MusicVolumeSetting { get; set; }                 // BGM音量 (0~100)
        public int SoundVolumeSetting { get; set; }                 // SE音量 (0~100)
        public ResolutionOptionID ResolutionSetting { get; set; }   // 解像度設定
        public bool UseOrthographicCamera {  get; set; }               // 平面投影カメラの有無
        public bool ShowMiniReelSetting { get; set; }               // ミニリール表示
        public List<int> AssistMarkerPos { get; private set; }      // アシストマーカー位置
        public bool HasWaitCut { get; set; }                        // ウェイトカット
        public bool HasDelayDisplay { get; set; }                   // スベリコマ表示設定
        public LanguageOptionID CurrentLanguage { get; set; }      // 言語設定

        public OtherOptionData()
        {
            MusicVolumeSetting = 50;
            SoundVolumeSetting = 50;
            ResolutionSetting = ResolutionOptionID.W1280H720;
            UseOrthographicCamera = false;
            ShowMiniReelSetting = false;
            AssistMarkerPos = new List<int>()
            {
                -1,
                -1,
                -1,
            };
            HasWaitCut = false;
            HasDelayDisplay = false;
            CurrentLanguage = LanguageOptionID.Japanese;
        }

        // マーカー位置設定
        public void SetMarkerPos(List<int> markerPos)
        {
            AssistMarkerPos.Clear();
            foreach(int pos in markerPos)
            {
                AssistMarkerPos.Add(pos);
            }
        }

        // データ初期化
        public void InitializeData()
        {
            MusicVolumeSetting = 50;
            SoundVolumeSetting = 50;
            ResolutionSetting = ResolutionOptionID.W1280H720;
            UseOrthographicCamera = false;
            ShowMiniReelSetting = false;
            AssistMarkerPos.Clear();
            for (int i = 0; i < ReelAmount; i++)
            {
                AssistMarkerPos.Add(-1);
            }
            HasWaitCut = false;
            HasDelayDisplay = false;

            CurrentLanguage = LanguageOptionID.Japanese;
        }
    }
}
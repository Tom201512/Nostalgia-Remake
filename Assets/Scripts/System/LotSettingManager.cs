using ReelSpinGame_Lamps;
using UnityEngine;

namespace ReelSpinGame_System.Setting
{
    // 設定変更マネージャー
    public class LotSettingManager : MonoBehaviour
    {
        [SerializeField] SettingSelectScreen settingSelectScreen;       // 設定変更画面
        [SerializeField] private BonusSevenSegment bonusSegments;       // ボーナス状態のセグメント

        public bool IsSettingChanging { get; private set; }      // 設定変更中か

        // 台設定が変更された時のイベント
        public delegate void SlotSettingChanged(int setting);
        public event SlotSettingChanged SlotSettingChangedEvent;

        void Awake()
        {
            IsSettingChanging = false;
            settingSelectScreen.ClosedScreenEvent += OnScreenClosed;
        }

        void Start()
        {
            settingSelectScreen.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            settingSelectScreen.ClosedScreenEvent -= OnScreenClosed;
        }

        // 設定変更画面を開く
        public void OpenSettingSelect()
        {
            IsSettingChanging = true;
            settingSelectScreen.gameObject.SetActive(true);
            settingSelectScreen.OpenScreen();
            bonusSegments.StartDisplayError();
        }

        // 設定画面が閉じられた時の処理
        void OnScreenClosed()
        {
            IsSettingChanging = false;
            settingSelectScreen.gameObject.SetActive(false);
            bonusSegments.TurnOffAllSegments();
            SlotSettingChangedEvent?.Invoke(settingSelectScreen.CurrentSetting);
        }
    }
}
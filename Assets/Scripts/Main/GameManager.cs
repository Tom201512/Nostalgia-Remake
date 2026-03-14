using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Flag.Flag;
using ReelSpinGame_Lamps;
using ReelSpinGame_Medal;
using ReelSpinGame_Option;
using ReelSpinGame_Payout;
using ReelSpinGame_Reels;
using ReelSpinGame_Save.Database;
using ReelSpinGame_Save.Database.Option;
using ReelSpinGame_System;
using ReelSpinGame_System.Setting;
using ReelSpinGame_UI.Player;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

// RSE (ReelSpinEngine) Copyright Strum7(Tom.u.) 2026
// ゲームの管理
namespace ReelSpinGame_Main
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;                     // 入力処理
        [SerializeField] private LotSettingManager lotSettingManager;           // 設定変更
        [SerializeField] private MedalManager medalManager;                     // メダル管理
        [SerializeField] private FlagManager flagManager;                       // フラグ抽選
        [SerializeField] private ReelLogicManager reelManagerObj;               // リール情報
        [SerializeField] private PayoutManager payoutManager;                   // 払い出し情報
        [SerializeField] private BonusManager bonusManager;                     // ボーナス情報
        [SerializeField] private EffectPresenter effectManagerObj;              // 演出
        [SerializeField] private OptionManager optionManagerObj;                // オプション画面
        [SerializeField] private PlayerUI playerUI;                             // プレイヤーUI
        [SerializeField] private StatusPanel statusPanel;                       // ステータスパネル
        [SerializeField] private LimitReachedScreen limitReachedScreen;         // 打ち止め時画面

        public ScreenManager Screen { get; private set; }
        public InitialSettingManager InitialSetting { get; private set; }
        public LotSettingManager LotSetting { get => lotSettingManager; }
        public InputManager InputManager { get => inputManager; }
        public MedalManager MedalManager { get => medalManager; }
        public FlagManager FlagManager { get => flagManager; }
        public WaitManager Wait { get; private set; }
        public ReelLogicManager ReelManager { get => reelManagerObj; }
        public PayoutManager PayoutManager { get => payoutManager; }
        public BonusManager BonusManager { get => bonusManager; }
        public EffectPresenter Effect { get => effectManagerObj; }
        public OptionManager Option { get => optionManagerObj; }
        public AutoManager Auto { get; private set; }

        public PlayerDatabase Player;                                                   // プレイヤー情報
        public PlayerUI PlayerUI { get => playerUI; }                                   // プレイヤーUI
        public PlayerSaveDatabase PlayerSaveDatabase { get => saveManager.PlayerSaveData; }         // プレイヤーのセーブ
        public OptionSave OptionSave { get => saveManager.OptionSave; }                 // オプションのセーブ
        public StatusPanel Status { get; private set; }                                 // ステータスパネル

        public bool IsFirstLaunch { get; set; }                     // セーブをしないか
        public bool HasReachedLimitSpin { get; private set; }       // 打ち止めに達したか
        public MainGameFlow MainFlow { get; private set; }          // ゲームステート用

        private SaveManager saveManager;                                // セーブ機能

        void Awake()
        {
            Screen = GetComponent<ScreenManager>();                     // 画面
            InitialSetting = GetComponent<InitialSettingManager>();     // 初回起動
            Wait = GetComponent<WaitManager>();                         // ウェイト
            MainFlow = new MainGameFlow(this);                          // メインフロー作成
            Status = statusPanel;                                       // ステータスパネル
            Player = new PlayerDatabase();                              // プレイヤー情報
            Auto = GetComponent<AutoManager>();                         // オート機能
            saveManager = new SaveManager();                            // セーブ機能

            HasReachedLimitSpin = false;
            IsFirstLaunch = false;

            Option.AutoSettingChangedEvent += OnAutoSettingChanged;
            Option.OtherSettingChangedEvent += OnOtherSettingChanged;
            Option.GameExitEvent += OnGameExit;
        }

        void Start()
        {
            limitReachedScreen.gameObject.SetActive(false);

            bool loadFailed = false;            // 読み込みに失敗したか
            bool playerLoadFailed = false;      // プレイヤーファイルのみ読み込みが失敗したか

            // セーブがない場合は新規にデータ作成
            if (saveManager.GenerateSaveFolder())
            {
                loadFailed = true;
            }
            // 読み込みに失敗したら新規にデータを作成する
            else if (!saveManager.LoadOptionSave())
            {
                loadFailed = true;
            }
            else if (!saveManager.LoadPlayerSave())
            {
                playerLoadFailed = true;
            }

            // 読み込みに失敗した場合はセーブを初期化
            if (loadFailed)
            {
                OptionSave.InitializeSave();
                PlayerSaveDatabase.InitializeSave();
                saveManager.DeleteOptionSave();
                saveManager.DeletePlayerSave();
                IsFirstLaunch = true;
            }
            else if (playerLoadFailed)
            {
                PlayerSaveDatabase.InitializeSave();
                saveManager.DeletePlayerSave();
            }

            // オプション画面へ情報を送る
            Option.UpdateSlotData(PlayerSaveDatabase, Player);
            Option.LoadAutoSettingFromSave(OptionSave.AutoOptionData);
            Option.LoadOtherSettingFromSave(OptionSave.OtherOptionData);

            // イベント登録
            InputManager.ActionTriggeredEvent += OnActionTriggered;

            // ステート開始
            MainFlow.StateManager.StartState();
        }

        void Update()
        {
            MainFlow.UpdateState();
        }

        void OnApplicationQuit()
        {
            // イベント登録解除
            Option.GameExitEvent -= OnGameExit;
            Option.AutoSettingChangedEvent -= OnAutoSettingChanged;

            // セーブ開始(初回設定が完了している場合のみ)
            if (!IsFirstLaunch)
            {
                saveManager.GenerateSaveFolder();

                if (Option.DeleteOptionSave)
                {
                    saveManager.DeleteOptionSave();
                    saveManager.DeletePlayerSave();
                }
                else
                {
                    saveManager.GenerateOptionSave();
                    // プレイヤーセーブを消去する場合
                    if (Option.DeletePlayerSave)
                    {
                        saveManager.DeletePlayerSave();
                    }
                    else
                    {
                        saveManager.GeneratePlayerSave();
                    }
                }
            }
        }

        // 初回設定の言語を設定する
        public void SetFirstLaunchLanguage(LanguageOptionID language)
        {
            OptionSave.OtherOptionData.CurrentLanguage = language;
            Option.LoadOtherSettingFromSave(OptionSave.OtherOptionData);
            _ = ChangeLocale(Option.OtherOptionData.CurrentLanguage);
        }

        // 打ち止めにする
        public void SetLimitReached()
        {
            limitReachedScreen.gameObject.SetActive(true);
            limitReachedScreen.OpenScreen();
            HasReachedLimitSpin = true;
        }

        // 設定反映
        void ApplyOptionSetting()
        {
            Effect.ChangeMusicVolume(Option.OtherOptionData.MusicVolumeSetting);
            Effect.ChangeSoundVolume(Option.OtherOptionData.SoundVolumeSetting);
            Screen.SetScreenSize(Option.OtherOptionData.ResolutionSetting);
            Screen.SetCameraMode(Option.OtherOptionData.UseOrthographicCamera);
            ReelManager.SetMiniReelVisible(Option.OtherOptionData.ShowMiniReelSetting);
            Wait.SetWaitCutSetting(Option.OtherOptionData.HasWaitCut);
            ReelManager.SetReelDelayVisible(Option.OtherOptionData.HasDelayDisplay);
            ReelManager.SetReelMarkers(Option.OtherOptionData.AssistMarkerPos);
            _ = ChangeLocale(Option.OtherOptionData.CurrentLanguage);
        }

        // オート処理が実行された時の処理
        void ToggleAuto()
        {
            if (!IsFirstLaunch && !Option.HasOptionMode && !LotSetting.IsSettingChanging && !HasReachedLimitSpin)
            {
                // 設定を反映する
                Auto.CurrentSpeed = Option.AutoOptionData.CurrentSpeed;
                Auto.CurrentStopOrder = Option.AutoOptionData.CurrentStopOrder;
                Auto.BigLineUpSymbol = Option.AutoOptionData.BigLineUpSymbol;
                Auto.HasTechnicalPlay = Option.AutoOptionData.HasTechnicalPlay;
                Auto.EndConditionFlag = Option.AutoOptionData.EndConditionFlag;
                Auto.SpinTimeCondition = Option.AutoOptionData.SpinConditionID;

                Auto.ChangeAutoMode();

                if (Auto.HasAuto)
                {
                    Option.ToggleOptionLock(true);
                }
            }
        }

        // メニュー画面が開かれた時の処理

        // プレイヤーが押したキーごとに処理を変更する
        void OnActionTriggered(InputManager.ControlKeys controlKey)
        {
            switch (controlKey)
            {
                case InputManager.ControlKeys.ToggleAuto:
                    ToggleAuto();
                    break;
                case InputManager.ControlKeys.ToggleMenu:
                    Option.ToggleOptionScreen(-1);
                    break;
            }
        }

        // オート設定変更時の処理
        void OnAutoSettingChanged() => OptionSave.RecordAutoData(Option.AutoOptionData);

        // その他設定変更時の処理
        void OnOtherSettingChanged()
        {
            OptionSave.RecordOtherData(Option.OtherOptionData);
            ApplyOptionSetting();
        }

        // ゲーム終了時の処理
        void OnGameExit() => Application.Quit();

        // 言語変更
        async Task ChangeLocale(LanguageOptionID languageID)
        {
            switch (languageID)
            {
                case LanguageOptionID.Japanese:
                    if (LocalizationSettings.SelectedLocale.Identifier.Code != "ja")
                    {
                        LocalizationSettings.SelectedLocale = Locale.CreateLocale("ja");
                    }
                    break;

                case LanguageOptionID.English:
                    if (LocalizationSettings.SelectedLocale.Identifier.Code != "en")
                    {
                        LocalizationSettings.SelectedLocale = Locale.CreateLocale("en");
                    }
                    break;
            }

            await LocalizationSettings.InitializationOperation.Task;
        }
    }
}
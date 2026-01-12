using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lamps;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Option;
using ReelSpinGame_Option.MenuContent;
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

// ゲームの管理
public class GameManager : MonoBehaviour
{
    public const int MaxSlotSetting = 6;    // 最高設定値

    [SerializeField] private ReelLogicManager reelManagerObj;       // リール情報
    [SerializeField] private EffectPresenter effectManagerObj;      // 演出
    [SerializeField] private OptionManager optionManagerObj;        // オプション画面
    [SerializeField] PlayerUI playerUI;                             // プレイヤーUI
    [SerializeField] SlotDataScreen slotDataScreen;                 // スロット情報データ画面UI
    [SerializeField] StatusPanel statusPanel;                       // ステータスパネル
    [SerializeField] LimitReachedScreen limitReachedScreen;         // 打ち止め時画面

    public ScreenManager Screen { get; private set; }                           // 画面マネージャー
    public InitialSettingManager InitialSetting { get; private set; }           // 初回起動設定マネージャー
    public LotSettingManager LotSetting { get; private set; }                   // 設定変更
    public InputManager InputManager { get; private set; }                      // 入力マネージャー
    public MedalManager Medal { get; private set; }                             // メダルマネージャー
    public FlagLots Lots { get; private set; }                                  // フラグ抽選マネージャー
    public WaitManager Wait { get; private set; }                               // ウェイト管理マネージャー
    public ReelLogicManager Reel { get { return reelManagerObj; } }             // リールマネージャー
    public PayoutManager Payout { get; private set; }                           // 払い出しマネージャー
    public BonusManager Bonus { get; private set; }                             // ボーナス管理マネージャー
    public EffectPresenter Effect { get { return effectManagerObj; } }          // 演出管理マネージャー
    public OptionManager Option { get { return optionManagerObj; } }            // オプションマネージャー
    public AutoManager Auto { get; private set; }                               // オートプレイ機能

    public PlayerDatabase Player;                                                   // プレイヤー情報
    public PlayerUI PlayerUI { get { return playerUI; } }                           // プレイヤーUI
    public SaveDatabase PlayerSave { get { return saveManager.PlayerSaveData; } }   // プレイヤーのセーブ
    public OptionSave OptionSave { get { return saveManager.OptionSave; } }         // オプションのセーブ
    public StatusPanel Status { get; private set; }                                 // ステータスパネル

    public bool IsFirstLaunch { get; set; }                     // セーブをしないか
    public int Setting { get; private set; }                    // 台設定値
    public bool HasReachedLimitSpin {  get; private set; }      // 打ち止めに達したか
    public MainGameFlow MainFlow { get; private set; }          // ゲームステート用

    private SaveManager saveManager;                                // セーブ機能

    void Awake()
    {
        InputManager = GetComponent<InputManager>();                // 操作
        Screen = GetComponent<ScreenManager>();                     // 画面
        InitialSetting = GetComponent<InitialSettingManager>();     // 初回起動
        LotSetting = GetComponent<LotSettingManager>();             // 設定変更
        Medal = GetComponent<MedalManager>();                       // メダル管理
        Lots = GetComponent<FlagLots>();                            // フラグ管理
        Wait = GetComponent<WaitManager>();                         // ウェイト
        Bonus = GetComponent<BonusManager>();                       // ボーナス
        Payout = GetComponent<PayoutManager>();                     // 払い出し
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
            PlayerSave.InitializeSave();
            saveManager.DeleteOptionSave();
            saveManager.DeletePlayerSave();
            IsFirstLaunch = true;
        }
        else if (playerLoadFailed)
        {
            PlayerSave.InitializeSave();
            saveManager.DeletePlayerSave();
        }

        // オプション画面へ情報を送る
        slotDataScreen.SendData(Player);
        Option.LoadAutoSettingFromSave(OptionSave.AutoOptionData);
        Option.LoadOtherSettingFromSave(OptionSave.OtherOptionData);

        // ステート開始
        MainFlow.StateManager.StartState();
    }

    void OnDestroy()
    {
        Option.GameExitEvent -= OnGameExit;
    }

    void Update()
    {
        // オートプレイ機能ボタン
        if (InputManager.CheckOneKeyInput(InputManager.ControlKeys.ToggleAuto))
        {
            if (!IsFirstLaunch && !Option.HasOptionMode && 
                LotSetting.IsSettingChanging && !HasReachedLimitSpin)
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

        // オプション画面起動(メニューボタンを押しても作動)
        if (InputManager.CheckOneKeyInput(InputManager.ControlKeys.ToggleOption))
        {
            Option.ToggleOptionScreen(-1);
        }

        MainFlow.UpdateState();
    }

    void OnApplicationQuit()
    {
        // イベント登録解除
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

    // 設定値変更
    public void ChangeSetting(int setting)
    {
        // 0ならランダムを選ぶ
        if (setting == 0)
        {
            Setting = Random.Range(1, MaxSlotSetting + 1);
        }
        else
        {
            Setting = setting;
        }

        saveManager.PlayerSaveData.Setting = Setting;
    }

    // 打ち止めにする
    public void SetLimitReached()
    {
        limitReachedScreen.gameObject.SetActive(true);
        limitReachedScreen.OpenScreen();
        HasReachedLimitSpin = true;
    }

    // 設定反映
    void ApplySetting()
    {
        Effect.ChangeMusicVolume(Option.OtherOptionData.MusicVolumeSetting);
        Effect.ChangeSoundVolume(Option.OtherOptionData.SoundVolumeSetting);
        Screen.SetScreenSize(Option.OtherOptionData.ResolutionSetting);
        Screen.SetCameraMode(Option.OtherOptionData.UseOrthographicCamera);
        Reel.SetMiniReelVisible(Option.OtherOptionData.ShowMiniReelSetting);
        Wait.SetWaitCutSetting(Option.OtherOptionData.HasWaitCut);
        Reel.SetReelDelayVisible(Option.OtherOptionData.HasDelayDisplay);
        Reel.SetReelMarkers(Option.OtherOptionData.AssistMarkerPos);
        _ = ChangeLocale(Option.OtherOptionData.CurrentLanguage);
    }

    // オート設定変更時の処理
    void OnAutoSettingChanged() => OptionSave.RecordAutoData(Option.AutoOptionData);

    // その他設定変更時の処理
    void OnOtherSettingChanged()
    {
        OptionSave.RecordOtherData(Option.OtherOptionData);
        ApplySetting();
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
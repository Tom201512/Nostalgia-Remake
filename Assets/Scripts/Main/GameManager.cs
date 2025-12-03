using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Option;
using ReelSpinGame_Option.MenuContent;
using ReelSpinGame_Save.Database;
using ReelSpinGame_System;
using ReelSpinGame_UI.Player;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;
using static ReelSpinGame_Bonus.BonusSystemData;

// ゲームの管理
public class GameManager : MonoBehaviour
{
    // const
    public const int MaxSlotSetting = 6;    // 最高設定値

    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight }

    // var
    // メインカメラ
    [SerializeField] private SlotCamera slotCam;

    // 各種マネージャー
    // リール情報
    [SerializeField] private ReelManager reelManagerObj;
    // 演出
    [SerializeField] private EffectPresenter effectManagerObj;
    // オプション画面
    [SerializeField] private OptionManager optionManagerObj;
    // プレイヤーUI
    [SerializeField] PlayerUI playerUI;
    // スロット情報データ画面UI
    [SerializeField] SlotDataScreen slotDataScreen;

    // デバッグ用
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;
    [SerializeField] AutoTestUI autoUI;

    // デバッグ用設定値
    [Range(0, 6), SerializeField] private int debugSetting;

    // 各種マネージャー
    public InputManager InputManager { get; private set; }              // 入力マネージャー
    public MedalManager Medal { get; private set; }                     // メダルマネージャー
    public FlagLots Lots { get; private set; }                          // フラグ抽選マネージャー
    public WaitManager Wait { get; private set; }                       // ウェイト管理マネージャー
    public ReelManager Reel { get { return reelManagerObj; } }          // リールマネージャー
    public BonusManager Bonus { get; private set; }                     // ボーナス管理マネージャー
    public EffectPresenter Effect { get { return effectManagerObj; } }  // 演出管理マネージャー
    public OptionManager Option { get { return optionManagerObj; } }    // オプションマネージャー

    // プレイヤー関連
    public PlayerDatabase Player;                               // プレイヤー情報
    public PlayerUI PlayerUI { get { return playerUI; } }       // プレイヤーUI

    public AutoPlayFunction Auto { get; private set; }      // オートプレイ機能

    public SaveDatabase Save { get { return saveManager.CurrentSave; } }    // セーブデータベース





    // ステータスパネル
    [SerializeField] StatusPanel statusPanel;
    public StatusPanel Status { get; private set; }

    // <デバッグ用> デバッグUI表示用
    [SerializeField] private KeyCode keyToDebugToggle;
    // カメラの視点変更
    [SerializeField] private KeyCode keyCameraModeChange;
    // <デバッグ用> オートの押し順設定
    [SerializeField] private KeyCode keyToAutoOrderChange;
    // <デバッグ用> オートスピード変更
    [SerializeField] private KeyCode keyToAutoSpeedChange;
    // <デバッグ用> ウェイトカットの有無設定
    [SerializeField] private KeyCode keyToWaitCutToggle;

    // <デバッグ用>セーブをしない
    [SerializeField] private bool dontSaveFlag;
    // <デバッグ用> 開始時にセーブ消去
    [SerializeField] private bool deleteSaveFlag;

    // <デバッグ用> デバッグUI表示するか
    private bool hasDebugUI;

    // 設定値
    public int Setting { get; private set; }

    // ゲームステート用
    public MainGameFlow MainFlow { get; private set; }

    private SaveManager saveManager;    // セーブ機能

    private void Awake()
    {
        // 画面サイズ初期化
        Screen.SetResolution(1600, 900, false);

        // FPS固定
        Application.targetFrameRate = 60;

        // 操作
        InputManager = GetComponent<InputManager>();
        // メダル管理
        Medal = GetComponent<MedalManager>();
        // フラグ管理
        Lots = GetComponent<FlagLots>();
        // ウェイト
        Wait = GetComponent<WaitManager>();
        // ボーナス
        Bonus = GetComponent<BonusManager>();
        // メインフロー作成
        MainFlow = new MainGameFlow(this);
        // ステータスパネル
        Status = statusPanel;
        // プレイヤー情報
        Player = new PlayerDatabase();
        // オート機能
        Auto = new AutoPlayFunction();
        // セーブ機能
        saveManager = new SaveManager();

        // デバッグUIの表示
        hasDebugUI = false;
    }

    private void Start()
    {
        // 読み込みに失敗したか
        bool loadFailed = false;

        if(deleteSaveFlag)
        {
            Debug.LogWarning("セーブを削除しました。");
            saveManager.DeleteSaveFile();
        }

        if(!dontSaveFlag)
        {
            // セーブ読み込み。セーブがない場合は新規作成
            // セーブフォルダの作成

            // セーブがない場合は新規にデータ作成
            if(saveManager.GenerateSaveFolder())
            {
                loadFailed = true;
            }
            // 読み込みに失敗したら新規にデータを作成する
            else if (!saveManager.LoadSaveFileWithDecryption())
            {
                loadFailed = true;
            }
        }

        // 読み込みに失敗した場合はセーブを初期化
        if(loadFailed)
        {
            Debug.LogWarning("セーブの読み込みに失敗しました。新規ファイルでプレイします。");
            Save.InitializeSave();
            saveManager.DeleteSaveFile();
            Save.RecordSlotSetting(debugSetting);
        }

        // UI 設定
        waitUI.SetWaitManager(Wait);
        autoUI.SetAutoFunction(Auto);

        slotDataScreen.SendData(Player);

        // デバッグをすべて非表示
        ToggleDebugUI(false);

        // ステート開始
        MainFlow.stateManager.StartState();
    }

    private void Update()
    {
        // 画面サイズ調整

        // 終了処理
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // オートプレイ機能ボタン
        if (InputManager.CheckOneKeyInput(InputManager.ControlKeys.ToggleAuto))
        {
            if(!Option.hasOptionMode)
            {
                Auto.ChangeAutoMode(AutoEndConditionID.None, 0, true, false, false, BigColor.None);

                if(Auto.HasAuto)
                {
                    Option.ToggleOptionLock(true);
                    Debug.Log("Option lock enabled");
                }
                else if(Auto.AutoSpeedID == (int)AutoPlaySpeed.Normal)
                {
                    if(!Option.lockOptionMode)
                    {
                        Option.ToggleOptionLock(false);
                    }
                    Debug.Log("Option lock disabled");
                }
            }
            else
            {
                Debug.LogAssertion("Can't activate auto because you're in option mode");
            }
        }

        // オプション画面起動(メニューボタンを押しても作動)
        if (InputManager.CheckOneKeyInput(InputManager.ControlKeys.ToggleOption))
        {
            Option.ToggleOptionScreen();
        }

        // オート押し順変更
        if (Input.GetKeyDown(keyToAutoOrderChange))
        {
            Auto.ChangeAutoOrder();
        }

        // オートスピード変更
        if(Input.GetKeyDown(keyToAutoSpeedChange))
        {
            Auto.ChangeAutoSpeed();
        }

        // ウェイトカット
        if (Input.GetKeyDown(keyToWaitCutToggle))
        {
            Wait.SetWaitCutSetting(!Wait.HasWaitCut);
        }

        // カメラ表示方法変更
        if (Input.GetKeyDown(keyCameraModeChange))
        {
            slotCam.ChangeCameraMode();
        }

        // デバッグ表示
        if (Input.GetKeyDown(keyToDebugToggle))
        {
            DebugButtonBehavior();
        }

        MainFlow.UpdateState();
    }

    private void OnApplicationQuit()
    {
        // セーブ開始
        if(!dontSaveFlag)
        {
            saveManager.GenerateSaveFolder();
            saveManager.GenerateSaveFileWithEncrypt();
        }
    }

    // func
    // 設定値変更
    public void ChangeSetting(int setting)
    {
        // 例外処理
        if (setting < 0 && setting > 6) 
        { 
            throw new System.Exception("Invalid Setting, must be within 0~6");
        }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            Setting = Random.Range(1, MaxSlotSetting + 1);
        }
        else
        {
            Setting = setting;
        }
    }

    // デバッグをつける機能(デバッグ用)
    private void DebugButtonBehavior()
    {
        hasDebugUI = !hasDebugUI;
        ToggleDebugUI(hasDebugUI);
    }

    // デバッグUIの表示非表示(デバッグ用)
    private void ToggleDebugUI(bool value)
    {
        autoUI.ToggleUI(value);
        medalUI.ToggleUI(value);
        lotsUI.ToggleUI(value);
        waitUI.ToggleUI(value);
        reelUI.ToggleUI(value);
        bonusUI.ToggleUI(value);
    }
}

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
using ReelSpinGame_System;
using ReelSpinGame_UI.Player;
using System;
using UnityEngine;

// ゲームの管理
public class GameManager : MonoBehaviour
{
    public const int MaxSlotSetting = 6;    // 最高設定値

    // 各種操作のシリアライズ
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight }

    [SerializeField] private SlotCamera slotCam;    // メインカメラ

    // 各種マネージャー
    [SerializeField] private ReelLogicManager reelManagerObj;       // リール情報
    [SerializeField] private EffectPresenter effectManagerObj;      // 演出
    [SerializeField] private OptionManager optionManagerObj;        // オプション画面
    [SerializeField] PlayerUI playerUI;                             // プレイヤーUI
    [SerializeField] SlotDataScreen slotDataScreen;                 // スロット情報データ画面UI
    [SerializeField] StatusPanel statusPanel;                       // ステータスパネル

    [SerializeField] private bool dontSaveFlag;                 // <デバッグ用>セーブをしない
    [SerializeField] private bool deleteSaveFlag;               // <デバッグ用> 開始時にセーブ消去

    [Range(0, 6), SerializeField] private int debugSetting;         // デバッグ用設定値

    // 各種マネージャー
    public InputManager InputManager { get; private set; }              // 入力マネージャー
    public MedalManager Medal { get; private set; }                     // メダルマネージャー
    public FlagLots Lots { get; private set; }                          // フラグ抽選マネージャー
    public WaitManager Wait { get; private set; }                       // ウェイト管理マネージャー
    public ReelLogicManager Reel { get { return reelManagerObj; } }     // リールマネージャー
    public PayoutManager Payout { get; private set; }                   // 払い出しマネージャー
    public BonusManager Bonus { get; private set; }                     // ボーナス管理マネージャー
    public EffectPresenter Effect { get { return effectManagerObj; } }  // 演出管理マネージャー
    public OptionManager Option { get { return optionManagerObj; } }    // オプションマネージャー
    public AutoManager Auto { get; private set; }                       // オートプレイ機能

    // プレイヤー関連
    public PlayerDatabase Player;                                                   // プレイヤー情報
    public PlayerUI PlayerUI { get { return playerUI; } }                           // プレイヤーUI
    public SaveDatabase PlayerSave { get { return saveManager.PlayerSaveData; } }   // プレイヤーのセーブ
    public OptionSave OptionSave { get { return saveManager.OptionSave; } }         // オプションのセーブ
    public StatusPanel Status { get; private set; }                                 // ステータスパネル

    public int Setting { get; private set; }                // 台設定値
    public MainGameFlow MainFlow { get; private set; }      // ゲームステート用

    SaveManager saveManager;                        // セーブ機能

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);         // 画面サイズ初期化
        Application.targetFrameRate = 60;               // FPS固定

        InputManager = GetComponent<InputManager>();        // 操作
        Medal = GetComponent<MedalManager>();               // メダル管理
        Lots = GetComponent<FlagLots>();                    // フラグ管理
        Wait = GetComponent<WaitManager>();                 // ウェイト
        Bonus = GetComponent<BonusManager>();               // ボーナス
        Payout = GetComponent<PayoutManager>();             // 払い出し
        MainFlow = new MainGameFlow(this);                  // メインフロー作成
        Status = statusPanel;                               // ステータスパネル
        Player = new PlayerDatabase();                      // プレイヤー情報
        Auto = GetComponent<AutoManager>();                 // オート機能
        saveManager = new SaveManager();                    // セーブ機能

        Option.AutoSettingChangedEvent += OnAutoSettingChanged;        // イベント登録
    }

    void Start()
    {
        bool loadFailed = false;            // 読み込みに失敗したか
        bool playerLoadFailed = false;      // プレイヤーファイルのみ読み込みが失敗したか

        if (deleteSaveFlag)
        {
            Debug.LogWarning("セーブを削除しました。");
            saveManager.DeletePlayerSave();
        }

        if (!dontSaveFlag)
        {
            // セーブ読み込み。セーブがない場合は新規作成
            // セーブフォルダの作成

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
        }

        // 読み込みに失敗した場合はセーブを初期化
        if (loadFailed)
        {
            Debug.LogWarning("セーブファイルが破損しています。全てのファイルをリセットしました。");
            OptionSave.InitializeSave();
            PlayerSave.InitializeSave();
            saveManager.DeleteOptionSave();
            saveManager.DeletePlayerSave();
            PlayerSave.Setting = debugSetting;
        }
        else if (playerLoadFailed)
        {
            Debug.LogWarning("プレイヤーセーブの読み込みに失敗しました。新規にプレイします。");
            PlayerSave.InitializeSave();
            saveManager.DeletePlayerSave();
            PlayerSave.Setting = debugSetting;
        }

        // オプション画面へ情報を送る
        slotDataScreen.SendData(Player);
        Option.LoadAutoSettingFromSave(OptionSave.AutoOptionData);

        // ステート開始
        MainFlow.StateManager.StartState();
    }

    void Update()
    {
        // 画面サイズ調整

        // オートプレイ機能ボタン
        if (InputManager.CheckOneKeyInput(InputManager.ControlKeys.ToggleAuto))
        {
            if (!Option.hasOptionMode)
            {
                // 設定を反映する
                Auto.CurrentSpeed = Option.GetAutoOptionData().CurrentSpeed;
                Auto.CurrentStopOrder = Option.GetAutoOptionData().CurrentStopOrder;
                Auto.BigLineUpSymbol = Option.GetAutoOptionData().BigLineUpSymbol;
                Auto.HasTechnicalPlay = Option.GetAutoOptionData().HasTechnicalPlay;
                Auto.EndConditionFlag = Option.GetAutoOptionData().EndConditionFlag;
                Auto.SpinTimeCondition = Option.GetAutoOptionData().SpinConditionID;

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

        // セーブ開始
        if (!dontSaveFlag)
        {
            saveManager.GenerateSaveFolder();
            saveManager.GeneratePlayerSave();
            saveManager.GenerateOptionSave();
        }
    }

    // 設定値変更
    public void ChangeSetting(int setting)
    {
        // 例外処理
        if (setting < 0 && setting > 6)
        {
            throw new Exception("Invalid Setting, must be within 0~6");
        }
        // 0ならランダムを選ぶ
        else if (setting == 0)
        {
            Setting = UnityEngine.Random.Range(1, MaxSlotSetting + 1);
        }
        else
        {
            Setting = setting;
        }
    }

    // オート設定変更時の挙動
    void OnAutoSettingChanged() => OptionSave.RecordAutoData(Option.GetAutoOptionData());

    // その他設定変更時の挙動
    void OnOtherSettingChanged()
    {
        Debug.Log("Received OtherSetting Changed");

        // セーブに記録する
    }
}
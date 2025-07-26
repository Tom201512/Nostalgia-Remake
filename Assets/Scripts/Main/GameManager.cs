using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_System;
using ReelSpinGame_Save.Database;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �Q�[���̊Ǘ�
    // const
    // �ō��ݒ�l
    public const int MaxSlotSetting = 6;

    // �e�푀��̃V���A���C�Y
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight }

    // var
    // ���C���J����
    [SerializeField] private SlotCamera slotCam;

    // �e��@�\
    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] private EffectManager effectManagerObj;

    [SerializeField] PlayerUI playerUI;

    // �f�o�b�O�p
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;
    [SerializeField] AutoTestUI AutoUI;


    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get { return reelManagerObj; } }
    public BonusManager Bonus { get; private set; }
    public EffectManager Effect { get { return effectManagerObj; } }

    // �v���C���[���
    public PlayerDatabase Player;

    // �I�[�g�v���C�@�\
    public AutoPlayFunction Auto { get; private set; }

    // �Z�[�u�@�\
    private SaveManager saveManager;

    // �Z�[�u�f�[�^�x�[�X
    public SaveDatabase Save { get { return saveManager.CurrentSave; } }

    [SerializeField] StatusPanel statusPanel;
    public StatusPanel Status { get; private set; }

    // �L�[�ݒ�
    // MAXBET
    [SerializeField] private KeyCode maxBetKey;
    // 1BET
    [SerializeField] private KeyCode betOneKey;
    //2BET
    [SerializeField] private KeyCode betTwoKey;
    // ���[���n��(�܂���MAX BET)
    [SerializeField] private KeyCode startAndMaxBetKey;
    // ���[����~
    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    // �I�[�g�J�n/��~�{�^��
    [SerializeField] private KeyCode keyToAutoToggle;

    // <�f�o�b�O�p> �f�o�b�OUI�\���p
    [SerializeField] private KeyCode keyToDebugToggle;

    // �J�����̎��_�ύX
    [SerializeField] private KeyCode keyCameraModeChange;

    // <�f�o�b�O�p> �I�[�g�̉������ݒ�
    [SerializeField] private KeyCode keyToAutoOrderChange;

    // <�f�o�b�O�p> �E�F�C�g�J�b�g�̗L���ݒ�
    [SerializeField] private KeyCode keyToWaitCutToggle;

    // <�f�o�b�O�p>�Z�[�u���������邩
    [SerializeField] private bool deleteSave;

    // <�f�o�b�O�p> �f�o�b�OUI�\�����邩
    private bool hasDebugUI;

    // �ݒ�l
    public int Setting { get; private set; }
    // �f�o�b�O�p�ݒ�l
    [Range(0, 6), SerializeField] private int debugSetting;

    // ���͗p�L�[�R�[�h
    public KeyCode[] KeyCodes { get; private set; }

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        // ���
        //Debug.Log("Screen:" + Screen.width + "," + Screen.height);
        Screen.SetResolution(960, 540, false);

        // ���_���Ǘ�
        Medal = GetComponent<MedalManager>();
        // �t���O�Ǘ�
        Lots = GetComponent<FlagLots>();
        // �E�F�C�g�Ǘ�
        Wait = new WaitManager(false);
        // �{�[�i�X
        Bonus = GetComponent<BonusManager>();
        // ���C���t���[�쐬
        MainFlow = new MainGameFlow(this);
        // �X�e�[�^�X�p�l��
        Status = statusPanel;

        // �v���C���[���
        Player = new PlayerDatabase();

        // �I�[�g�@�\
        Auto = new AutoPlayFunction();
        // �Z�[�u�@�\
        saveManager = new SaveManager();

        // �Z�[�u�f�[�^�x�[�X

        // �L�[�{�[�h�̃R�[�h�ݒ�
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey, betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight };

        // ��ʃT�C�Y������
        Screen.SetResolution(1600, 900, false);

        // FPS�Œ�
        Application.targetFrameRate = 60;

        // �f�o�b�OUI�̕\��
        hasDebugUI = false;
    }

    void Start()
    {
        // �Z�[�u�ǂݍ��݁B�Z�[�u���Ȃ��ꍇ�͐V�K�쐬
        // �f�o�b�O�p(�Z�[�u������true�Ȃ�ǂݍ��܂��V�K�f�[�^�쐬)

        // �Z�[�u�t�H���_�̍쐬
        saveManager.GenerateSaveFolder();

        if (deleteSave)
        {
            // �Z�[�u�폜
            saveManager.DeleteSaveFile();

            // �ݒ�l�̍쐬
            Save.RecordSlotSetting(debugSetting);
            deleteSave = false;
        }

        else if (!saveManager.LoadSaveFile())
        {
            // �ݒ�l�̍쐬
            Save.RecordSlotSetting(debugSetting);

            //Debug.Log("Save is newly generated");
        }

        // UI �ݒ�
        waitUI.SetWaitManager(Wait);
        playerUI.SetPlayerData(Player);
        playerUI.SetMedalManager(Medal);
        AutoUI.SetAutoFunction(Auto);

        // �f�o�b�O�����ׂĔ�\��
        ToggleDebugUI(false);

        // �X�e�[�g�J�n
        MainFlow.stateManager.StartState();
    }

    void Update()
    {
        // ��ʃT�C�Y����

        // �I������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // �I�[�g�v���C�@�\�{�^��
        if (Input.GetKeyDown(keyToAutoToggle))
        {
            Auto.ChangeAutoMode();
        }

        // �I�[�g�������ύX
        if (Input.GetKeyDown(keyToAutoOrderChange))
        {
            Auto.ChangeAutoOrder();
        }

        // �E�F�C�g�J�b�g
        if (Input.GetKeyDown(keyToWaitCutToggle))
        {
            Wait.SetWaitCutSetting(!Wait.HasWaitCut);
        }

        // �J�����\�����@�ύX
        if (Input.GetKeyDown(keyCameraModeChange))
        {
            slotCam.ChangeCameraMode();
        }

        // �f�o�b�O�\��
        if (Input.GetKeyDown(keyToDebugToggle))
        {
            DebugButtonBehavior();
        }

        MainFlow.UpdateState();
    }

    private void OnApplicationQuit()
    {
        Wait.DisposeWait();
        // �Z�[�u�J�n
        saveManager.GenerateSaveFolder();
        // �t�@�C���Z�[�u���s��
        saveManager.GenerateSaveFile();
    }

    // func
    // �ݒ�l�ύX
    public void ChangeSetting(int setting)
    {
        // ��O����
        if (setting < 0 && setting > 6) 
        { 
            throw new System.Exception("Invalid Setting, must be within 0~6");
        }
        // 0�Ȃ烉���_����I��
        else if (setting == 0)
        {
            Setting = Random.Range(1, MaxSlotSetting + 1);
        }
        else
        {
            Setting = setting;
        }
    }

    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;

    // �Z�[�u�f�[�^�Q��
    public SaveDatabase GetSave() => saveManager.CurrentSave;

    // �f�o�b�O������@�\
    private void DebugButtonBehavior()
    {
        hasDebugUI = !hasDebugUI;
        ToggleDebugUI(hasDebugUI);
    }

    // �f�o�b�OUI�̕\����\��
    private void ToggleDebugUI(bool value)
    {
        medalUI.ToggleUI(value);
        lotsUI.ToggleUI(value);
        waitUI.ToggleUI(value);
        reelUI.ToggleUI(value);
        bonusUI.ToggleUI(value);
    }
}

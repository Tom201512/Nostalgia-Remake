using ReelSpinGame_AutoPlay;
using ReelSpinGame_Bonus;
using ReelSpinGame_Effect;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_System;
using UnityEngine;
using static ReelSpinGame_AutoPlay.AutoPlayFunction;

public class GameManager : MonoBehaviour
{
    // �Q�[���̊Ǘ�
    // const
    // �e�푀��̃V���A���C�Y
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var
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

    // �I�[�g�v���C�@�\
    public AutoPlayFunction Auto { get; private set; }

    // �Z�[�u�@�\
    private SaveManager save;

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

    // <�f�o�b�O�p> �f�o�b�OUI�\�����邩
    private bool hasDebugUI;

    // �ݒ�
    [Range(0,6), SerializeField] private int setting;
    // ���͗p�L�[�R�[�h
    public KeyCode[] KeyCodes { get; private set; }

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }
    public int Setting { get { return setting; } }

    // �v���C���[�f�[�^�p
    public PlayingDatabase PlayerData { get; private set; }

    void Awake()
    {
        // �v���C���[�f�[�^�쐬
        PlayerData = new PlayingDatabase();
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

        // �I�[�g�@�\
        Auto = new AutoPlayFunction(AutoPlaySpeed.Normal, AutoStopOrderOptions.LMR);
        // �Z�[�u�@�\
        save = new SaveManager();

        // �L�[�{�[�h�̃R�[�h�ݒ�
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // ��O����
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid Setting, must be within 0~6"); }
        // 0�Ȃ烉���_����I��
        else if (setting == 0)
        {
            setting = Random.Range(1, 6);
        }

        // ��ʃT�C�Y������
        Screen.SetResolution(1600, 900, false);

        // FPS�Œ�
        Application.targetFrameRate = 60;

        // �f�o�b�OUI�̕\��
        hasDebugUI = false;
    }

    void Start()
    {
        /*
        // �Z�[�u�ǂݍ��݁B�Z�[�u���Ȃ��ꍇ�͐V�K�쐬
        if(!save.LoadSaveFile())
        {
            // �Z�[�u�t�H���_�̍쐬
            save.GenerateSaveFolder();
        }*/

        // ���_���ݒ�
        Medal.SetMedalData(0, 3, 0, false);

        // UI �ݒ�
        waitUI.SetWaitManager(Wait);
        playerUI.SetPlayerData(PlayerData);
        playerUI.SetMedalManager(Medal);
        AutoUI.SetAutoFunction(Auto);

        // �X�e�[�g�J�n
        MainFlow.stateManager.StartState();

        // �f�o�b�O�����ׂĔ�\��
        ToggleDebugUI(false);
    }

    void Update()
    {
        // ��ʃT�C�Y����

        // �I������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Game Closed");
            Application.Quit();
        }

        // �I�[�g�v���C�@�\�{�^��
        if (Input.GetKeyDown(keyToAutoToggle))
        {
            Auto.ChangeAutoMode();
        }

        // �f�o�b�O�\��
        if(Input.GetKeyDown(keyToDebugToggle))
        {
            DebugButtonBehavior();
        }

        MainFlow.UpdateState();
    }

    // �I�����̏���
    private void OnDestroy()
    {
        Wait.DisposeWait();

        /*
        // �Z�[�u
        save.GenerateSaveFolder();
        // �e�X�g
        save.GenerateSaveFile(setting);
        */
    }

    // func
    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;

    // �f�o�b�O������@�\
    private void DebugButtonBehavior()
    {
        hasDebugUI = !hasDebugUI;
        Debug.Log("Debug:" + hasDebugUI);
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

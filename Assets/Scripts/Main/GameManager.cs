using ReelSpinGame_Bonus;
using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Sound;
using ReelSpinGame_System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �Q�[���̊Ǘ�
    // const
    // �e�푀��̃V���A���C�Y
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var
    // �e��@�\
    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] private SoundManager soundManagerObj;
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;
    [SerializeField] BonusTestUI bonusUI;
    [SerializeField] PlayerUI playerUI;

    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get { return reelManagerObj; } }
    public BonusManager Bonus { get; private set; }
    public SoundManager Sound { get { return soundManagerObj; } }

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
        ////Debug.Log("Medal is launched");

        // �t���O�Ǘ�
        Lots = GetComponent<FlagLots>();
        ////Debug.Log("Lots is launched");

        // �E�F�C�g�Ǘ�
        Wait = new WaitManager(false);
        ////Debug.Log("Wait is launched");

        // �{�[�i�X
        Bonus = GetComponent<BonusManager>();
        ////Debug.Log("Bonus is launched");

        // ���C���t���[�쐬
        MainFlow = new MainGameFlow(this);

        // �X�e�[�^�X�p�l��
        Status = statusPanel;
        ////Debug.Log("StatusPanel is launched");

        // �L�[�{�[�h�̃R�[�h�ݒ�
        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // ��O����
        if (setting < 0 && setting > 6) { throw new System.Exception("Invalid Setting, must be within 0~6"); }
        // 0�Ȃ烉���_����I��
        else if (setting == 0)
        {
            setting = Random.Range(1, 6);
        }
        ////Debug.Log("Setting:" + setting);

        // ��ʃT�C�Y������
        Screen.SetResolution(1600, 900, false);

        // FPS�Œ�
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // ���_���ݒ�
        Medal.SetMedalData(0, 3, 0, false);

        // UI �ݒ�
        waitUI.SetWaitManager(Wait);
        playerUI.SetPlayerData(PlayerData);

        // �X�e�[�g�J�n
        MainFlow.stateManager.StartState();
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

        MainFlow.UpdateState();
    }

    // �^�C�}�[�����@�\�̔p��
    private void OnDestroy()
    {
        Wait.DisposeWait();
    }

    // func
    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}

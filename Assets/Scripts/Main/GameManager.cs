using ReelSpinGame_Lots.Flag;
using ReelSpinGame_Medal;
using ReelSpinGame_Medal.Payout;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �Q�[���̊Ǘ�

    // const

    // �e�푀��̃V���A���C�Y
    public enum ControlSets { MaxBet, BetOne, BetTwo, StartAndMax, StopLeft, StopMiddle, StopRight}

    // var

    // �e��@�\

    public MedalManager Medal { get; private set; }
    public FlagLots Lots { get; private set; }
    public WaitManager Wait { get; private set; }
    public ReelManager Reel { get; private set; }
    public PayoutChecker Payout { get; private set; }

    [SerializeField] private ReelManager reelManagerObj;
    [SerializeField] MedalTestUI medalUI;
    [SerializeField] LotsTestUI lotsUI;
    [SerializeField] WaitTestUI waitUI;
    [SerializeField] ReelTestUI reelUI;

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
    [SerializeField] private int setting;

    public KeyCode[] KeyCodes { get; private set; }

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        // ���_���Ǘ�
        Medal = new MedalManager(0, MedalManager.MaxBet);
        Debug.Log("Medal is launched");

        // �t���O�Ǘ�
        Lots = new FlagLots(setting);
        Debug.Log("Lots is launched");

        // �E�F�C�g�Ǘ�
        Wait = new WaitManager(false);
        Debug.Log("Wait is launched");

        // ���[��
        Reel = reelManagerObj.GetComponent<ReelManager>();
        Debug.Log("Reel is launched");

        // �����o������
        Payout = new PayoutChecker(PayoutChecker.PayoutCheckMode.PayoutNormal);
        Debug.Log("Payout is launched");

        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // ���C���t���[�쐬
        MainFlow = new MainGameFlow(this);
    }

    void Start()
    {
        medalUI.SetMedalManager(Medal);
        lotsUI.SetFlagManager(Lots);
        waitUI.SetWaitManager(Wait);
        reelUI.SetReelManager(Reel);
    }

    void Update()
    {
        MainFlow.UpdateState();
    }

    // func
    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}

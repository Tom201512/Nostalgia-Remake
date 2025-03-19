using ReelSpinGame_Medal;
using ReelSpinGame_Lots.Flag;
using UnityEngine;
using System.IO;

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

    // �L�[�ݒ�

    // MAXBET
    [SerializeField] private KeyCode maxBetKey;
    // 1BET
    [SerializeField] private KeyCode betOneKey;
    //2BET
    [SerializeField] private KeyCode betTwoKey;
    // ���[���n��(�܂���MAX BET)
    [SerializeField] private KeyCode startAndMaxBetKey;

    [SerializeField] private KeyCode keyToStopLeft;
    [SerializeField] private KeyCode keyToStopMiddle;
    [SerializeField] private KeyCode keyToStopRight;

    // �m���ݒ�
    [SerializeField] private int setting;
    // ��m����
    [SerializeField] private string flagTableAPath;
    // ���m����
    [SerializeField] private string flagTableBPath;
    // BIG���e�[�u��
    [SerializeField] private string flagTableBIGPath;
    // JAC�͂���m��
    [SerializeField] private int jacNoneProb;

    // �����o���\�̃f�[�^
    [SerializeField] private string normalPayoutPath;
    [SerializeField] private string bigPayoutPath;
    [SerializeField] private string jacPayoutPath;
    [SerializeField] private string payoutLinePath;

    public KeyCode[] KeyCodes { get; private set; }

    // �Q�[���X�e�[�g�p
    public MainGameFlow MainFlow { get; private set; }

    void Awake()
    {
        // ���_���Ǘ�
        Medal = new MedalManager(0, MedalManager.MaxBet);
        Debug.Log("Medal is launched");

        // �t���O�Ǘ�
        Lots = new FlagLots(setting, flagTableAPath, flagTableBPath, flagTableBIGPath, jacNoneProb);
        Debug.Log("Lots is launched");

        // �E�F�C�g�Ǘ�
        Wait = new WaitManager(false);

        // ���[��
        Reel = reelManagerObj.GetComponent<ReelManager>();

        // �����o������
        Payout = new PayoutChecker(normalPayoutPath, bigPayoutPath, jacPayoutPath, payoutLinePath, PayoutChecker.PayoutCheckMode.PayoutNormal);

        KeyCodes = new KeyCode[] { maxBetKey, betOneKey ,betTwoKey, startAndMaxBetKey, keyToStopLeft, keyToStopMiddle, keyToStopRight};

        // ���C���t���[�쐬
        MainFlow = new MainGameFlow(this);
    }

    void Start()
    {
        medalUI.SetMedalManager(Medal);
    }

    void Update()
    {
        MainFlow.UpdateState();
    }

    // func

    // �L�[�ݒ�ύX
    public void ChangeKeyBinds(ControlSets controlSets, KeyCode changeKey) => KeyCodes[(int)controlSets] = changeKey;
}

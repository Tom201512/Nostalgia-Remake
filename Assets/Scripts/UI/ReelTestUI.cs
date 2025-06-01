using TMPro;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

public class ReelTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] private ReelManager reel;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Reels-" + "\n";
        buffer += "CanStop:" + reel.GetCanStopReels() + "\n";

        // ���݂̃��[���ʒu
        buffer += "LeftPos:" + reel.GetCurrentReelPos(ReelID.ReelLeft) + "\n";
        buffer += "MiddlePos:" + reel.GetCurrentReelPos(ReelID.ReelMiddle) + "\n";
        buffer += "RightPos:" + reel.GetCurrentReelPos(ReelID.ReelRight) + "\n" + "\n";

        // ���[�����
        buffer += "LeftStatus:" + reel.GetReelStatus(ReelID.ReelLeft) + "\n";
        buffer += "MiddleStatus:" + reel.GetReelStatus(ReelID.ReelMiddle) + "\n";
        buffer += "RightStatus:" + reel.GetReelStatus(ReelID.ReelRight) + "\n" + "\n";

        // ���[�����x
        buffer += "LeftSpeed:" + reel.GetReelSpeed(ReelID.ReelLeft) + "\n";
        buffer += "MiddleSpeed:" + reel.GetReelSpeed(ReelID.ReelMiddle) + "\n";
        buffer += "RightSpeed:" + reel.GetReelSpeed(ReelID.ReelRight) + "\n" + "\n";

        // ���[���p�x
        buffer += "LeftDegree:" + reel.GetReelDegree(ReelID.ReelLeft) + "\n";
        buffer += "MiddleDegree:" + reel.GetReelDegree(ReelID.ReelMiddle) + "\n";
        buffer += "RightDegree:" + reel.GetReelDegree(ReelID.ReelRight) + "\n" + "\n";

        // ��~�\��ʒu
        buffer += "LeftWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelLeft)) + "\n";
        buffer += "MiddleWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelMiddle)) + "\n";
        buffer += "RightWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelRight)) + "\n" + "\n";

        // ��~�����ʒu
        buffer += "LeftStopped:" + (reel.GetStoppedReelPos(ReelID.ReelLeft)) + "\n";
        buffer += "MiddleStopped:" + (reel.GetStoppedReelPos(ReelID.ReelMiddle)) + "\n";
        buffer += "RightStopped:" + (reel.GetStoppedReelPos(ReelID.ReelRight)) + "\n" + "\n";

        // �X�x���R�}��
        buffer += "LeftDelay:" + reel.GetLastDelay(ReelID.ReelLeft) + "\n";
        buffer += "MiddleDelay:" + reel.GetLastDelay(ReelID.ReelMiddle) + "\n";
        buffer += "RightDelay:" + reel.GetLastDelay(ReelID.ReelRight) + "\n" + "\n";

        // �g�p���ꂽ�e�[�u��ID
        buffer += "LeftTableID" + reel.GetUsedReelTableID(ReelID.ReelLeft) + "\n";
        buffer += "MiddleTableID" + reel.GetUsedReelTableID(ReelID.ReelMiddle) + "\n";
        buffer += "RightTableID" + reel.GetUsedReelTableID(ReelID.ReelRight) + "\n" + "\n";
        // �����_���l
        buffer += "Random" + reel.GetRandomValue() + "\n" + "\n";

        text.text = buffer;
    }
}

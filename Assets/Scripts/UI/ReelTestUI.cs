using TMPro;
using UnityEngine;

public class ReelTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    ReelManager reel;
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

        // ���݂̃��[���ʒu
        buffer += "LeftPos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelLeft);
        buffer += "MiddlePos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelMiddle);
        buffer += "RightPos:" + reel.GetCurrentReelPos((int)ReelManager.ReelID.ReelRight) + "\n";

        // ��~�����ʒu
        buffer += "LeftStopped:" + reel.GetStoppedReelPos((int)ReelManager.ReelID.ReelLeft);
        buffer += "MiddleStopped:" + reel.GetStoppedReelPos((int)ReelManager.ReelID.ReelMiddle);
        buffer += "RightStopped:" + reel.GetStoppedReelPos((int)ReelManager.ReelID.ReelRight) + "\n";

        // �X�x���R�}��
        buffer += "LeftDelay:" + reel.GetLastDelay((int)ReelManager.ReelID.ReelLeft);
        buffer += "MiddleDelay:" + reel.GetLastDelay((int)ReelManager.ReelID.ReelMiddle);
        buffer += "RightDelay:" + reel.GetLastDelay((int)ReelManager.ReelID.ReelRight) + "\n";

        // �g�p���ꂽ�e�[�u��ID
        buffer += "LeftTableID" + reel.GetLastTableID((int)ReelManager.ReelID.ReelLeft);
        buffer += "MiddleTableID" + reel.GetLastTableID((int)ReelManager.ReelID.ReelMiddle);
        buffer += "RightTableID" + reel.GetLastTableID((int)ReelManager.ReelID.ReelRight) + "\n";

        text.text = buffer;
    }

    public void SetReelManager(ReelManager reelManager) => this.reel = reelManager;
}

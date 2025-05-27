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
        buffer += "CanStop:" + reel.Data.CanStop + "\n";

        // 現在のリール位置
        buffer += "LeftPos:" + reel.GetCurrentReelPos((int)ReelID.ReelLeft) + "\n";
        buffer += "MiddlePos:" + reel.GetCurrentReelPos((int)ReelID.ReelMiddle) + "\n";
        buffer += "RightPos:" + reel.GetCurrentReelPos((int)ReelID.ReelRight) + "\n" + "\n";

        // 停止した位置
        buffer += "LeftStopped:" + (reel.GetStoppedReelPos((int)ReelID.ReelLeft)) + "\n";
        buffer += "MiddleStopped:" + (reel.GetStoppedReelPos((int)ReelID.ReelMiddle)) + "\n";
        buffer += "RightStopped:" + (reel.GetStoppedReelPos((int)ReelID.ReelRight)) + "\n" + "\n";

        // スベリコマ数
        buffer += "LeftDelay:" + reel.GetLastDelay((int)ReelID.ReelLeft) + "\n";
        buffer += "MiddleDelay:" + reel.GetLastDelay((int)ReelID.ReelMiddle) + "\n";
        buffer += "RightDelay:" + reel.GetLastDelay((int)ReelID.ReelRight) + "\n" + "\n";

        // 使用されたテーブルID
        buffer += "LeftTableID" + reel.Data.ReelTableManager.UsedReelTableID[(int)ReelID.ReelLeft] + "\n";
        buffer += "MiddleTableID" + reel.Data.ReelTableManager.UsedReelTableID[(int)ReelID.ReelMiddle] + "\n";
        buffer += "RightTableID" + reel.Data.ReelTableManager.UsedReelTableID[(int)ReelID.ReelRight] + "\n" + "\n";

        // ランダム値
        buffer += "Random" + reel.Data.RandomValue + "\n" + "\n";

        text.text = buffer;
    }
}

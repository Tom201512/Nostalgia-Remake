using TMPro;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

public class ReelTestUI : UIBaseClass
{
    TextMeshProUGUI text;
    [SerializeField] private ReelManager reel;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string buffer = "";

        buffer += "Reels-" + "\n";
        buffer += "CanStop:" + reel.GetCanStopReels() + "\n";
        buffer += "HasForceStop:" + reel.HasForceStop() + "\n";

        // 現在のリール位置
        buffer += "LeftPos:" + reel.GetCurrentReelPos(ReelID.ReelLeft) + "\n";
        buffer += "MiddlePos:" + reel.GetCurrentReelPos(ReelID.ReelMiddle) + "\n";
        buffer += "RightPos:" + reel.GetCurrentReelPos(ReelID.ReelRight) + "\n" + "\n";

        // リール状態
        buffer += "LeftStatus:" + reel.GetReelStatus(ReelID.ReelLeft) + "\n";
        buffer += "MiddleStatus:" + reel.GetReelStatus(ReelID.ReelMiddle) + "\n";
        buffer += "RightStatus:" + reel.GetReelStatus(ReelID.ReelRight) + "\n" + "\n";

        // リール速度
        buffer += "LeftSpeed:" + reel.GetReelSpeed(ReelID.ReelLeft) + "\n";
        buffer += "MiddleSpeed:" + reel.GetReelSpeed(ReelID.ReelMiddle) + "\n";
        buffer += "RightSpeed:" + reel.GetReelSpeed(ReelID.ReelRight) + "\n" + "\n";

        // リール角度
        buffer += "LeftDegree:" + reel.GetReelDegree(ReelID.ReelLeft) + "\n";
        buffer += "MiddleDegree:" + reel.GetReelDegree(ReelID.ReelMiddle) + "\n";
        buffer += "RightDegree:" + reel.GetReelDegree(ReelID.ReelRight) + "\n" + "\n";

        // 停止予定位置
        buffer += "LeftWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelLeft)) + "\n";
        buffer += "MiddleWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelMiddle)) + "\n";
        buffer += "RightWillStopAt:" + (reel.GetWillStopReelPos(ReelID.ReelRight)) + "\n" + "\n";

        // 停止した位置
        buffer += "LeftStopped:" + (reel.GetPushedPos(ReelID.ReelLeft)) + "\n";
        buffer += "MiddleStopped:" + (reel.GetPushedPos(ReelID.ReelMiddle)) + "\n";
        buffer += "RightStopped:" + (reel.GetPushedPos(ReelID.ReelRight)) + "\n" + "\n";

        // スベリコマ数
        buffer += "LeftDelay:" + reel.GetLastDelay(ReelID.ReelLeft) + "\n";
        buffer += "MiddleDelay:" + reel.GetLastDelay(ReelID.ReelMiddle) + "\n";
        buffer += "RightDelay:" + reel.GetLastDelay(ReelID.ReelRight) + "\n" + "\n";

        // 使用されたテーブルID
        buffer += "LeftTableTID" + reel.GetUsedReelTID(ReelID.ReelLeft) + "\n";
        buffer += "MiddleTableTID" + reel.GetUsedReelTID(ReelID.ReelMiddle) + "\n";
        buffer += "RightTableTID" + reel.GetUsedReelTID(ReelID.ReelRight) + "\n" + "\n";

        // 使用された組み合わせID
        buffer += "LeftTableCID" + reel.GetUsedReelCID(ReelID.ReelLeft) + "\n";
        buffer += "MiddleTableCID" + reel.GetUsedReelCID(ReelID.ReelMiddle) + "\n";
        buffer += "RightTableCID" + reel.GetUsedReelCID(ReelID.ReelRight) + "\n" + "\n";

        // ランダム値
        buffer += "Random" + reel.GetRandomValue() + "\n" + "\n";

        text.text = buffer;
    }
}

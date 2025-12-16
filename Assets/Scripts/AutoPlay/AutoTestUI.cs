using ReelSpinGame_AutoPlay;
using TMPro;
using static ReelSpinGame_Reels.ReelObjectPresenter;

public class AutoTestUI : UIBaseClass
{
    // オートプレイUI用
    TextMeshProUGUI text;
    private AutoPlayFunction auto;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string buffer = "";

        buffer += "Auto-" + "\n";
        buffer += "AutoEnabled:" + auto.HasAuto + "\n";
        buffer += "AutoEndCondition:" + auto.GetConditionID() + "\n";
        buffer += "AutoSpeed:" + auto.GetSpeedName() + "\n";
        buffer += "AutoOrderOption:" + auto.GetOrderName() + "\n";
        buffer += "AutoPos: L:" + auto.AutoStopPos[(int)ReelID.ReelLeft] + " M:" + auto.AutoStopPos[(int)ReelID.ReelMiddle] + " R:" + auto.AutoStopPos[(int)ReelID.ReelRight] + "\n";
        buffer += "AutoPosDecided:" + auto.HasStopPosDecided + "\n";
        buffer += "RemainingG:" + auto.RemainingAutoGames;

        text.text = buffer;
    }

    public void SetAutoFunction(AutoPlayFunction auto) => this.auto = auto;
}

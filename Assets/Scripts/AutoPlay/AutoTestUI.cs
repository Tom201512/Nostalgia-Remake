using ReelSpinGame_AutoPlay;
using TMPro;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;

public class AutoTestUI : UIBaseClass
{
    // �I�[�g�v���CUI�p
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
        buffer += "AutoEndCondition:" + auto.GetConditionName() + "\n";
        buffer += "AutoSpeed:" + auto.GetSpeedName() + "\n";
        buffer += "AutoOrderOption:" + auto.GetOrderName() + "\n";
        buffer += "AutoPos: L:" + auto.AutoStopPos[(int)ReelLeft] + " M:" + auto.AutoStopPos[(int)ReelMiddle] + " R:" + auto.AutoStopPos[(int)ReelRight] + "\n";
        buffer += "AutoPosDecided:" + auto.HasStopPosDecided + "\n";
        buffer += "RemainingG:" + auto.RemainingAutoGames;

        text.text = buffer;
    }

    public void SetAutoFunction(AutoPlayFunction auto) => this.auto = auto;
}

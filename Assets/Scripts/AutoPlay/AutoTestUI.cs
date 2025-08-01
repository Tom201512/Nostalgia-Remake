using ReelSpinGame_AutoPlay;
using TMPro;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;

public class AutoTestUI : UIBaseClass
{
    // オートプレイUI用
    TextMeshProUGUI text;
    private AutoPlayFunction auto;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Auto-" + "\n";
        buffer += "AutoEnabled:" + auto.HasAuto + "\n";
        buffer += "AutoSpeed:" + auto.AutoSpeed + "\n";
        buffer += "AutoOrderOption:" + auto.GetOrderName() + "\n";
        buffer += "AutoPos: L:" + auto.AutoStopPos[(int)ReelLeft] + " M:" + auto.AutoStopPos[(int)ReelMiddle] + " R:" + auto.AutoStopPos[(int)ReelRight] + "\n";
        buffer += "AutoPosDecided:" + auto.HasStopPosDecided + "\n";

        text.text = buffer;
    }

    public void SetAutoFunction(AutoPlayFunction auto) => this.auto = auto;
}

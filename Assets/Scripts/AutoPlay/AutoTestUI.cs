using ReelSpinGame_AutoPlay;
using static ReelSpinGame_AutoPlay.AutoPlayFunction.AutoStopOrder;
using static ReelSpinGame_Reels.ReelManagerBehaviour.ReelID;
using TMPro;
using UnityEngine;

public class AutoTestUI : MonoBehaviour
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
        buffer += "AutoOrderOption:" + auto.AutoStopOrderOption.ToString() + "\n";
        buffer += "AutoPos: L:" + auto.AutoStopPos[(int)ReelLeft] + " M:" + auto.AutoStopPos[(int)ReelMiddle] + " R:" + auto.AutoStopPos[(int)ReelRight] + "\n";
        buffer += "AutoPosDecided:" + auto.HasStopPosDecided + "\n";

        text.text = buffer;
    }

    public void SetAutoFunction(AutoPlayFunction auto) => this.auto = auto;
}

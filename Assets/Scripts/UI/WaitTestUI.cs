using TMPro;
using UnityEngine;

public class WaitTestUI : UIBaseClass
{
    TextMeshProUGUI text;
    WaitManager wait;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string buffer = "";

        buffer += "Wait-" + "\n";
        buffer += "WaitEnable?:" + wait.HasWait + "\n";
        buffer += "WaitCut:" + wait.HasWaitCut;
        text.text = buffer;
    }

    public void SetWaitManager(WaitManager waitManager) => this.wait = waitManager;
}

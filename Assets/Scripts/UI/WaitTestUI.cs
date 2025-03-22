using TMPro;
using UnityEngine;

public class WaitTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    WaitManager wait;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Wait-" + "\n";
        buffer += "WaitEnable?:" + wait.hasWait + "\n";
        buffer += "WaitCut:" + wait.hasWaitCut;
        text.text = buffer;
    }

    public void SetWaitManager(WaitManager waitManager) => this.wait = waitManager;
}

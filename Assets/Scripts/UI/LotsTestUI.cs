using ReelSpinGame_Lots.Flag;
using TMPro;
using UnityEngine;

public class LotsTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    FlagLots lots;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Lots-" + "\n";
        buffer += "Flag:" + lots.CurrentFlag + "\n";
        buffer += "Table:" + lots.CurrentTable + "\n";
        buffer += "Counter:" + lots.FlagCounter.Counter + "\n";

        text.text = buffer;
    }

    public void SetFlagManager(FlagLots lots) => this.lots = lots;
}

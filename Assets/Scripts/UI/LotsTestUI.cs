using ReelSpinGame_Lots.Flag;
using TMPro;
using UnityEngine;

public class LotsTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    private FlagLots lots;
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
        buffer += "Flag:" + lots.FlagBehaviour.CurrentFlag + "\n";
        buffer += "Table:" + lots.FlagBehaviour.CurrentTable + "\n";
        buffer += "Counter:" + lots.FlagBehaviour.FlagCounter.Counter + "\n";

        text.text = buffer;
    }

    public void SetFlagManager(FlagLots lots) => this.lots = lots;
}

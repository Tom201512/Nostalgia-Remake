using ReelSpinGame_Lots.Flag;
using TMPro;
using UnityEngine;

public class LotsTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] private FlagLots lots;
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
        buffer += "Flag:" + lots.Data.CurrentFlag + "\n";
        buffer += "Table:" + lots.Data.CurrentTable + "\n";
        buffer += "Counter:" + lots.Data.FlagCounter.Counter + "\n";

        text.text = buffer;
    }
}

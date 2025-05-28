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
        buffer += "Flag:" + lots.GetCurrentFlag() + "\n";
        buffer += "Table:" + lots.GetCurrentTable() + "\n";
        buffer += "Counter:" + lots.GetCounter() + "\n";

        text.text = buffer;
    }
}

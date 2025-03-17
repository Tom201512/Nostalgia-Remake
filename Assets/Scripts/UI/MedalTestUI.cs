using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    MedalManager medal;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Credits:" + medal.Credits + "\n";
        buffer += "Bet:" + medal.CurrentBet + "\n";
        buffer += "Payout:" + medal.PayoutAmounts;

        text.text = buffer;
    }

    public void SetMedalManager(MedalManager medalManager) => medal = medalManager;
}

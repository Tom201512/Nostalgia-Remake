using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : MonoBehaviour
{
    // var
    TextMeshProUGUI text;
    // ƒƒ_ƒ‹‚Ìˆ—
    [SerializeField] private MedalManager medal;

    // func
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string buffer = "";

        buffer += "Medal-" + "\n";
        buffer += "Credits:" + medal.Data.Credits + "\n";
        buffer += "Bet:" + medal.Data.CurrentBet + "\n";
        buffer += "Payout:" + medal.Data.PayoutAmounts + "\n";
        buffer += "MaxBet:" + medal.Data.MaxBetAmounts + "\n";
        buffer += "LastBet:" + medal.Data.LastBetAmounts + "\n";
        buffer += "Replay:" + medal.Data.HasReplay;

        text.text = buffer;
    }
}

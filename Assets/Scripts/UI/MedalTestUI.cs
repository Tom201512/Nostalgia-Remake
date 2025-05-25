using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : MonoBehaviour
{
    // var
    TextMeshProUGUI text;
    // ƒƒ_ƒ‹‚Ìˆ—
    private MedalManager medal;

    // func
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string buffer = "";

        buffer += "Medal-" + "\n";
        buffer += "Credits:" + medal.MedalBehaviour.Credits + "\n";
        buffer += "Bet:" + medal.MedalBehaviour.CurrentBet + "\n";
        buffer += "Payout:" + medal.MedalBehaviour.PayoutAmounts + "\n";
        buffer += "MaxBet:" + medal.MedalBehaviour.MaxBetAmounts + "\n";
        buffer += "LastBet:" + medal.MedalBehaviour.LastBetAmounts + "\n";
        buffer += "Replay:" + medal.MedalBehaviour.HasReplay;

        text.text = buffer;
    }

    public void SetMedalManager(MedalManager medalManager) => medal = medalManager;
}

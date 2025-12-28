using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : UIBaseClass
{
    TextMeshProUGUI text;
    // ÉÅÉ_ÉãÇÃèàóù
    [SerializeField] private MedalManager medal;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string buffer = "";

        buffer += "Medal-" + "\n";
        buffer += "Credit:" + medal.Credit + "\n";
        buffer += "Bet:" + medal.CurrentBet + "\n";
        buffer += "MaxBet:" + medal.MaxBetAmount + "\n";
        buffer += "LastBet:" + medal.LastBetAmount + "\n";
        buffer += "Payout:" + medal.RemainingPayout + "\n";
        buffer += "LastPayout:" + medal.LastPayoutAmount + "\n";
        buffer += "BetFinished:" + medal.IsFinishedBet + "\n";
        buffer += "Replay:" + medal.HasReplay;

        text.text = buffer;
    }
}

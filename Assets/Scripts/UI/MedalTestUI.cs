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
        buffer += "Credit:" + medal.GetCredit() + "\n";
        buffer += "Bet:" + medal.GetCurrentBet() + "\n";
        buffer += "MaxBet:" + medal.GetMaxBet() + "\n";
        buffer += "LastBet:" + medal.GetLastBetAmount() + "\n";
        buffer += "Payout:" + medal.GetRemainingPayout() + "\n";
        buffer += "LastPayout:" + medal.GetLastPayout() + "\n";
        buffer += "BetFinished:" + medal.GetBetFinished() + "\n";
        buffer += "Replay:" + medal.GetHasReplay();

        text.text = buffer;
    }
}

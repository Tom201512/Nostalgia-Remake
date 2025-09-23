using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : UIBaseClass
{
    // var
    TextMeshProUGUI text;
    // ÉÅÉ_ÉãÇÃèàóù
    [SerializeField] private MedalManager medal;

    // func
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
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

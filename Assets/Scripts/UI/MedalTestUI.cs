using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : UIBaseClass
{
    // var
    TextMeshProUGUI text;
    // ���_���̏���
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
        buffer += "Credits:" + medal.GetCredits() + "\n";
        buffer += "Bet:" + medal.GetCurrentBet() + "\n";
        buffer += "MaxBet:" + medal.GetMaxBet() + "\n";
        buffer += "LastBet:" + medal.GetLastBetAmounts() + "\n";
        buffer += "Payout:" + medal.GetRemainingPayouts() + "\n";
        buffer += "LastPayout:" + medal.GetLastPayout() + "\n";
        buffer += "BetFinished:" + medal.GetBetFinished() + "\n";
        buffer += "Replay:" + medal.GetHasReplay();

        text.text = buffer;
    }
}

using ReelSpinGame_Bonus;
using TMPro;
using UnityEngine;

public class BonusTestUI : UIBaseClass
{
    TextMeshProUGUI text;
    [SerializeField] private BonusManager bonusManager;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string buffer = "";

        buffer += "Bonus-" + "\n";
        buffer += "Holding:" + bonusManager.GetHoldingBonusID() + "\n";
        buffer += "Status:" + bonusManager.GetCurrentBonusStatus() + "\n";
        buffer += "BIG_Color:" + bonusManager.GetBigChanceType() + "\n";
        buffer += "BIG_Games:" + bonusManager.GetRemainingBigGames() + "\n";
        buffer += "BIG_JACIN:" + bonusManager.GetRemainingJacIn() + "\n";
        buffer += "JAC_Games:" + bonusManager.GetRemainingJacGames() + "\n";
        buffer += "JAC_Hits:" + bonusManager.GetRemainingJacHits() + "\n" + "\n";
        buffer += "TotalPayout:" + bonusManager.GetCurrentBonusPayout() + "\n";
        buffer += "ZonePayout:" + bonusManager.GetCurrentZonePayout() + "\n";
        buffer += "HasZone:" + bonusManager.GetHasZone() + "\n";
        buffer += "LastZone:" + bonusManager.GetLastZonePayout() + "\n";

        text.text = buffer;
    }
}
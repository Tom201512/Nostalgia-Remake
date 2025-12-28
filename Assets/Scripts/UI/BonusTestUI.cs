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
        // ストック中のボーナス
        buffer += "Holding:" + bonusManager.GetHoldingBonusID() + "\n";
        // ボーナス状態
        buffer += "Status:" + bonusManager.GetCurrentBonusStatus() + "\n";

        // BIG中の状態
        buffer += "BIG_Color:" + bonusManager.GetBigChanceColor() + "\n";
        buffer += "BIG_Games:" + bonusManager.GetRemainingBigGames() + "\n";
        buffer += "BIG_JACIN:" + bonusManager.GetRemainingJacIn() + "\n";

        // JAC中の状態
        buffer += "JAC_Games:" + bonusManager.GetRemainingJacGames() + "\n";
        buffer += "JAC_Hits:" + bonusManager.GetRemainingJacHits() + "\n" + "\n";

        // 獲得枚数表示
        buffer += "TotalPayout:" + bonusManager.GetCurrentBonusPayout() + "\n";
        buffer += "ZonePayout:" + bonusManager.GetCurrentZonePayout() + "\n";
        buffer += "HasZone:" + bonusManager.GetHasZone() + "\n";
        buffer += "LastZone:" + bonusManager.GetLastZonePayout() + "\n";

        text.text = buffer;
    }
}
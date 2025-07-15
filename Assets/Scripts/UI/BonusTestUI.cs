using UnityEngine;
using TMPro;
using ReelSpinGame_Bonus;

public class BonusTestUI : UIBaseClass
{
    TextMeshProUGUI text;
    [SerializeField] private BonusManager bonusManager;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Bonus-" + "\n";
        // ストック中のボーナス
        buffer += "Holding:" + bonusManager.GetHoldingBonusID() + "\n";
        // ボーナス状態
        buffer += "Status:" + bonusManager.GetCurrentBonusStatus() + "\n";

        // BIG中の状態
        buffer += "BIG_Color:" + bonusManager.GetBigChangeColor() + "\n";
        buffer += "BIG_Games:" + bonusManager.GetRemainingBigGames() + "\n";
        buffer += "BIG_JACIN:" + bonusManager.GetRemainingJacIn() + "\n";

        // JAC中の状態
        buffer += "JAC_Games:" + bonusManager.GetRemainingJacGames() + "\n";
        buffer += "JAC_Hits:" + bonusManager.GetRemainingJacHits()+ "\n" + "\n";

        // 獲得枚数表示
        buffer += "TotalPayouts:" + bonusManager.GetCurrentBonusPayouts()+ "\n";
        buffer += "ZonePayouts:" + bonusManager.GetCurrentZonePayouts()+ "\n";
        buffer += "HasZone:" + bonusManager.GetHasZone() + "\n";

        text.text = buffer;
    }
}
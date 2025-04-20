using UnityEngine;
using TMPro;
using ReelSpinGame_Bonus;

public class BonusTestUI : MonoBehaviour
{
    TextMeshProUGUI text;
    BonusManager bonus;
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
        buffer += "Holding:" + bonus.HoldingBonusID + "\n";
        // ボーナス状態
        buffer += "Status:" + bonus.CurrentBonusStatus + "\n";

        // BIG中の状態
        buffer += "BIG_Games:" + bonus.RemainingBigGames + "\n";
        buffer += "BIG_JACIN:" + bonus.RemainingJacIn + "\n";

        // JAC中の状態
        buffer += "JAC_Games:" + bonus.RemainingJacGames + "\n";
        buffer += "JAC_Hits:" + bonus.RemainingJacHits + "\n";

        text.text = buffer;
    }

    public void SetBonusManager(BonusManager bonusManager) => this.bonus = bonusManager;
}
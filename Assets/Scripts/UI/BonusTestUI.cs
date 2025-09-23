using UnityEngine;
using TMPro;
using ReelSpinGame_Bonus;

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
        // �X�g�b�N���̃{�[�i�X
        buffer += "Holding:" + bonusManager.GetHoldingBonusID() + "\n";
        // �{�[�i�X���
        buffer += "Status:" + bonusManager.GetCurrentBonusStatus() + "\n";

        // BIG���̏��
        buffer += "BIG_Color:" + bonusManager.GetBigChanceColor() + "\n";
        buffer += "BIG_Games:" + bonusManager.GetRemainingBigGames() + "\n";
        buffer += "BIG_JACIN:" + bonusManager.GetRemainingJacIn() + "\n";

        // JAC���̏��
        buffer += "JAC_Games:" + bonusManager.GetRemainingJacGames() + "\n";
        buffer += "JAC_Hits:" + bonusManager.GetRemainingJacHits()+ "\n" + "\n";

        // �l�������\��
        buffer += "TotalPayout:" + bonusManager.GetCurrentBonusPayout()+ "\n";
        buffer += "ZonePayout:" + bonusManager.GetCurrentZonePayout()+ "\n";
        buffer += "HasZone:" + bonusManager.GetHasZone() + "\n";
        buffer += "LastZone:" + bonusManager.GetLastZonePayout() + "\n";

        text.text = buffer;
    }
}
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
        // �X�g�b�N���̃{�[�i�X
        buffer += "Holding:" + bonus.HoldingBonusID + "\n";
        // �{�[�i�X���
        buffer += "Status:" + bonus.CurrentBonusStatus + "\n";

        // BIG���̏��
        buffer += "BIG_Games:" + bonus.RemainingBigGames + "\n";
        buffer += "BIG_JACIN:" + bonus.RemainingJacIn + "\n";

        // JAC���̏��
        buffer += "JAC_Games:" + bonus.RemainingJacGames + "\n";
        buffer += "JAC_Hits:" + bonus.RemainingJacHits + "\n";

        text.text = buffer;
    }

    public void SetBonusManager(BonusManager bonusManager) => this.bonus = bonusManager;
}
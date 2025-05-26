using UnityEngine;
using TMPro;
using ReelSpinGame_Bonus;

public class BonusTestUI : MonoBehaviour
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
        // �X�g�b�N���̃{�[�i�X
        buffer += "Holding:" + bonusManager.Data.HoldingBonusID + "\n";
        // �{�[�i�X���
        buffer += "Status:" + bonusManager.Data.CurrentBonusStatus + "\n";

        // BIG���̏��
        buffer += "BIG_Games:" + bonusManager.Data.RemainingBigGames + "\n";
        buffer += "BIG_JACIN:" + bonusManager.Data.RemainingJacIn + "\n";

        // JAC���̏��
        buffer += "JAC_Games:" + bonusManager.Data.RemainingJacGames + "\n";
        buffer += "JAC_Hits:" + bonusManager.Data.RemainingJacHits + "\n";

        text.text = buffer;
    }
}
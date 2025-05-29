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
        buffer += "Holding:" + bonusManager.GetHoldingBonusID() + "\n";
        // �{�[�i�X���
        buffer += "Status:" + bonusManager.GetCurrentBonusStatus() + "\n";

        // BIG���̏��
        buffer += "BIG_Games:" + bonusManager.GetRemainingBigGames() + "\n";
        buffer += "BIG_JACIN:" + bonusManager.GetRemainingJacIn() + "\n";

        // JAC���̏��
        buffer += "JAC_Games:" + bonusManager.GetRemainingJacGames() + "\n";
        buffer += "JAC_Hits:" + bonusManager.GetRemainingJacHits()+ "\n";

        text.text = buffer;
    }
}
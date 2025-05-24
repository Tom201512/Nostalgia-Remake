using ReelSpinGame_Medal;
using TMPro;
using UnityEngine;

public class MedalTestUI : MonoBehaviour
{
    // const
    // �f�t�H���g�̖��邳(�_����)
    const byte TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    const byte TurnOffValue = 120;

    // var
    TextMeshProUGUI text;
    // ���_���}�l�[�W���[
    MedalManager medal;
    // ���_����UI�p�l��
    [SerializeField] MedalPanel medalPanel;

    // func
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        string buffer = "";

        buffer += "Medal-" + "\n";
        buffer += "Credits:" + medal.Credits + "\n";
        buffer += "Bet:" + medal.CurrentBet + "\n";
        buffer += "Payout:" + medal.PayoutAmounts + "\n";
        buffer += "MaxBet:" + medal.MaxBetAmounts + "\n";
        buffer += "LastBet:" + medal.LastBetAmounts + "\n";
        buffer += "Replay:" + medal.HasReplay;

        text.text = buffer;

        ChangeMedalPanelBrightness();
    }

    public void SetMedalManager(MedalManager medalManager) => medal = medalManager;

    public void ChangeMedalPanelBrightness()
    {
        if(medal.CurrentBet == 0)
        {
            if (medal.LastBetAmounts >= 1)
            {
                medalPanel.ChangeMedal1Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal1Lamp(TurnOffValue);
            }

            if (medal.LastBetAmounts >= 2)
            {
                medalPanel.ChangeMedal2Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal2Lamp(TurnOffValue);
            }

            if (medal.LastBetAmounts >= 3)
            {
                medalPanel.ChangeMedal3Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal3Lamp(TurnOffValue);
            }
        }
        else
        {
            if (medal.CurrentBet >= 1)
            {
                medalPanel.ChangeMedal1Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal1Lamp(TurnOffValue);
            }

            if (medal.CurrentBet >= 2)
            {
                medalPanel.ChangeMedal2Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal2Lamp(TurnOffValue);
            }

            if (medal.CurrentBet >= 3)
            {
                medalPanel.ChangeMedal3Lamp(TurnOnValue);
            }
            else
            {
                medalPanel.ChangeMedal3Lamp(TurnOffValue);
            }
        }
    }
}

using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Bonus
{
    // �{�[�i�X���I�񐔗p
    public class BonusInfoUI : MonoBehaviour
    {
        // const
        // �ő�\���\�{�[�i�X��
        private const int MaximumBonusCounts = 999;

        // var
        [SerializeField] private TextMeshProUGUI bigCount;
        [SerializeField] private TextMeshProUGUI regCount;

        // �{�[�i�X���I���X�V
        public void UpdateBonusUI(PlayerDatabase playerDatabase)
        {
            bigCount.text = "BIG:" + "\t" + Math.Clamp(playerDatabase.BigTimes, 0, MaximumBonusCounts).ToString();
            regCount.text = "REG:" + "\t" + Math.Clamp(playerDatabase.RegTimes, 0, MaximumBonusCounts).ToString();
        }
    }
}
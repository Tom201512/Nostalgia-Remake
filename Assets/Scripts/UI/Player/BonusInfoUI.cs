using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Bonus
{
    // ボーナス当選回数用
    public class BonusInfoUI : MonoBehaviour
    {
        private const int MaximumBonusCount = 999;        // 最大表示可能ボーナス回数

        [SerializeField] private TextMeshProUGUI bigCount;  // BIG回数
        [SerializeField] private TextMeshProUGUI regCount;  // REG回数

        // ボーナス当選情報更新
        public void UpdateBonusUI(PlayerDatabase playerDatabase)
        {
            bigCount.text = "BIG:" + Math.Clamp(playerDatabase.BigTimes, 0, MaximumBonusCount).ToString();
            regCount.text = "REG:" + Math.Clamp(playerDatabase.RegTimes, 0, MaximumBonusCount).ToString();
        }
    }
}
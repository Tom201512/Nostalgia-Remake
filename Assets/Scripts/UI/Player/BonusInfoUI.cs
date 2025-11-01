using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Bonus
{
    // ボーナス当選回数用
    public class BonusInfoUI : MonoBehaviour
    {
        // const
        // 最大表示可能ボーナス回数
        private const int MaximumBonusCount = 999;

        // var
        [SerializeField] private TextMeshProUGUI bigCount;
        [SerializeField] private TextMeshProUGUI regCount;

        // ボーナス当選情報更新
        public void UpdateBonusUI(PlayerDatabase playerDatabase)
        {
            bigCount.text = "BIG:" + Math.Clamp(playerDatabase.BigTimes, 0, MaximumBonusCount).ToString();
            regCount.text = "REG:" + Math.Clamp(playerDatabase.RegTimes, 0, MaximumBonusCount).ToString();
        }
    }
}
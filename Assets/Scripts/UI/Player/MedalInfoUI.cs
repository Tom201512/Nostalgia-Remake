using ReelSpinGame_Datas;
using ReelSpinGame_Medal;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Medal
{
    // メダル枚数表示用
    public class MedalInfoUI : MonoBehaviour
    {
        // 最大表示可能メダル枚数
        private const int MaximumGameCount = 99999;

        // 最大表示可能機械割
        private const float MaximumPayoutRatio = 999.99f;

        [SerializeField] private TextMeshProUGUI medalUI;
        [SerializeField] private TextMeshProUGUI payoutRateUI;

        // メダル情報更新
        public void UpdateMedalUI(PlayerMedalData playerMedalData, MedalManager medalManager)
        {
            // 手持ちメダル、OUT枚数は現在の残り払い出し枚数に合わせて減らす)

            medalUI.text = "Medal:" + "\n" + Math.Clamp(playerMedalData.CurrentPlayerMedal - medalManager.RemainingPayout
                , 0, MaximumGameCount).ToString();

            // 機械割計算

            float payoutRate = 0f;
            // 機械割
            if (playerMedalData.CurrentInMedal > 0 && playerMedalData.CurrentOutMedal > 0)
            {
                payoutRate = (float)(playerMedalData.CurrentOutMedal - medalManager.RemainingPayout) / playerMedalData.CurrentInMedal * 100;
            }

            payoutRateUI.text = "Payout:" + "\n" + Mathf.Clamp(payoutRate, 0f, MaximumPayoutRatio).ToString("F2") + "%";
        }
    }
}
using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class BonusDataUI : MonoBehaviour
    {
        // ボーナス中の情報

        // データ部分
        [SerializeField] TextMeshProUGUI dataTextUI;

        public void UpdateText(PlayerDatabase player)
        {
            float probability = 0.0f;
            float rate = 0.0f;
            string data = "\n";

            // 直近ボーナスゲームの表示(ただし入賞していないものは表示しない)
            // 2回目以降のボーナスは入賞時ゲームが記録していれば表示
            if(player.BonusHitRecord.Count > 1)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                // そうでなければ2つ前を表示
                else
                {
                    data += player.BonusHitRecord[^2].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^2].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^2].BonusPayout + "\n";
                }
            }
            // 初回ボーナスの場合は入賞していなければ表示しない
            else if(player.BonusHitRecord.Count > 0)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                // そうでなければ2つ前を表示
                else
                {
                    data += "-------\n";
                    data += "-------\n";
                    data += "-------\n";
                }
            }
            // 非成立の場合は表示しない
            else
            {
                data += "-------\n";
                data += "-------\n";
                data += "-------\n";
            }

            data += "\n\n";

            // JACハズシ
            // 成功回数
            data += player.PlayerAnalyticsData.BigJacAvoidTimes + "\n";
            // ビタハズシ回数
            data += player.PlayerAnalyticsData.BigJacPerfectAvoidTimes + "\n";
            // アシストハズシ回数
            data += player.PlayerAnalyticsData.BigJacAssistedAvoidTimes + "\n";

            // 成功確率
            if (player.PlayerAnalyticsData.BigJacAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacInTimes / player.PlayerAnalyticsData.BigJacAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ビタハズシ成功確率
            if (player.PlayerAnalyticsData.BigJacPerfectAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacAvoidTimes / player.PlayerAnalyticsData.BigJacPerfectAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            data += "\n\n";


            // ボーナスゲーム中ハズレ
            // 回数
            data += player.PlayerAnalyticsData.JacGameNoneTimes + "\n";
            // 確率
            if (player.PlayerAnalyticsData.JacGameNoneTimes > 0)
            {
                probability = (float)player.PlayerAnalyticsData.JacGamesCount / player.PlayerAnalyticsData.JacGameNoneTimes;
                data += "1/" + probability.ToString("F2");
            }
            else
            {
                data += "1/---.--";
            }

            dataTextUI.text = data;
        }
     }
}

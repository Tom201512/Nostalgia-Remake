using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class ProbabilityDataUI : MonoBehaviour
    {
        // 通常時確率などの情報

        // 情報画面
        [SerializeField] TextMeshProUGUI textUI;

        public void UpdateText(PlayerDatabase player)
        {
            string data = "";

            // ボーナス確率
            data += "ボーナス確率 Bonus probability: " + "\n" + "\n";
            data += "ビッグチャンス確率 BIG CHANCE: ";
            if (player.BigTimes > 0)
            {
                float bigProbability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + bigProbability.ToString("F3") + "\n";
            }
            else
            {
                data += "1/---" + "\n";
            }

            data += "ボーナスゲーム確率 BONUS GAME: ";
            if (player.RegTimes > 0)
            {
                float regprobability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + regprobability.ToString("F3") + "\n" + "\n";
            }
            else
            {
                data += "1/---" + "\n" + "\n";
            }

            // 小役確率
            data += "ベル Bell: " + "\n";
            data += " 確率 Probability: ";

            if (player.PlayerAnalyticsData.NormalBellHitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalBellHitCount;
                data += "1/" + probability.ToString("F3") + "<space=5em>";
            }
            else
            {
                data += "1/---" + "<space=5em>";
            }

            data += " 成立 Hit: " + player.PlayerAnalyticsData.NormalBellHitCount + "<space=5em>";
            data += " 入賞 Line Up: " + player.PlayerAnalyticsData.NormalBellLineUpCount + "\n";


            data += "スイカ Melon: " + "\n";
            data += " 確率 Probability: ";

            if (player.PlayerAnalyticsData.NormalMelonHitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalMelonHitCount;
                data += "1/" + probability.ToString("F3") + "<space=5em>";
            }
            else
            {
                data += "1/---" + "<space=5em>";
            }

            data += " 成立 Hit: " + player.PlayerAnalyticsData.NormalMelonHitCount + "<space=5em>";
            data += " 入賞 Line Up: " + player.PlayerAnalyticsData.NormalMelonLineUpCount + "\n";


            data += "2枚チェリー Cherry2: " + "\n";
            data += " 確率 Probability: ";

            if (player.PlayerAnalyticsData.NormalCherry2HitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry2HitCount;
                data += "1/" + probability.ToString("F3") + "<space=5em>";
            }
            else
            {
                data += "1/---" + "<space=5em>";
            }

            data += " 成立 Hit: " + player.PlayerAnalyticsData.NormalCherry2HitCount + "<space=5em>";
            data += " 入賞 Line Up: " + player.PlayerAnalyticsData.NormalCherry2LineUpCount + "\n";


            data += "4枚チェリー Cherry4: " + "\n";
            data += " 確率 Probability: ";

            if (player.PlayerAnalyticsData.NormalCherry4HitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry4HitCount;
                data += "1/" + probability.ToString("F3") + "<space=5em>";
            }
            else
            {
                data += "1/---" + "<space=5em>";
            }

            data += " 成立 Hit: " + player.PlayerAnalyticsData.NormalCherry4HitCount + "<space=5em>";
            data += " 入賞 Line Up: " + player.PlayerAnalyticsData.NormalCherry4LineUpCount + "\n";

            textUI.text = data;
        }
    }
}

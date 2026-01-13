using ReelSpinGame_System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // 確率などの情報
    public class ProbabilityDataUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI normalDataTextUI;      // 通常時
        [SerializeField] TextMeshProUGUI bigDataTextUI;         // BIG時

        public void UpdateText(PlayerDatabase player)
        {
            DisplayNormalMode(player);
            DisplayBigChance(player);
        }

        // 通常時表示
        private void DisplayNormalMode(PlayerDatabase player)
        {
            float probability;
            string data = "\n\n";

            // 小役確率
            // ベル
            // 確率
            if (player.PlayerAnalyticsData.NormalBellHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalBellHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.NormalBellHitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.NormalBellLineUpCount + "\n\n\n";

            // スイカ
            // 確率
            if (player.PlayerAnalyticsData.NormalMelonHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalMelonHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.NormalMelonHitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.NormalMelonLineUpCount + "\n\n\n";

            // 2枚チェリー
            // 確率
            if (player.PlayerAnalyticsData.NormalCherry2HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry2HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.NormalCherry2HitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.NormalCherry2LineUpCount + "\n\n\n";

            // 4枚チェリー
            // 確率
            if (player.PlayerAnalyticsData.NormalCherry4HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry4HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.NormalCherry4HitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.NormalCherry4LineUpCount + "\n\n\n";

            normalDataTextUI.text = data;
        }

        // ビッグチャンス時表示
        private void DisplayBigChance(PlayerDatabase player)
        {
            float probability;
            string data = "\n\n";

            // 小役確率
            // ベル
            // 確率
            if (player.PlayerAnalyticsData.BigBellHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigBellHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.BigBellHitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.BigBellLineUpCount + "\n\n\n";

            // スイカ
            // 確率
            if (player.PlayerAnalyticsData.BigMelonHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigMelonHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.BigMelonHitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.BigMelonLineUpCount + "\n\n\n";

            // 2枚チェリー
            // 確率
            if (player.PlayerAnalyticsData.BigCherry2HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigCherry2HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.BigCherry2HitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.BigCherry2LineUpCount + "\n\n\n";

            // 4枚チェリー
            // 確率
            if (player.PlayerAnalyticsData.BigCherry4HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigCherry4HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // 成立回数
            data += player.PlayerAnalyticsData.BigCherry4HitCount + "\n";
            // 入賞回数
            data += player.PlayerAnalyticsData.BigCherry4LineUpCount + "\n\n\n";

            bigDataTextUI.text = data;
        }
    }
}

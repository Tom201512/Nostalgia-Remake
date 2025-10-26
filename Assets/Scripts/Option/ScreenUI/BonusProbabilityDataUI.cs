using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class BonusProbabilityDataUI : MonoBehaviour
    {
        // ボーナス中の確率情報

        // 情報画面
        [SerializeField] TextMeshProUGUI textUI;

        public void UpdateText(PlayerDatabase player)
        {
            float probability = 0.0f;
            float payoutRate = 0.0f;
            string data = "";

            // 小役確率(ビッグチャンス中)
            data += "ベル Bell: " + "\n";
            data += " 確率 Probability:";

            if (player.PlayerAnalyticsData.BigBellHitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigBellHitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ');
            }
            else
            {
                data += "1/---   ";
            }

            data += "  成立 Hit: " + player.PlayerAnalyticsData.BigBellHitCount;
            data += "  入賞 Line Up: " + player.PlayerAnalyticsData.BigBellLineUpCount;


            data += "スイカ Melon: " + "\n";
            data += "確率 Probability: ";

            if (player.PlayerAnalyticsData.BigMelonHitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigMelonHitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "成立 Hit: " + player.PlayerAnalyticsData.BigMelonHitCount + "\t";
            data += "入賞 Line Up: " + player.PlayerAnalyticsData.BigMelonLineUpCount + "\n";


            data += "2枚チェリー Cherry2:" + "\n";
            data += "確率 Probability: ";

            if (player.PlayerAnalyticsData.BigCherry2HitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigCherry2HitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "成立 Hit: " + player.PlayerAnalyticsData.BigCherry2HitCount + "\t";
            data += "入賞 Line Up: " + player.PlayerAnalyticsData.BigCherry2LineUpCount + "\n";


            data += "4枚チェリー Cherry4: " + "\n";
            data += "確率 Probability: ";

            if (player.PlayerAnalyticsData.BigCherry4HitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigCherry4HitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "成立 Hit: " + player.PlayerAnalyticsData.BigCherry4HitCount + "\t";
            data += "入賞 Line Up: " + player.PlayerAnalyticsData.BigCherry4LineUpCount + "\n" + "\n";

            // JAC-AVOID

            // 成功回数
            data += "JACハズシ成功 JAC-Avoid Success:" + player.PlayerAnalyticsData.BigJacAvoidTimes + "\n";
            data += "ビタハズシ Perfect Avoid: " + player.PlayerAnalyticsData.BigJacPerfectAvoidTimes + "\n";
            data += "アシスト付きハズシ Assisted Avoid: " + player.PlayerAnalyticsData.BigJacAssistedAvoidTimes + "\n";

            // 精度
            data += "ビタハズシ精度 Perfect avoid accuracy: ";
            if (player.PlayerAnalyticsData.BigJacPerfectAvoidTimes > 0)
            {
                payoutRate = (float)player.PlayerAnalyticsData.BigJacAvoidTimes / player.PlayerAnalyticsData.BigJacPerfectAvoidTimes * 100;
                data += probability.ToString("F2").PadLeft(6, ' ') + "%" + "\n" + "\n";
            }
            else
            {
                data += "000.00%" + "\n" + "\n";
            }

            // JAC中はずれ

            data += "JAC中はずれ JAC none:\t";

            if (player.PlayerAnalyticsData.JacGameNoneTimes > 0)
            {
                probability = (float)player.PlayerAnalyticsData.JacGamesCount / player.PlayerAnalyticsData.JacGameNoneTimes;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ');
            }
            else
            {
                data += "1/---";
            }

            textUI.text = data;
        }
     }
}

using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotMainDataUI : MonoBehaviour
    {
        // スロット基本情報画面のスクリプト

        // データ部分1
        [SerializeField] TextMeshProUGUI dataTextUI;
        [SerializeField] TextMeshProUGUI dataTextUI2;

        public void UpdateText(PlayerDatabase player)
        {
            DisplayGamesAndBonus(player);
            DisplayMedal(player);
        }

        private void DisplayGamesAndBonus(PlayerDatabase player)
        {
            string data = "\n";
            float probability = 0.0f;

            //総ゲーム数
            data += player.PlayerAnalyticsData.TotalAllGamesCount.ToString() + "G\n";
            // 現在
            data += player.CurrentGames.ToString() + "G\n";
            // 通常時
            data += player.TotalGames.ToString() + "G\n";
            // BIG CHANCE
            data += player.PlayerAnalyticsData.BigGamesCount.ToString() + "G\n";
            // BONUS GAME
            data += player.PlayerAnalyticsData.JacGamesCount.ToString() + "G\n\n\n\n";

            // ビッグチャンス
            // 回数
            data += player.BigTimes.ToString() + "\n";

            // 確率
            if (player.BigTimes > 0)
            {
                probability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + probability.ToString("F2") + "\n\n\n";
            }
            else
            {
                data += "1/---.--" + "\n\n\n";
            }

            // ボーナスゲーム回数
            // 回数
            data += player.RegTimes.ToString() + "\n";

            // 確率
            if (player.RegTimes > 0)
            {
                probability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + probability.ToString("F2") + "\n\n";
            }
            else
            {
                data += "1/---.--" + "\n\n";
            }

            // 総合算
            // 両方当たっている場合は総合算を求める。
            // 片方当たっている場合は当たっている方の確率のみ表示
            if (player.BigTimes > 0 && player.RegTimes > 0)
            {
                float bigProb = (float)player.TotalGames / player.BigTimes;
                float regProb = (float)player.TotalGames / player.RegTimes;

                probability = bigProb * regProb / (bigProb + regProb);
                data += "1/" + probability.ToString("F2");
            }
            else if (player.BigTimes > 0)
            {
                probability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + probability.ToString("F2");
            }
            else if (player.RegTimes > 0)
            {
                probability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + probability.ToString("F2");
            }
            else
            {
                data += "1/---.--";
            }

            dataTextUI.text = data;
        }

        private void DisplayMedal(PlayerDatabase player)
        {
            string data = "\n";

            // メダル枚数
            // 枚数
            data += player.PlayerMedalData.CurrentPlayerMedal.ToString().PadLeft(7, ' ') + "\n";
            // 投入
            data += player.PlayerMedalData.CurrentInMedal.ToString().PadLeft(7, ' ') + "\n";
            // 払出
            data += player.PlayerMedalData.CurrentOutMedal.ToString().PadLeft(7, ' ') + "\n";
            // 差枚
            data += (player.PlayerMedalData.CurrentOutMedal - player.PlayerMedalData.CurrentInMedal).ToString().PadLeft(7, ' ') + "\n";
            // 機械割
            if (player.PlayerMedalData.CurrentInMedal > 0 && player.PlayerMedalData.CurrentOutMedal > 0)
            {
                float payoutRate = (float)player.PlayerMedalData.CurrentOutMedal / player.PlayerMedalData.CurrentInMedal * 100;
                data += payoutRate.ToString("F2") + "%\n";
            }
            else
            {
                data += "000.00%";
            }

            dataTextUI2.text = data;
        }
    }
}

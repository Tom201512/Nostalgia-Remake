using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotMainDataUI : MonoBehaviour
    {
        // スロット基本情報画面のスクリプト

        // 情報画面
        [SerializeField] TextMeshProUGUI textUI;
        // データ部分
        [SerializeField] TextMeshProUGUI dataTextUI;

        public void UpdateText(PlayerDatabase player)
        {
            DisplayInfoText();
            DisplayDataText(player);
        }

        private void DisplayInfoText()
        {
            string data = "";
            // ゲーム数など
            data += "ゲーム数 Games\n";
            data += "\t\t総ゲーム数 Total:\n";
            data += "\t\t現在 Current:\n";
            data += "\t\t通常時 Normal:\n";
            data += "\t\tビッグチャンス中 BIG CHANCE:\n";
            data += "\t\tボーナスゲーム中 BONUS GAME:\n\n";

            // ボーナス回数
            data += "ボーナス回数 Bonus:\n";
            data += "\t\tビッグチャンス BIG CHANCE:\n";
            data += "\t\tボーナスゲーム BONUS GAME:\n\n";

            // メダル枚数
            data += "メダル Medal:\n";
            data += "\t\t枚数 Current:\n";
            data += "\t\t投入 IN:\n";
            data += "\t\t払出 OUT:\n";
            data += "\t\t差枚 Difference\n";
            data += "\t\t機械割 PayoutRate:";

            textUI.text = data;
        }

        private void DisplayDataText(PlayerDatabase player)
        {
            string data = "";
            //総ゲーム数
            data += player.PlayerAnalyticsData.TotalAllGamesCount.ToString().PadLeft(7, ' ') + "G\n";
            // 現在
            data += player.CurrentGames.ToString().PadLeft(7, ' ') + "G\n";
            // 通常時
            data += player.TotalGames.ToString().PadLeft(7, ' ') + "G\n";
            // BIG CHANCE
            data += player.PlayerAnalyticsData.BigGamesCount.ToString().PadLeft(7, ' ') + "G\n";
            // BONUS GAME
            data += player.PlayerAnalyticsData.JacGamesCount.ToString().PadLeft(7, ' ') + "G\n\n\n";

            // BIG回数
            data += player.BigTimes.ToString().PadLeft(7, ' ') + "\n";
            // REG回数
            data += player.RegTimes.ToString().PadLeft(7, ' ') + "\n\n\n";


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
                data += payoutRate.ToString("F2").PadLeft(5, ' ') + "%";
            }
            else
            {
                data += "000.00%";
            }

            dataTextUI.text = data;
        }
    }
}

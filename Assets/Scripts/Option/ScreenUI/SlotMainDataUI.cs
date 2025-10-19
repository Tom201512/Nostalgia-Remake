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

        public void UpdateText(PlayerDatabase player)
        {
            string data = "";

            // ゲーム数など
            data += "ゲーム数 Games:" + "\n";
            data += "総回転数 TotalSpin: " + player.TotalGames + "\n";
            data += "現在回転数 CurrentSpin: " + player.CurrentGames + "\n";
            data += "ビッグチャンス回数 BIG CHANCE: " + player.BigTimes + "\n";
            data += "ボーナスゲーム回数 BONUS GAME: " + player.RegTimes + "\n";
            data += "BIG CHANCE中回転数 SpinTimes in BIG CHANCE: " + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "BONUG GAME中回転数 SpinTimes in BONUS GAME: " + player.PlayerAnalyticsData.JacGamesCount + "\n";
            data += "全ゲーム数 TotalPlayedGames: " + player.PlayerAnalyticsData.TotalAllGamesCount + "\n" + "\n";

            // メダル枚数
            data += "メダル Medal: " + "\n";
            data += "メダル枚数 CurrentMedal: " + player.PlayerMedalData.CurrentPlayerMedal + "\n";
            data += "投入枚数 CurrentIN: " + player.PlayerMedalData.CurrentInMedal + "\n";
            data += "払出枚数 CurrentOUT: " + player.PlayerMedalData.CurrentOutMedal + "\n";
            data += "差枚数 IN/OUT: " + (player.PlayerMedalData.CurrentOutMedal - player.PlayerMedalData.CurrentInMedal) + "\n";
            data += "機械割 PayoutRate: ";

            // 機械割
            if (player.PlayerMedalData.CurrentInMedal > 0 && player.PlayerMedalData.CurrentOutMedal > 0)
            {
                float payoutRate = (float)player.PlayerMedalData.CurrentOutMedal / player.PlayerMedalData.CurrentInMedal * 100;
                data += payoutRate.ToString("F2") + "%";
            }
            else
            {
                data += "000.00%";
            }


            textUI.text = data;
        }
    }
}

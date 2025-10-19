using ReelSpinGame_System;
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
            data += "総回転数 TotalSpin:" + player.TotalGames + "\n";
            data += "現在回転数 CurrentSpin:" + player.CurrentGames + "\n";
            data += "ビッグチャンス回数 BIG CHANCE:" + player.BigTimes + "\n";
            data += "ボーナスゲーム回数 BIG CHANCE:" + player.RegTimes + "\n";
            data += "BIG CHANCE中回転数 SpinTimesIn BIG CHANCE:" + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "BONUG GAME中回転数 SpinTimesIn BONUS GAME:" + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "全ゲーム数 TotalPlayedGames:" + player.PlayerAnalyticsData.TotalAllGamesCount + "\n" + "\n";

            textUI.text = data;
        }
    }
}

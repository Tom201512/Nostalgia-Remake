using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Games
{
    // ゲーム数表示用UI
    public class GamesInfoUI : MonoBehaviour
    {
        const int MaximumGameCount = 99999;        // 最大表示可能ゲーム数

        [SerializeField] private TextMeshProUGUI games;     // ゲーム数テキスト
        [SerializeField] private TextMeshProUGUI total;     // 総ゲーム数テキスト

        // ゲーム数UI更新
        public void UpdateGamesUI(PlayerDatabase playerDatabase)
        {
            this.games.text = "Games:" + "\n" + Math.Clamp(playerDatabase.CurrentGames, 0, MaximumGameCount).ToString() + "G";
            this.total.text = "Total:" + "\n" + Math.Clamp(playerDatabase.TotalGames, 0, MaximumGameCount).ToString() + "G";
        }
    }
}


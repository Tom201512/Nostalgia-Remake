using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Games
{
    // ゲーム数表示用UI
    public class GamesInfoUI : MonoBehaviour
    {
        // const
        // 最大表示可能ゲーム数
        private const int MaximumGameCounts = 99999;

        // var
        [SerializeField] private TextMeshProUGUI games;
        [SerializeField] private TextMeshProUGUI total;

        // ゲーム数UI更新
        public void UpdateGamesUI(PlayerDatabase playerDatabase)
        {
            this.games.text = "Games:" + "\n" + Math.Clamp(playerDatabase.CurrentGames, 0, MaximumGameCounts).ToString() + "G";
            this.total.text = "Total:" + "\n" + Math.Clamp(playerDatabase.TotalGames, 0, MaximumGameCounts).ToString() + "G";
        }
    }
}


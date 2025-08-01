using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Games
{
    // �Q�[�����\���pUI
    public class GamesInfoUI : MonoBehaviour
    {
        // const
        // �ő�\���\�Q�[����
        private const int MaximumGameCounts = 99999;

        // var
        [SerializeField] private TextMeshProUGUI games;
        [SerializeField] private TextMeshProUGUI total;

        // �Q�[����UI�X�V
        public void UpdateGamesUI(PlayerDatabase playerDatabase)
        {
            this.games.text = "Games:" + "\n" + Math.Clamp(playerDatabase.CurrentGames, 0, MaximumGameCounts).ToString() + "G";
            this.total.text = "Total:" + "\n" + Math.Clamp(playerDatabase.TotalGames, 0, MaximumGameCounts).ToString() + "G";
        }
    }
}


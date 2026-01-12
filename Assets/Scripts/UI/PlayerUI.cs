using ReelSpinGame_Medal;
using ReelSpinGame_System;
using ReelSpinGame_UI.Player.Bonus;
using ReelSpinGame_UI.Player.Games;
using ReelSpinGame_UI.Player.Medal;
using UnityEngine;

namespace ReelSpinGame_UI.Player
{
    // プレイヤーのUI
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private GamesInfoUI gamesInfoUI;        // ゲーム数表示用
        [SerializeField] private MedalInfoUI medalInfoUI;        // メダル枚数表示用 
        [SerializeField] private BonusInfoUI bonusInfoUI;        // ボーナス当選回数表示用

        // UI更新
        public void UpdatePlayerUI(PlayerDatabase playerDatabase, MedalManager medalManager)
        {
            gamesInfoUI.UpdateGamesUI(playerDatabase);
            medalInfoUI.UpdateMedalUI(playerDatabase.PlayerMedalData, medalManager);
            bonusInfoUI.UpdateBonusUI(playerDatabase);
        }
    }
}


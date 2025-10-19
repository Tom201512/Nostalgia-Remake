using ReelSpinGame_System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotMainDataUI : MonoBehaviour
    {
        // �X���b�g��{����ʂ̃X�N���v�g

        // �����
        [SerializeField] TextMeshProUGUI textUI;

        public void UpdateText(PlayerDatabase player)
        {
            string data = "";

            // �Q�[�����Ȃ�
            data += "����]�� TotalSpin:" + player.TotalGames + "\n";
            data += "���݉�]�� CurrentSpin:" + player.CurrentGames + "\n";
            data += "�r�b�O�`�����X�� BIG CHANCE:" + player.BigTimes + "\n";
            data += "�{�[�i�X�Q�[���� BIG CHANCE:" + player.RegTimes + "\n";
            data += "BIG CHANCE����]�� SpinTimesIn BIG CHANCE:" + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "BONUG GAME����]�� SpinTimesIn BONUS GAME:" + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "�S�Q�[���� TotalPlayedGames:" + player.PlayerAnalyticsData.TotalAllGamesCount + "\n" + "\n";

            textUI.text = data;
        }
    }
}

using ReelSpinGame_System;
using System;
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
            data += "�Q�[���� Games:" + "\n";
            data += "����]�� TotalSpin: " + player.TotalGames + "\n";
            data += "���݉�]�� CurrentSpin: " + player.CurrentGames + "\n";
            data += "�r�b�O�`�����X�� BIG CHANCE: " + player.BigTimes + "\n";
            data += "�{�[�i�X�Q�[���� BONUS GAME: " + player.RegTimes + "\n";
            data += "BIG CHANCE����]�� SpinTimes in BIG CHANCE: " + player.PlayerAnalyticsData.BigGamesCount + "\n";
            data += "BONUG GAME����]�� SpinTimes in BONUS GAME: " + player.PlayerAnalyticsData.JacGamesCount + "\n";
            data += "�S�Q�[���� TotalPlayedGames: " + player.PlayerAnalyticsData.TotalAllGamesCount + "\n" + "\n";

            // ���_������
            data += "���_�� Medal: " + "\n";
            data += "���_������ CurrentMedal: " + player.PlayerMedalData.CurrentPlayerMedal + "\n";
            data += "�������� CurrentIN: " + player.PlayerMedalData.CurrentInMedal + "\n";
            data += "���o���� CurrentOUT: " + player.PlayerMedalData.CurrentOutMedal + "\n";
            data += "������ IN/OUT: " + (player.PlayerMedalData.CurrentOutMedal - player.PlayerMedalData.CurrentInMedal) + "\n";
            data += "�@�B�� PayoutRate: ";

            // �@�B��
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

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
        // �f�[�^����
        [SerializeField] TextMeshProUGUI dataTextUI;

        public void UpdateText(PlayerDatabase player)
        {
            DisplayInfoText();
            DisplayDataText(player);
        }

        private void DisplayInfoText()
        {
            string data = "";
            // �Q�[�����Ȃ�
            data += "�Q�[���� Games\n";
            data += "\t\t���Q�[���� Total:\n";
            data += "\t\t���� Current:\n";
            data += "\t\t�ʏ펞 Normal:\n";
            data += "\t\t�r�b�O�`�����X�� BIG CHANCE:\n";
            data += "\t\t�{�[�i�X�Q�[���� BONUS GAME:\n\n";

            // �{�[�i�X��
            data += "�{�[�i�X�� Bonus:\n";
            data += "\t\t�r�b�O�`�����X BIG CHANCE:\n";
            data += "\t\t�{�[�i�X�Q�[�� BONUS GAME:\n\n";

            // ���_������
            data += "���_�� Medal:\n";
            data += "\t\t���� Current:\n";
            data += "\t\t���� IN:\n";
            data += "\t\t���o OUT:\n";
            data += "\t\t���� Difference\n";
            data += "\t\t�@�B�� PayoutRate:";

            textUI.text = data;
        }

        private void DisplayDataText(PlayerDatabase player)
        {
            string data = "";
            //���Q�[����
            data += player.PlayerAnalyticsData.TotalAllGamesCount.ToString().PadLeft(7, ' ') + "G\n";
            // ����
            data += player.CurrentGames.ToString().PadLeft(7, ' ') + "G\n";
            // �ʏ펞
            data += player.TotalGames.ToString().PadLeft(7, ' ') + "G\n";
            // BIG CHANCE
            data += player.PlayerAnalyticsData.BigGamesCount.ToString().PadLeft(7, ' ') + "G\n";
            // BONUS GAME
            data += player.PlayerAnalyticsData.JacGamesCount.ToString().PadLeft(7, ' ') + "G\n\n\n";

            // BIG��
            data += player.BigTimes.ToString().PadLeft(7, ' ') + "\n";
            // REG��
            data += player.RegTimes.ToString().PadLeft(7, ' ') + "\n\n\n";


            // ���_������
            // ����
            data += player.PlayerMedalData.CurrentPlayerMedal.ToString().PadLeft(7, ' ') + "\n";
            // ����
            data += player.PlayerMedalData.CurrentInMedal.ToString().PadLeft(7, ' ') + "\n";
            // ���o
            data += player.PlayerMedalData.CurrentOutMedal.ToString().PadLeft(7, ' ') + "\n";
            // ����
            data += (player.PlayerMedalData.CurrentOutMedal - player.PlayerMedalData.CurrentInMedal).ToString().PadLeft(7, ' ') + "\n";
            // �@�B��
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

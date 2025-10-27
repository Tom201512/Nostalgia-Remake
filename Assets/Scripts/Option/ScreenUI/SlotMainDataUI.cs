using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotMainDataUI : MonoBehaviour
    {
        // �X���b�g��{����ʂ̃X�N���v�g

        // �f�[�^����
        [SerializeField] TextMeshProUGUI dataTextUI;

        public void UpdateText(PlayerDatabase player)
        {
            string data = "\n";
            float probability = 0.0f;

            //���Q�[����
            data += player.PlayerAnalyticsData.TotalAllGamesCount.ToString() + "G\n";
            // ����
            data += player.CurrentGames.ToString()+ "G\n";
            // �ʏ펞
            data += player.TotalGames.ToString() + "G\n";
            // BIG CHANCE
            data += player.PlayerAnalyticsData.BigGamesCount.ToString()+ "G\n";
            // BONUS GAME
            data += player.PlayerAnalyticsData.JacGamesCount.ToString() + "G\n\n\n";

            // �r�b�O�`�����X
            // ��
            data += player.BigTimes.ToString() + "\n";

            if (player.BigTimes > 0)
            {
                probability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + probability.ToString("F2") + "\n\n\n";
            }
            else
            {
                data += "1/---.--" + "\n\n\n";
            }

            // �{�[�i�X�Q�[����
            // ��
            data += player.RegTimes.ToString() + "\n";

            if (player.RegTimes > 0)
            {
                float regprobability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + regprobability.ToString("F2") + "\n" + "\n";
            }
            else
            {
                data += "1/---.--" + "\n\n\n";
            }



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

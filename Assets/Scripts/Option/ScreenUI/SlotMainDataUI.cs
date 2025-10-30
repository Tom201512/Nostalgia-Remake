using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlotMainDataUI : MonoBehaviour
    {
        // �X���b�g��{����ʂ̃X�N���v�g

        // �f�[�^����1
        [SerializeField] TextMeshProUGUI dataTextUI;
        [SerializeField] TextMeshProUGUI dataTextUI2;

        public void UpdateText(PlayerDatabase player)
        {
            DisplayGamesAndBonus(player);
            DisplayMedal(player);
        }

        private void DisplayGamesAndBonus(PlayerDatabase player)
        {
            string data = "\n";
            float probability = 0.0f;

            //���Q�[����
            data += player.PlayerAnalyticsData.TotalAllGamesCount.ToString() + "G\n";
            // ����
            data += player.CurrentGames.ToString() + "G\n";
            // �ʏ펞
            data += player.TotalGames.ToString() + "G\n";
            // BIG CHANCE
            data += player.PlayerAnalyticsData.BigGamesCount.ToString() + "G\n";
            // BONUS GAME
            data += player.PlayerAnalyticsData.JacGamesCount.ToString() + "G\n\n\n\n";

            // �r�b�O�`�����X
            // ��
            data += player.BigTimes.ToString() + "\n";

            // �m��
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

            // �m��
            if (player.RegTimes > 0)
            {
                probability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + probability.ToString("F2") + "\n\n";
            }
            else
            {
                data += "1/---.--" + "\n\n";
            }

            // �����Z
            // �����������Ă���ꍇ�͑����Z�����߂�B
            // �Е��������Ă���ꍇ�͓������Ă�����̊m���̂ݕ\��
            if (player.BigTimes > 0 && player.RegTimes > 0)
            {
                float bigProb = (float)player.TotalGames / player.BigTimes;
                float regProb = (float)player.TotalGames / player.RegTimes;

                probability = bigProb * regProb / (bigProb + regProb);
                data += "1/" + probability.ToString("F2");
            }
            else if (player.BigTimes > 0)
            {
                probability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + probability.ToString("F2");
            }
            else if (player.RegTimes > 0)
            {
                probability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + probability.ToString("F2");
            }
            else
            {
                data += "1/---.--";
            }

            dataTextUI.text = data;
        }

        private void DisplayMedal(PlayerDatabase player)
        {
            string data = "\n";

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
                data += payoutRate.ToString("F2") + "%\n";
            }
            else
            {
                data += "000.00%";
            }

            dataTextUI2.text = data;
        }
    }
}

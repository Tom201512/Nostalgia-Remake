using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class ProbabilityDataUI : MonoBehaviour
    {
        // �m���Ȃǂ̏��

        // �f�[�^����
        // �ʏ펞
        [SerializeField] TextMeshProUGUI normalDataTextUI;
        // BIG��
        [SerializeField] TextMeshProUGUI bigDataTextUI;

        public void UpdateText(PlayerDatabase player)
        {
            DisplayNormalMode(player);
            DisplayBigChance(player);
        }

        // �ʏ펞�\��
        private void DisplayNormalMode(PlayerDatabase player)
        {
            float probability = 0.0f;
            string data = "\n\n";

            // �����m��
            // �x��
            // �m��
            if (player.PlayerAnalyticsData.NormalBellHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalBellHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.NormalBellHitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.NormalBellLineUpCount + "\n\n\n";

            // �X�C�J
            // �m��
            if (player.PlayerAnalyticsData.NormalMelonHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalMelonHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.NormalMelonHitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.NormalMelonLineUpCount + "\n\n\n";

            // 2���`�F���[
            // �m��
            if (player.PlayerAnalyticsData.NormalCherry2HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry2HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.NormalCherry2HitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.NormalCherry2LineUpCount + "\n\n\n";

            // 4���`�F���[
            // �m��
            if (player.PlayerAnalyticsData.NormalCherry4HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry4HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.NormalCherry4HitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.NormalCherry4LineUpCount + "\n\n\n";

            normalDataTextUI.text = data;
        }

        // �r�b�O�`�����X���\��
        private void DisplayBigChance(PlayerDatabase player)
        {
            float probability = 0.0f;
            string data = "\n\n";

            // �����m��
            // �x��
            // �m��
            if (player.PlayerAnalyticsData.BigBellHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigBellHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.BigBellHitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.BigBellLineUpCount + "\n\n\n";

            // �X�C�J
            // �m��
            if (player.PlayerAnalyticsData.BigMelonHitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigMelonHitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.BigMelonHitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.BigMelonLineUpCount + "\n\n\n";

            // 2���`�F���[
            // �m��
            if (player.PlayerAnalyticsData.BigCherry2HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigCherry2HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.BigCherry2HitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.BigCherry2LineUpCount + "\n\n\n";

            // 4���`�F���[
            // �m��
            if (player.PlayerAnalyticsData.BigCherry4HitCount > 0)
            {
                probability = (float)player.TotalGames / player.PlayerAnalyticsData.BigCherry4HitCount;
                data += "1/" + probability.ToString("F2") + "\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // ������
            data += player.PlayerAnalyticsData.BigCherry4HitCount + "\n";
            // ���܉�
            data += player.PlayerAnalyticsData.BigCherry4LineUpCount + "\n\n\n";

            bigDataTextUI.text = data;
        }
    }
}

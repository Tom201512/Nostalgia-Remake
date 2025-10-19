using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class ProbabilityDataUI : MonoBehaviour
    {
        // �X���b�g��{����ʂ̃X�N���v�g

        // �����
        [SerializeField] TextMeshProUGUI textUI;

        public void UpdateText(PlayerDatabase player)
        {
            string data = "";

            // �{�[�i�X�m��
            data += "�{�[�i�X�m�� Bonus probability: " + "\n" + "\n";
            data += "�r�b�O�`�����X�m�� BIG CHANCE: ";
            if (player.BigTimes > 0)
            {
                float bigProbability = (float)player.TotalGames / player.BigTimes;
                data += "1/" + bigProbability.ToString("F3") + "\n";
            }
            else
            {
                data += "1/---\n";
            }

            data += "�{�[�i�X�Q�[���m�� BONUS GAME: ";
            if (player.RegTimes > 0)
            {
                float regprobability = (float)player.TotalGames / player.RegTimes;
                data += "1/" + regprobability.ToString("F3") + "\n" + "\n";
            }
            else
            {
                data += "1/---\n" + "\n";
            }

            // �����m��
            data += "�ʏ펞�����m�� Normal mode probability:" + "\n" + "\n";
            data += "�x�� Bell: ";
            data += " �m�� Probability: ";

            if (player.PlayerAnalyticsData.NormalBellHitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalBellHitCount;
                data += "1/" + probability.ToString("F3");
            }
            else
            {
                data += "1/---";
            }

            data += " ���� Hit: " + player.PlayerAnalyticsData.NormalBellHitCount;
            data += " ���� Line Up: " + player.PlayerAnalyticsData.NormalBellLineUpCount + "\n";


            data += "�X�C�J Melon: ";
            data += " �m�� Probability: ";

            if (player.PlayerAnalyticsData.NormalMelonHitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalMelonHitCount;
                data += "1/" + probability.ToString("F3");
            }
            else
            {
                data += "1/---";
            }

            data += " ���� Hit: " + player.PlayerAnalyticsData.NormalMelonHitCount;
            data += " ���� Line Up: " + player.PlayerAnalyticsData.NormalMelonLineUpCount + "\n";


            data += "2���`�F���[ Cherry2: ";
            data += " �m�� Probability: ";

            if (player.PlayerAnalyticsData.NormalCherry2HitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry2HitCount;
                data += "1/" + probability.ToString("F3");
            }
            else
            {
                data += "1/---";
            }

            data += " ���� Hit: " + player.PlayerAnalyticsData.NormalCherry2HitCount;
            data += " ���� Line Up: " + player.PlayerAnalyticsData.NormalCherry2LineUpCount + "\n";


            data += "4���`�F���[ Cherry4: ";
            data += " �m�� Probability: ";

            if (player.PlayerAnalyticsData.NormalCherry4HitCount > 0)
            {
                float probability = (float)player.TotalGames / player.PlayerAnalyticsData.NormalCherry4HitCount;
                data += "1/" + probability.ToString("F3");
            }
            else
            {
                data += "1/---";
            }

            data += " ���� Hit: " + player.PlayerAnalyticsData.NormalCherry4HitCount;
            data += " ���� Line Up: " + player.PlayerAnalyticsData.NormalCherry4LineUpCount + "\n";

            textUI.text = data;
        }
    }
}

using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    public class BonusProbabilityDataUI : MonoBehaviour
    {
        // �{�[�i�X���̊m�����

        // �����
        [SerializeField] TextMeshProUGUI textUI;

        public void UpdateText(PlayerDatabase player)
        {
            float probability = 0.0f;
            float payoutRate = 0.0f;
            string data = "";

            // �����m��(�r�b�O�`�����X��)
            data += "�x�� Bell: " + "\n";
            data += " �m�� Probability:";

            if (player.PlayerAnalyticsData.BigBellHitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigBellHitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ');
            }
            else
            {
                data += "1/---   ";
            }

            data += "  ���� Hit: " + player.PlayerAnalyticsData.BigBellHitCount;
            data += "  ���� Line Up: " + player.PlayerAnalyticsData.BigBellLineUpCount;


            data += "�X�C�J Melon: " + "\n";
            data += "�m�� Probability: ";

            if (player.PlayerAnalyticsData.BigMelonHitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigMelonHitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "���� Hit: " + player.PlayerAnalyticsData.BigMelonHitCount + "\t";
            data += "���� Line Up: " + player.PlayerAnalyticsData.BigMelonLineUpCount + "\n";


            data += "2���`�F���[ Cherry2:" + "\n";
            data += "�m�� Probability: ";

            if (player.PlayerAnalyticsData.BigCherry2HitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigCherry2HitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "���� Hit: " + player.PlayerAnalyticsData.BigCherry2HitCount + "\t";
            data += "���� Line Up: " + player.PlayerAnalyticsData.BigCherry2LineUpCount + "\n";


            data += "4���`�F���[ Cherry4: " + "\n";
            data += "�m�� Probability: ";

            if (player.PlayerAnalyticsData.BigCherry4HitCount > 0)
            {
                probability = (float)player.PlayerAnalyticsData.BigGamesCount / player.PlayerAnalyticsData.BigCherry4HitCount;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ') + "\t";
            }
            else
            {
                data += "1/---" + "\t";
            }

            data += "���� Hit: " + player.PlayerAnalyticsData.BigCherry4HitCount + "\t";
            data += "���� Line Up: " + player.PlayerAnalyticsData.BigCherry4LineUpCount + "\n" + "\n";

            // JAC-AVOID

            // ������
            data += "JAC�n�Y�V���� JAC-Avoid Success:" + player.PlayerAnalyticsData.BigJacAvoidTimes + "\n";
            data += "�r�^�n�Y�V Perfect Avoid: " + player.PlayerAnalyticsData.BigJacPerfectAvoidTimes + "\n";
            data += "�A�V�X�g�t���n�Y�V Assisted Avoid: " + player.PlayerAnalyticsData.BigJacAssistedAvoidTimes + "\n";

            // ���x
            data += "�r�^�n�Y�V���x Perfect avoid accuracy: ";
            if (player.PlayerAnalyticsData.BigJacPerfectAvoidTimes > 0)
            {
                payoutRate = (float)player.PlayerAnalyticsData.BigJacAvoidTimes / player.PlayerAnalyticsData.BigJacPerfectAvoidTimes * 100;
                data += probability.ToString("F2").PadLeft(6, ' ') + "%" + "\n" + "\n";
            }
            else
            {
                data += "000.00%" + "\n" + "\n";
            }

            // JAC���͂���

            data += "JAC���͂��� JAC none:\t";

            if (player.PlayerAnalyticsData.JacGameNoneTimes > 0)
            {
                probability = (float)player.PlayerAnalyticsData.JacGamesCount / player.PlayerAnalyticsData.JacGameNoneTimes;
                data += "1/" + probability.ToString("F3").PadLeft(6, ' ');
            }
            else
            {
                data += "1/---";
            }

            textUI.text = data;
        }
     }
}

using ReelSpinGame_Reels;
using ReelSpinGame_System;
using ReelSpinGame_UI.Reel;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Option.MenuContent
{
    public class BonusDataUI : MonoBehaviour
    {
        // �{�[�i�X���̏��

        // �f�[�^����
        [SerializeField] TextMeshProUGUI dataTextUI;
        // ���[���f�B�X�v���C(���[�`�ڗp)
        [SerializeField] ReelDisplayUI reelDisplay;

        // ���[���I�u�W�F�N�g���Z�b�g����
        public void SetReelObject(List<ReelObjectPresenter> reelObjects)
        {
            //reelDisplay.SetReels(reelObjects);
        }

        public void UpdateText(PlayerDatabase player)
        {
            float probability = 0.0f;
            float rate = 0.0f;
            string data = "\n";

            // ���߃{�[�i�X�Q�[���̕\��(���������܂��Ă��Ȃ����͕̂\�����Ȃ�)
            // 2��ڈȍ~�̃{�[�i�X�͓��܎��Q�[�����L�^���Ă���Ε\��
            if(player.BonusHitRecord.Count > 1)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                // �����łȂ����2�O��\��
                else
                {
                    data += player.BonusHitRecord[^2].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^2].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^2].BonusPayout + "\n";
                }
            }
            // ����{�[�i�X�̏ꍇ�͓��܂��Ă��Ȃ���Ε\�����Ȃ�
            else if(player.BonusHitRecord.Count > 0)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    data += player.BonusHitRecord[^1].BonusHitGame + "\n";
                    data += player.BonusHitRecord[^1].BonusStartGame + "\n";
                    data += player.BonusHitRecord[^1].BonusPayout + "\n";
                }
                else
                {
                    data += "-------\n";
                    data += "-------\n";
                    data += "-------\n";
                }
            }
            // �񐬗��̏ꍇ�͕\�����Ȃ�
            else
            {
                data += "-------\n";
                data += "-------\n";
                data += "-------\n";
            }

            data += "\n\n";

            // JAC�n�Y�V
            // ������
            data += player.PlayerAnalyticsData.BigJacAvoidTimes + "\n";
            // �r�^�n�Y�V��
            data += player.PlayerAnalyticsData.BigJacPerfectAvoidTimes + "\n";
            // �A�V�X�g�n�Y�V��
            data += player.PlayerAnalyticsData.BigJacAssistedAvoidTimes + "\n";

            // �����m��
            if (player.PlayerAnalyticsData.BigJacAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacInTimes / player.PlayerAnalyticsData.BigJacAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            // �r�^�n�Y�V�����m��
            if (player.PlayerAnalyticsData.BigJacPerfectAvoidTimes > 0)
            {
                rate = (float)player.PlayerAnalyticsData.BigJacAvoidTimes / player.PlayerAnalyticsData.BigJacPerfectAvoidTimes * 100;
                data += rate.ToString("F2") + "%\n";
            }
            else
            {
                data += "1/---.--" + "\n";
            }

            data += "\n\n";


            // �{�[�i�X�Q�[�����n�Y��
            // ��
            data += player.PlayerAnalyticsData.JacGameNoneTimes + "\n";
            // �m��
            if (player.PlayerAnalyticsData.JacGameNoneTimes > 0)
            {
                probability = (float)player.PlayerAnalyticsData.JacGamesCount / player.PlayerAnalyticsData.JacGameNoneTimes;
                data += "1/" + probability.ToString("F2");
            }
            else
            {
                data += "1/---.--";
            }

            dataTextUI.text = data;
        }

        // ���I��o�ڂ�\������
        public void DisplayWinningPattern(PlayerDatabase player)
        {
            // �{�[�i�X������2�ȏ゠��ꍇ�͏����ɍ��킹�ĕ\��
            if(player.BonusHitRecord.Count > 1)
            {
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    reelDisplay.gameObject.SetActive(true);
                    // �{�[�i�X��2�ȏ゠��ꍇ�ł܂����I���Ă��Ȃ����2�ڂ�\��
                    reelDisplay.DisplayReels(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);
                }
                // �����łȂ����2�O��\��
                else
                {
                    reelDisplay.gameObject.SetActive(true);
                    // �{�[�i�X��2�ȏ゠��ꍇ�ł܂����I���Ă��Ȃ����2�ڂ�\��
                    reelDisplay.DisplayReels(player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^2].BonusReelPos[(int)ReelID.ReelRight]);
                }
            }
            // 1����ꍇ��
            else if(player.BonusHitRecord.Count > 0)
            {
                // ���܃Q�[�������Ȃ���Ε\�����Ȃ�
                if (player.BonusHitRecord[^1].BonusStartGame != 0)
                {
                    reelDisplay.DisplayReels(player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelLeft],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelMiddle],
                        player.BonusHitRecord[^1].BonusReelPos[(int)ReelID.ReelRight]);
                }
                else
                {
                    reelDisplay.gameObject.SetActive(false);
                }
            }
            else
            {
                reelDisplay.gameObject.SetActive(false);
            }
        }
    }
}

using ReelSpinGame_Datas;
using ReelSpinGame_Medal;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player.Medal
{
    // ���_�������\���p
    public class MedalInfoUI : MonoBehaviour
    {
        // const
        // �ő�\���\���_������
        private const int MaximumGameCount = 99999;

        // �ő�\���\�@�B��
        private const float MaximumPayoutRatio = 999.99f;

        // var
        [SerializeField] private TextMeshProUGUI medalUI;
        [SerializeField] private TextMeshProUGUI payoutRateUI;

        // ���_�����X�V
        public void UpdateMedalUI(PlayerMedalData playerMedalData, MedalManager medalManager)
        {
            // �莝�����_���AOUT�����͌��݂̎c�蕥���o�������ɍ��킹�Č��炷)

            medalUI.text = "Medal:" + "\n" + Math.Clamp(playerMedalData.CurrentPlayerMedal - medalManager.GetRemainingPayout()
                , 0, MaximumGameCount).ToString();

            // �@�B���v�Z

            float payoutRate = 0f;
            // �@�B��
            if (playerMedalData.CurrentInMedal > 0 && playerMedalData.CurrentOutMedal > 0)
            {
                payoutRate = (float)(playerMedalData.CurrentOutMedal - medalManager.GetRemainingPayout()) / playerMedalData.CurrentInMedal * 100;
            }

            payoutRateUI.text = "Payout:" + "\n" + Mathf.Clamp(payoutRate, 0f, MaximumPayoutRatio).ToString("F2") + "%";
        }
    }
}
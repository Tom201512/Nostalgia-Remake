using ReelSpinGame_Medal;
using ReelSpinGame_System;
using ReelSpinGame_UI.Player.Games;
using ReelSpinGame_UI.Player.Medal;
using ReelSpinGame_UI.Player.Bonus;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Player
{
    public class PlayerUI : UIBaseClass
    {
        // var

        // �Q�[�����\���p
        [SerializeField] private GamesInfoUI gamesInfoUI;
        // ���_�������\���p 
        [SerializeField] private MedalInfoUI medalInfoUI;
        // �{�[�i�X���I�񐔕\���p
        [SerializeField] private BonusInfoUI bonusInfoUI;

        //TextMeshProUGUI text;
        //PlayerDatabase playerDatabase;
        //MedalManager medalManager;

        // Start is called before the first frame update
        /*
        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }*/

        /*
        // Update is called once per frame
        void Update()
        {
            string buffer = "";

            // ���_���������鉉�o�p
            int playerMedal = playerDatabase.PlayerMedalData.CurrentPlayerMedal - medalManager.GetRemainingPayout();
            int outMedal = playerDatabase.PlayerMedalData.CurrentOutMedal - medalManager.GetRemainingPayout();

            buffer += "Player-" + "\n";
            buffer += "Total:" + playerDatabase.TotalGames + "\n";
            buffer += "Games:" + playerDatabase.CurrentGames + "\n" + "\n";
            buffer += "Medal:" + playerMedal + "\n";
            buffer += "IN:" + playerDatabase.PlayerMedalData.CurrentInMedal + "\n";
            buffer += "OUT:" + outMedal + "\n";

            // ������
            buffer += "Dif:" + (playerDatabase.PlayerMedalData.CurrentOutMedal - playerDatabase.PlayerMedalData.CurrentInMedal) + "\n";

            // �@�B��
            if (playerDatabase.PlayerMedalData.CurrentInMedal > 0 && playerDatabase.PlayerMedalData.CurrentOutMedal > 0)
            {
                float payoutRate = (float)playerDatabase.PlayerMedalData.CurrentOutMedal / playerDatabase.PlayerMedalData.CurrentInMedal * 100;
                buffer += "Payout:" + payoutRate.ToString("F2") + "%" + "\n" + "\n";
            }
            else
            {
                buffer += "Payout:" + "0.00%" + "\n" + "\n";
            }

            // �r�b�O�`�����X���I��
            buffer += "BIG:" + playerDatabase.BigTimes + "\n";
            // �{�[�i�X�Q�[�����I��
            buffer += "REG:" + playerDatabase.RegTimes + "\n";
            //buffer += "REG:" + playingDatabase.RegTimes + "\n" + "\n";

            /*
            // �{�[�i�X����(�{�[�i�X�J�n�Q�[�������L�^����Ă��邩)
            if (playerDatabase.BonusHitRecord.Count > 0 && playerDatabase.BonusHitRecord[^1].BonusStartGame > 0)
            {
                // ���I�{�[�i�X
                buffer += "BonusType:" + playerDatabase.BonusHitRecord[^1].BonusID + "\n";
                // ���I���Q�[��
                buffer += "BonusHitGame:" + playerDatabase.BonusHitRecord[^1].BonusHitGame + "\n";
                // ���܎��Q�[��
                buffer += "BonusStartGame:" + playerDatabase.BonusHitRecord[^1].BonusStartGame + "\n";
                // BIG���̐F
                buffer += "BigColor:" + playerDatabase.BonusHitRecord[^1].BigColor + "\n";
                // �l������
                buffer += "BonusPayout:" + playerDatabase.BonusHitRecord[^1].BonusPayout + "\n" + "\n";
            }
            // �{�[�i�X����(�{�[�i�X�J�n�Q�[�������܂��L�^����Ă��Ȃ��ꍇ�͈�O��\��)
            else if (playerDatabase.BonusHitRecord.Count > 1 && playerDatabase.BonusHitRecord[^1].BonusStartGame == 0)
            {
                // ���I�{�[�i�X
                buffer += "BonusType:" + playerDatabase.BonusHitRecord[^2].BonusID + "\n";
                // ���I���Q�[��
                buffer += "BonusHitGame:" + playerDatabase.BonusHitRecord[^2].BonusHitGame + "\n";
                // ���܎��Q�[��
                buffer += "BonusStartGame:" + playerDatabase.BonusHitRecord[^2].BonusStartGame + "\n";
                // BIG���̐F
                buffer += "BigColor:" + playerDatabase.BonusHitRecord[^2].BigColor + "\n";
                // �l������
                buffer += "BonusPayout:" + playerDatabase.BonusHitRecord[^2].BonusPayout + "\n" + "\n";
            }
            else
            {
                // ���I�{�[�i�X
                buffer += "BonusType:" + "\n";
                // ���I���Q�[��
                buffer += "BonusHitGame:" + "\n";
                // ���܎��Q�[��
                buffer += "BonusStartGame:" + "\n";
                // BIG���̐F
                buffer += "BigColor:" + "\n";
                // �l������
                buffer += "BonusPayout:" + "\n" + "\n";
            }

            text.text = buffer;
        }*/

        //public void SetPlayerData(PlayerDatabase playingDatabase) => this.playerDatabase = playingDatabase;
       //public void SetMedalManager(MedalManager medalManager) => this.medalManager = medalManager;

        // UI�X�V
        public void UpdatePlayerUI(PlayerDatabase playerDatabase, MedalManager medalManager)
        {
            gamesInfoUI.UpdateGamesUI(playerDatabase);
            medalInfoUI.UpdateMedalUI(playerDatabase.PlayerMedalData, medalManager);
            bonusInfoUI.UpdateBonusUI(playerDatabase);
        }
    }
}


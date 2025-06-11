using ReelSpinGame_System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    TextMeshProUGUI text;
    PlayingDatabase playingDatabase;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        buffer += "Player-" + "\n";
        buffer += "Total:" + playingDatabase.TotalGames + "\n";
        buffer += "Games:" + playingDatabase.CurrentGames + "\n" + "\n";

        buffer += "Medal:" + playingDatabase.PlayerMedalData.CurrentPlayerMedal + "\n";
        buffer += "IN:" + playingDatabase.PlayerMedalData.CurrentInMedal + "\n";
        buffer += "OUT:" + playingDatabase.PlayerMedalData.CurrentOutMedal + "\n";

        // ������
        buffer += "Dif:" + (playingDatabase.PlayerMedalData.CurrentOutMedal -
            playingDatabase.PlayerMedalData.CurrentInMedal) + "\n";

        // �@�B��
        if (playingDatabase.PlayerMedalData.CurrentInMedal > 0 && playingDatabase.PlayerMedalData.CurrentOutMedal > 0)
        {
            float payoutRate = (float)playingDatabase.PlayerMedalData.CurrentOutMedal / playingDatabase.PlayerMedalData.CurrentInMedal * 100;
            buffer += "Payout:" + payoutRate.ToString("F2") + "%" + "\n" + "\n";
        }
        else
        {
            buffer += "Payout:" + "0.00%" + "\n" + "\n";
        }

        // �r�b�O�`�����X���I��
        buffer += "BIG:" + playingDatabase.BigTimes + "\n";
        // �{�[�i�X�Q�[�����I��
        buffer += "REG:" + playingDatabase.RegTimes + "\n" + "\n";

        // �{�[�i�X����(�{�[�i�X�J�n�Q�[�������L�^����Ă��邩)
        if (playingDatabase.BonusHitDatas.Count > 0 && playingDatabase.BonusHitDatas[^1].BonusStartGame > 0)
        {
            // ���I�{�[�i�X
            buffer += "BonusType:" + playingDatabase.BonusHitDatas[^1].BonusID + "\n";
            // ���I���Q�[��
            buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^1].BonusHitGame + "\n";
            // ���܎��Q�[��
            buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^1].BonusStartGame + "\n";
            // BIG���̐F
            buffer += "BigColor:" + playingDatabase.BonusHitDatas[^1].BigColor + "\n";
            // �l������
            buffer += "BonusPayouts:" + playingDatabase.BonusHitDatas[^1].BonusPayouts + "\n" + "\n";
        }
        // �{�[�i�X����(�{�[�i�X�J�n�Q�[�������܂��L�^����Ă��Ȃ��ꍇ�͈�O��\��)
        else if (playingDatabase.BonusHitDatas.Count > 1 && playingDatabase.BonusHitDatas[^1].BonusStartGame == 0)
        {
            // ���I�{�[�i�X
            buffer += "BonusType:" + playingDatabase.BonusHitDatas[^2].BonusID + "\n";
            // ���I���Q�[��
            buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^2].BonusHitGame + "\n";
            // ���܎��Q�[��
            buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^2].BonusStartGame + "\n";
            // BIG���̐F
            buffer += "BigColor:" + playingDatabase.BonusHitDatas[^2].BigColor + "\n";
            // �l������
            buffer += "BonusPayouts:" + playingDatabase.BonusHitDatas[^2].BonusPayouts + "\n" + "\n";
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
            buffer += "BonusPayouts:" + "\n" + "\n";
        }

        text.text = buffer;
    }

    public void SetPlayerData(PlayingDatabase playingDatabase) => this.playingDatabase = playingDatabase;
}


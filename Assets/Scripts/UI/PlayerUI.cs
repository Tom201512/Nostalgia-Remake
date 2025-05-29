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

        // 機械割
        if (playingDatabase.PlayerMedalData.CurrentInMedal > 0 && playingDatabase.PlayerMedalData.CurrentOutMedal > 0)
        {
            float payoutRate = (float)playingDatabase.PlayerMedalData.CurrentOutMedal / playingDatabase.PlayerMedalData.CurrentInMedal * 100;
            buffer += "Payout:" + payoutRate.ToString("F2") + "%" + "\n" + "\n";
        }
        else
        {
            buffer += "Payout:" + "0.00%" + "\n" + "\n";
        }

        // ビッグチャンス当選回数
        buffer += "BIG:" + playingDatabase.BigTimes + "\n";
        // ボーナスゲーム当選回数
        buffer += "REG:" + playingDatabase.RegTimes + "\n";

        text.text = buffer;
    }

    public void SetPlayerData(PlayingDatabase playingDatabase) => this.playingDatabase = playingDatabase;
}


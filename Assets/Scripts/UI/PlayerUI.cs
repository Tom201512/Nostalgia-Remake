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

        // 差枚数
        buffer += "Dif:" + (playingDatabase.PlayerMedalData.CurrentOutMedal -
            playingDatabase.PlayerMedalData.CurrentInMedal) + "\n";

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
        buffer += "REG:" + playingDatabase.RegTimes + "\n" + "\n";

        // ボーナス履歴(ボーナス開始ゲーム数が記録されているか)
        if (playingDatabase.BonusHitDatas.Count > 0 && playingDatabase.BonusHitDatas[^1].BonusStartGame > 0)
        {
            // 当選ボーナス
            buffer += "BonusType:" + playingDatabase.BonusHitDatas[^1].BonusID + "\n";
            // 当選時ゲーム
            buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^1].BonusHitGame + "\n";
            // 入賞時ゲーム
            buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^1].BonusStartGame + "\n";
            // BIG時の色
            buffer += "BigColor:" + playingDatabase.BonusHitDatas[^1].BigColor + "\n";
            // 獲得枚数
            buffer += "BonusPayouts:" + playingDatabase.BonusHitDatas[^1].BonusPayouts + "\n" + "\n";
        }
        // ボーナス履歴(ボーナス開始ゲーム数がまだ記録されていない場合は一つ前を表示)
        else if (playingDatabase.BonusHitDatas.Count > 1 && playingDatabase.BonusHitDatas[^1].BonusStartGame == 0)
        {
            // 当選ボーナス
            buffer += "BonusType:" + playingDatabase.BonusHitDatas[^2].BonusID + "\n";
            // 当選時ゲーム
            buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^2].BonusHitGame + "\n";
            // 入賞時ゲーム
            buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^2].BonusStartGame + "\n";
            // BIG時の色
            buffer += "BigColor:" + playingDatabase.BonusHitDatas[^2].BigColor + "\n";
            // 獲得枚数
            buffer += "BonusPayouts:" + playingDatabase.BonusHitDatas[^2].BonusPayouts + "\n" + "\n";
        }
        else
        {
            // 当選ボーナス
            buffer += "BonusType:" + "\n";
            // 当選時ゲーム
            buffer += "BonusHitGame:" + "\n";
            // 入賞時ゲーム
            buffer += "BonusStartGame:" + "\n";
            // BIG時の色
            buffer += "BigColor:" + "\n";
            // 獲得枚数
            buffer += "BonusPayouts:" + "\n" + "\n";
        }

        text.text = buffer;
    }

    public void SetPlayerData(PlayingDatabase playingDatabase) => this.playingDatabase = playingDatabase;
}


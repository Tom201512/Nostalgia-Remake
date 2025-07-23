using ReelSpinGame_Medal;
using ReelSpinGame_System;
using TMPro;

public class PlayerUI : UIBaseClass
{
    // var
    TextMeshProUGUI text;
    PlayerDatabase playerDatabase;
    MedalManager medalManager;

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string buffer = "";

        // メダルが増える演出用
        int playerMedal = playerDatabase.PlayerMedalData.CurrentPlayerMedal - medalManager.GetRemainingPayouts();
        int outMedal = playerDatabase.PlayerMedalData.CurrentOutMedal - medalManager.GetRemainingPayouts();

        buffer += "Player-" + "\n";
        buffer += "Total:" + playerDatabase.TotalGames + "\n";
        buffer += "Games:" + playerDatabase.CurrentGames + "\n" + "\n";

        //buffer += "Medal:" + playingDatabase.PlayerMedalData.CurrentPlayerMedal + "\n";
        //buffer += "Medal(Effect):" + playerMedal + "\n";
        buffer += "Medal:" + playerMedal + "\n";
        buffer += "IN:" + playerDatabase.PlayerMedalData.CurrentInMedal + "\n";
        //buffer += "OUT:" + playingDatabase.PlayerMedalData.CurrentOutMedal+ "\n";
        //buffer += "OUT(Effect):" + outMedal + "\n";
        buffer += "OUT:" + outMedal + "\n";

        // 差枚数
        buffer += "Dif:" + (playerDatabase.PlayerMedalData.CurrentOutMedal - playerDatabase.PlayerMedalData.CurrentInMedal) + "\n";

        // 機械割
        if (playerDatabase.PlayerMedalData.CurrentInMedal > 0 && playerDatabase.PlayerMedalData.CurrentOutMedal > 0)
        {
            float payoutRate = (float)playerDatabase.PlayerMedalData.CurrentOutMedal / playerDatabase.PlayerMedalData.CurrentInMedal * 100;
            buffer += "Payout:" + payoutRate.ToString("F2") + "%" + "\n" + "\n";
        }
        else
        {
            buffer += "Payout:" + "0.00%" + "\n" + "\n";
        }

        // ビッグチャンス当選回数
        buffer += "BIG:" + playerDatabase.BigTimes + "\n";
        // ボーナスゲーム当選回数
        buffer += "REG:" + playerDatabase.RegTimes + "\n";
        //buffer += "REG:" + playingDatabase.RegTimes + "\n" + "\n";

        // ボーナス履歴(ボーナス開始ゲーム数が記録されているか)
        if (playerDatabase.BonusHitRecord.Count > 0 && playerDatabase.BonusHitRecord[^1].BonusStartGame > 0)
        {
            // 当選ボーナス
            //buffer += "BonusType:" + playingDatabase.BonusHitDatas[^1].BonusID + "\n";
            // 当選時ゲーム
            //buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^1].BonusHitGame + "\n";
            // 入賞時ゲーム
            //buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^1].BonusStartGame + "\n";
            // BIG時の色
            //buffer += "BigColor:" + playingDatabase.BonusHitDatas[^1].BigColor + "\n";
            // 獲得枚数
            //buffer += "BonusPayouts:" + playingDatabase.BonusHitDatas[^1].BonusPayouts + "\n" + "\n";
        }
        // ボーナス履歴(ボーナス開始ゲーム数がまだ記録されていない場合は一つ前を表示)
        else if (playerDatabase.BonusHitRecord.Count > 1 && playerDatabase.BonusHitRecord[^1].BonusStartGame == 0)
        {
            // 当選ボーナス
            //buffer += "BonusType:" + playingDatabase.BonusHitDatas[^2].BonusID + "\n";
            // 当選時ゲーム
            //buffer += "BonusHitGame:" + playingDatabase.BonusHitDatas[^2].BonusHitGame + "\n";
            // 入賞時ゲーム
            //buffer += "BonusStartGame:" + playingDatabase.BonusHitDatas[^2].BonusStartGame + "\n";
            // BIG時の色
            //buffer += "BigColor:" + playingDatabase.BonusHitDatas[^2].BigColor + "\n";
            // 獲得枚数
            buffer += "BonusPayouts:" + playerDatabase.BonusHitRecord[^2].BonusPayouts + "\n" + "\n";
        }
        else
        {
            // 当選ボーナス
            //buffer += "BonusType:" + "\n";
            // 当選時ゲーム
            //buffer += "BonusHitGame:" + "\n";
            // 入賞時ゲーム
            //buffer += "BonusStartGame:" + "\n";
            // BIG時の色
            //buffer += "BigColor:" + "\n";
            // 獲得枚数
            buffer += "BonusPayouts:" + "\n" + "\n";
        }

        text.text = buffer;
    }

    public void SetPlayerData(PlayerDatabase playingDatabase) => this.playerDatabase = playingDatabase;
    public void SetMedalManager(MedalManager medalManager) => this.medalManager = medalManager;
}


namespace ReelSpinGame_Bonus
{
	public class BonusBehaviour
	{
        // ボーナスの処理
        // const

        // ボーナスの種類
        public enum BonusType { BonusNone, BonusBIG, BonusREG }

        // ボーナスの状態
        public enum BonusStatus { BonusNone, BonusBIGGames, BonusJACGames };

        // BIGボーナスで当選した色
        public enum BigColor {None, Red, Blue, Black};

        // 残り小役ゲーム数
        public const int BigGames = 30;
        // 残りJACIN
        public const int JacInTimes = 3;

        // JACゲーム中
        // 残りJACゲーム数
        public const int JacGames = 12;
        // 残り当選回数
        public const int JacHits = 8;

        // var
        // 現在ストックしているボーナス
        public BonusType HoldingBonusID { get; set; }
        // ボーナス状態
        public BonusStatus CurrentBonusStatus { get; set; }
        // BIGボーナス当選時の色
        public BigColor BigChanceColor { get; set; }

        // 残りゲーム数、当選回数(JAC-INまたはJAC役)

        // 小役ゲーム中
        // 残り小役ゲーム数
        public int RemainingBigGames { get; set; }
        // 残りJACIN
        public int RemainingJacIn { get; set; }

        // JACゲーム中
        // 残りJACゲーム数
        public int RemainingJacGames { get; set; }
        // 残り当選回数
        public int RemainingJacHits { get; set; }

        // コンストラクタ
        public BonusBehaviour()
        {
            HoldingBonusID = BonusType.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigChanceColor = BigColor.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
        }

        // func

        // ビッグチャンス開始
        public void StartBigChance(BigColor bigColor)
        {
            //Debug.Log("BIG CHANCE start");
            RemainingBigGames = BigGames;
            RemainingJacIn = JacInTimes;
            CurrentBonusStatus = BonusStatus.BonusBIGGames;
            HoldingBonusID = BonusType.BonusNone;
        }

        public void SetBonusColor(BigColor bigColor) => BigChanceColor = bigColor;
        public void ResetBonusColor() => BigChanceColor = BigColor.None;
    }
}
namespace ReelSpinGame_Medal
{
    // セーブ用のデータ
    public class MedalSystemData
    {
        public int Credit { get; set; }             // クレジット枚数
        public int MaxBetAmount { get; set; }       // 最高ベット枚数
        public int LastBetAmount { get; set; }      // 最後にかけたメダル枚数
        public bool HasReplay { get; set; }         // リプレイ状態か

        public MedalSystemData()
        {
            Credit = 0;
            MaxBetAmount = MedalModel.MaxBetLimit;
            LastBetAmount = 0;
            HasReplay = false;
        }
    }
}
using System.Collections.Generic;

namespace ReelSpinGame_Reels.Payout
{
    public static class PayoutDataList
    {
        public enum BonusID { None, BigBonus, RegBonus };

        // 払い出し情報
        public class PayoutData
        {
            int payout;
            bool hasReplay;
            bool isJacPayout;
            BonusID bonusID;
            List<ReelData.ReelSymbols> symbols;

            public PayoutData(int payout,
                List<ReelData.ReelSymbols> symbols, BonusID bonusID, bool hasReplay, bool isJacPayout)
            {
                this.payout = payout;
                this.symbols = symbols;
                this.bonusID = bonusID;
                this.hasReplay = hasReplay;
                this.isJacPayout = isJacPayout;
            }
        }

        // 各種小役配当
        // BIGチャンス　各種

        // 赤7BIG
        private static PayoutData redBig = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.RedSeven,
                ReelData.ReelSymbols.RedSeven,
                ReelData.ReelSymbols.RedSeven
            },
            BonusID.BigBonus, false, false);

        // 青7BIG
        private static PayoutData blueBig = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.BlueSeven,
                ReelData.ReelSymbols.BlueSeven,
                ReelData.ReelSymbols.BlueSeven
            },
            BonusID.BigBonus, false, false);


        // BB7 BIG(BAR,BAR,7)
        private static PayoutData bb7Big = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.BAR,
                ReelData.ReelSymbols.BAR,
                ReelData.ReelSymbols.RedSeven
            },
            BonusID.BigBonus, false, false);


        // ボーナスゲーム(15枚)
        private static PayoutData bonusGame = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.BAR,
                ReelData.ReelSymbols.BAR,
                ReelData.ReelSymbols.BAR
            },
            BonusID.RegBonus, false, false);


        // チェリー(各ラインに2枚)
        private static PayoutData cherry = new PayoutData(2,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.Cherry
            },
            BonusID.None, false, false);

        // スイカ(15枚)
        private static PayoutData melon = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.Melon,
                ReelData.ReelSymbols.Melon,
                ReelData.ReelSymbols.Melon
            },
            BonusID.None, false, false);

        // ベル(10枚)
        private static PayoutData bell = new PayoutData(10,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.Bell,
                ReelData.ReelSymbols.Bell,
                ReelData.ReelSymbols.Bell
            },
            BonusID.None, false, false);


        // リプレイ(BIG中はJAC-IN+3枚)
        private static PayoutData replay = new PayoutData(3,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.Replay,
                ReelData.ReelSymbols.Replay,
                ReelData.ReelSymbols.Replay
            },
            BonusID.None, true, false);


        // JAC GAME中の払い出し

        private static PayoutData jacPayoutA = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.Replay,
                ReelData.ReelSymbols.Replay,
                ReelData.ReelSymbols.Replay
            },
            BonusID.None, false, true);

        private static PayoutData jacPayoutB = new PayoutData(15,
            new List<ReelData.ReelSymbols>
            {
                ReelData.ReelSymbols.RedSeven,
                ReelData.ReelSymbols.Replay,
                ReelData.ReelSymbols.Replay
            },
            BonusID.None, false, true);
    }
}
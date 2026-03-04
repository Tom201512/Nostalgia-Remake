using ReelSpinGame_Datas;

namespace ReelSpinGame_Lots
{
    // フラグID
    public enum FlagID
    {
        FlagNone,
        FlagBig,
        FlagReg,
        FlagCherry2,
        FlagCherry4,
        FlagMelon,
        FlagBell,
        FlagReplayJacIn,
        FlagJac,
    }

    // フラグテーブル
    public enum FlagLotTable
    {
        Normal,
        BigBonus,
        JacGame,
    };

    // フラグの処理
    public class FlagLotsModel
    {
        const int MaxFlagLots = 16384;      // 最大フラグ数

        public FlagID CurrentFlag { get; set; }                             // 現在のフラグ
        public FlagLotTable CurrentTable { get; set; }                      // 抽選テーブル

        // フラグ抽選の開始
        public void GetFlagLots(int counter, int setting, int betAmount, FlagDatabase flagDatabase)
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentTable)
            {
                case FlagLotTable.Normal:

                    // カウンタが0より少ないなら高確率テーブルを使用
                    if (counter < 0)
                    {
                        CurrentFlag = LotsNormalBTable(setting, betAmount, flagDatabase);
                    }
                    // カウンタが0以上の場合は低確率
                    else
                    {
                        CurrentFlag = LotsNormalATable(setting, betAmount, flagDatabase);
                    }

                    break;

                case FlagLotTable.BigBonus:
                    CurrentFlag = LotsBIGTable(setting, betAmount, flagDatabase);
                    break;

                case FlagLotTable.JacGame:
                    CurrentFlag = LotsJACTable(setting, flagDatabase);
                    break;

                default:
                    break;

            }
        }

        // 通常時Aテーブルでの抽選
        private FlagID LotsNormalATable(int setting, int betAmount, FlagDatabase flagDatabase)
        {
            switch(betAmount)
            {
                case 1:
                    return CheckResultByTable(setting, flagDatabase.NormalATableBet1);

                case 2:
                    return CheckResultByTable(setting, flagDatabase.NormalATableBet2);

                case 3:
                    return CheckResultByTable(setting, flagDatabase.NormalATableBet3);
            }

            return FlagID.FlagNone;
        }

        // 通常時Bテーブルでの抽選
        private FlagID LotsNormalBTable(int setting, int betAmount, FlagDatabase flagDatabase)
        {
            switch (betAmount)
            {
                case 1:
                    return CheckResultByTable(setting, flagDatabase.NormalBTableBet1);

                case 2:
                    return CheckResultByTable(setting, flagDatabase.NormalBTableBet2);

                case 3:
                    return CheckResultByTable(setting, flagDatabase.NormalBTableBet3);
            }

            return FlagID.FlagNone;
        }

        // BIG中テーブルでの抽選
        private FlagID LotsBIGTable(int setting, int betAmount, FlagDatabase flagDatabase)
        {
            switch (betAmount)
            {
                case 1:
                    return CheckResultByTable(setting, flagDatabase.BigTableBet1);

                case 2:
                    return CheckResultByTable(setting, flagDatabase.BigTableBet2);

                case 3:
                    return CheckResultByTable(setting, flagDatabase.BigTableBet3);
            }

            return FlagID.FlagNone;
        }

        // JAC中テーブルでの抽選
        private FlagID LotsJACTable(int setting, FlagDatabase flagDatabase)
        {
            return CheckResultByTable(setting, flagDatabase.JacTable);
        }

        // テーブル、設定値とベット枚数からフラグ判定
        private FlagID CheckResultByTable(int setting, FlagDatabaseSet flagTable)
        {
            // 16384フラグ取得(0-16383)
            int flag = UnityEngine.Random.Range(0, MaxFlagLots);
            int flagCheckNum = 0;

            // 各フラグごとに抽選
            foreach(FlagData flagData in flagTable.FlagDataList)
            {
                // 当選フラグ数が0でなければ抽選
                if (flagData.FlagCountBySetting[setting - 1] > 0)
                {
                    flagCheckNum += flagData.FlagCountBySetting[setting - 1];
                    // フラグ数より少ない数値を引いたら当選とする
                    if (flag < flagCheckNum)
                    {
                        return flagData.FlagID;
                    }
                }
            }
            // 何も当たらなければ"はずれ"を返す
            return FlagID.FlagNone;
        }
    }
}
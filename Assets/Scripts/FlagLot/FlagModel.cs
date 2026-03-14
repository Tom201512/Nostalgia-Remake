using ReelSpinGame_Scriptable;
using UnityEngine;

namespace ReelSpinGame_Flag
{
    // フラグの処理
    public class FlagModel
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

        public const int RandomSettingValue = 0;        // ランダム設定を使用するときの数値
        public const int MinSlotSetting = 1;       // 最低設定値
        public const int MaxSlotSetting = 6;       // 最高設定値
        public const int SlotSettingErrorValue = -1;    // 台設定エラー値

        const int MaxFlagLots = 16384;      // 最大フラグ数


        public FlagID CurrentFlag { get; set; }                             // 現在のフラグ
        public FlagLotTable CurrentFlagTable { get; set; }                      // 抽選テーブル

        // スロット台設定
        public int CurrentSlotSetting { get; set; }
        // ランダム設定を使用しているか
        public bool IsRandomSlotSetting { get; set; }

        public FlagModel()
        {
            CurrentFlag = FlagID.FlagNone;
            CurrentFlagTable = FlagLotTable.Normal;
            CurrentSlotSetting = SlotSettingErrorValue;
            IsRandomSlotSetting = false;
        }

        // ランダムに台設定を決める
        public void SetRandomSlotSetting()
        {
            CurrentSlotSetting = Random.Range(MinSlotSetting, MaxSlotSetting + 1);
            IsRandomSlotSetting = true;
        }

        // フラグ抽選の開始
        public void GetFlagLots(int counter, int betAmount, FlagDatabase flagDatabase)
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentFlagTable)
            {
                case FlagLotTable.Normal:

                    // カウンタが0より少ないなら高確率テーブルを使用
                    if (counter < 0)
                    {
                        CurrentFlag = LotsNormalBTable(betAmount, flagDatabase);
                    }
                    // カウンタが0以上の場合は低確率
                    else
                    {
                        CurrentFlag = LotsNormalATable(betAmount, flagDatabase);
                    }

                    break;

                case FlagLotTable.BigBonus:
                    CurrentFlag = LotsBIGTable(betAmount, flagDatabase);
                    break;

                case FlagLotTable.JacGame:
                    CurrentFlag = LotsJACTable(flagDatabase);
                    break;

                default:
                    break;

            }
        }

        // 通常時Aテーブルでの抽選
        private FlagID LotsNormalATable(int betAmount, FlagDatabase flagDatabase)
        {
            switch(betAmount)
            {
                case 1:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalATableBet1);

                case 2:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalATableBet2);

                case 3:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalATableBet3);
            }

            return FlagID.FlagNone;
        }

        // 通常時Bテーブルでの抽選
        private FlagID LotsNormalBTable(int betAmount, FlagDatabase flagDatabase)
        {
            switch (betAmount)
            {
                case 1:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalBTableBet1);

                case 2:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalBTableBet2);

                case 3:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.NormalBTableBet3);
            }

            return FlagID.FlagNone;
        }

        // BIG中テーブルでの抽選
        private FlagID LotsBIGTable(int betAmount, FlagDatabase flagDatabase)
        {
            switch (betAmount)
            {
                case 1:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.BigTableBet1);

                case 2:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.BigTableBet2);

                case 3:
                    return CheckResultByTable(CurrentSlotSetting, flagDatabase.BigTableBet3);
            }

            return FlagID.FlagNone;
        }

        // JAC中テーブルでの抽選
        private FlagID LotsJACTable(FlagDatabase flagDatabase) => CheckResultByTable(CurrentSlotSetting, flagDatabase.JacTable);

        // テーブル、設定値とベット枚数からフラグ判定
        private FlagID CheckResultByTable(int setting, FlagDatabaseSet flagTable)
        {
            // 16384フラグ取得(0-16383)
            int flag = Random.Range(0, MaxFlagLots);
            int flagCheckNum = 0;

            // 各フラグごとに抽選
            foreach (FlagData flagData in flagTable.FlagDataList)
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
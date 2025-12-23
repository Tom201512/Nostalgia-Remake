using ReelSpinGame_Datas;
using UnityEngine;

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
        const int MaxFlagLots = 16384;                                      // 最大フラグ数
        const int SeekNum = 6;                                              // テーブルシーク位置

        public FlagID CurrentFlag { get; set; }                             // 現在のフラグ
        public FlagLotTable CurrentTable { get; set; }                      // 抽選テーブル

        // フラグ抽選の開始
        public void GetFlagLots(int counter, int setting, int betAmount, FlagDatabase flagDatabase)
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentTable)
            {
                case FlagLotTable.Normal:

                    FlagID[] lotResultNormal = new FlagID[]
                    {
                        FlagID.FlagBig,
                        FlagID.FlagReg,
                        FlagID.FlagCherry2,
                        FlagID.FlagCherry4,
                        FlagID.FlagMelon,
                        FlagID.FlagBell,
                        FlagID.FlagReplayJacIn,
                    };

                    // カウンタが0より少ないなら高確率
                    if (counter < 0)
                    {
                        CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.NormalBTable, lotResultNormal);
                    }
                    // カウンタが0以上の場合は低確率
                    else
                    {
                        CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.NormalATable, lotResultNormal);
                    }

                    break;

                case FlagLotTable.BigBonus:

                    FlagID[] lotResultBig = new FlagID[]
                    {
                         FlagID.FlagCherry2,
                         FlagID.FlagCherry4,
                         FlagID.FlagMelon,
                         FlagID.FlagReplayJacIn,
                         FlagID.FlagBell,
                    };

                    CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.BigTable, lotResultBig);
                    break;

                case FlagLotTable.JacGame:
                    CurrentFlag = BonusGameLots(flagDatabase.JacNonePoss);
                    break;

                default:
                    break;

            }
        }

        // テーブル、設定値とベット枚数からフラグ判定
        FlagID CheckResultByTable(int setting, int betAmount, FlagDataSets flagTable, FlagID[] lotResult)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // ベット枚数に合わせたテーブルを参照するようにする
            int offset = SeekNum * (betAmount - 1);

            // 各フラグごとに
            int index = 0;
            foreach (float f in flagTable.FlagDataBySettings[setting + offset - 1].FlagTable)
            {
                //各役ごとに抽選
                flagCheckNum += Mathf.RoundToInt(MaxFlagLots / f);

                if (flag < flagCheckNum)
                {
                    return lotResult[index];
                }
                index += 1;
            }

            // 何も当たらなければ"はずれ"を返す
            return FlagID.FlagNone;
        }

        // BONUS GAME中の抽選
        FlagID BonusGameLots(float jacNoneProbability)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選。端数切捨て)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt(MaxFlagLots / jacNoneProbability);
            if (flag < flagCheckNum)
            {
                return FlagID.FlagNone;
            }

            // はずれでなければJAC当選
            return FlagID.FlagJac;
        }

        // フラグ抽選
        int GetFlag() => Random.Range(0, MaxFlagLots);
    }
}
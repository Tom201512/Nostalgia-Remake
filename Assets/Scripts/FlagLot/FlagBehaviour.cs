using ReelSpinGame_Datas;
using UnityEngine;

using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Lots
{
	public class FlagBehaviour
	{
        // フラグの処理

        // const
        // 最大フラグ数
        const int MaxFlagLots = 16384;
        // テーブルシーク位置
        const int SeekNum = 6;

        // フラグID
        public enum FlagID { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJacIn, FlagJac }
        // フラグテーブル
        public enum FlagLotMode { Normal, BigBonus, JacGame };

        // var
        // 現在フラグ(プロパティ)
        public FlagID CurrentFlag { get; set; }
        // 参照するテーブルID
        public FlagLotMode CurrentTable { get; set; }
        // フラグカウンタ
        public FlagCounter.FlagCounter FlagCounter { get; private set; }
        // 強制役の設定
        public bool UseInstant { get; private set; }
        public FlagID InstantFlagID { get; private set; }

        // 抽選順番(最終的に当選したフラグを参照するのに使う)
        private FlagID[] lotResultNormal = new FlagID[]
        {
            FlagID.FlagBig,
            FlagID.FlagReg,
            FlagID.FlagCherry2,
            FlagID.FlagCherry4,
            FlagID.FlagMelon,
            FlagID.FlagBell,
            FlagID.FlagReplayJacIn
        };

        // BIG CHANCE中
        private FlagID[] lotResultBig = new FlagID[]
        {
            FlagID.FlagCherry2,
            FlagID.FlagCherry4,
            FlagID.FlagMelon,
            FlagID.FlagReplayJacIn,
            FlagID.FlagBell
        };

        // コンストラクタ
        public FlagBehaviour()
        {
            FlagCounter = new FlagCounter.FlagCounter();
        }

        // func

        // フラグ抽選の開始
        public void GetFlagLots(int setting, int betAmount, FlagDatabase flagDatabase)
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentTable)
            {
                case FlagLotMode.Normal:

                    // カウンタが0より少ないなら高確率
                    if (FlagCounter.Counter < 0)
                    {
                        CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.NormalBTable, lotResultNormal);
                    }
                    // カウンタが0以上の場合は低確率
                    else
                    {
                        CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.NormalATable, lotResultNormal);
                    }

                    break;

                case FlagLotMode.BigBonus:
                    CurrentFlag = CheckResultByTable(setting, betAmount, flagDatabase.BigTable, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    CurrentFlag = BonusGameLots(flagDatabase.JacNonePoss);
                    break;

                default:
                    break;

            }
        }

        // テーブル、設定値とベット枚数からフラグ判定
        private FlagID CheckResultByTable(int setting, int betAmount, FlagDataSets flagTable, FlagID[] lotResult)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // ベット枚数に合わせたテーブルを参照するようにする
            int offset = SeekNum * (betAmount - 1);
            //Debug.Log(offset);

            int index = 0;

            //Debug.Log(setting + offset - 1);
            foreach (float f in flagTable.FlagDataBySettings[setting + offset - 1].FlagTable)
            {
                //各役ごとに抽選
                flagCheckNum += Mathf.RoundToInt((float)MaxFlagLots / f);

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
        private FlagID BonusGameLots(float nonePoss)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選。端数切捨て)
            int flagCheckNum = 0;
            int flag = GetFlag();

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt(MaxFlagLots / nonePoss);
            if (flag < flagCheckNum)
            {
                return FlagID.FlagNone;
            }

            // 何も当たらなければ"JAC役"を返す
            return FlagID.FlagJac;
        }

        // フラグ抽選
        private int GetFlag()
        {
            // 16384フラグを得る(0~16383)
            int flag = Random.Range(0, MaxFlagLots);
            //Debug.Log("You get:" + flag);
            return flag;
        }
    }
}
using System;
using System.IO;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // フラグ抽選

        // const
        // 最大フラグ数
        const int MaxFlagLots = 16384;

        // enum
        // フラグID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJACin, FlagJAC }
        // フラグテーブル
        public enum FlagLotMode { NormalA, NormalB, BigBonus, JacGame };

        // var
        // 現在フラグ(プロパティ)
        public FlagId CurrentFlag { get; private set; } = FlagId.FlagNone;
        // 参照するテーブルID
        public FlagLotMode CurrentTable { get; private set; } = FlagLotMode.NormalA;
        // テーブル内数値
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;
        // JAC GAME中はずれ
        private float jacNoneProb;

        // 抽選順番(最終的に当選したフラグを参照するのに使う)
        private FlagId[] lotResultNormal = new FlagId[] 
        {
            FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // BIG CHANCE中
        private FlagId[] lotResultBig = new FlagId[] 
        {
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin
        };

        // コンストラクタ
        public FlagLots(int setting, StreamReader tableAData,
            StreamReader tableBData, StreamReader tableBIGData, int jacNoneProb)
        {
            // 設定値をもとにテーブル作成
            Debug.Log("Lots Setting set by :" + setting);

            // 設定値をもとにデータを得る(設定値の列まで読み込む)
            for (int i = 0; i < setting - 1; i++)
            {
                tableAData.ReadLine();
                tableBData.ReadLine();
                tableBIGData.ReadLine();
            }

            // データ読み込み
            string[] valueA = tableAData.ReadLine().Split(',');
            string[] valueB = tableBData.ReadLine().Split(',');
            string[] valueBIG = tableBIGData.ReadLine().Split(',');

            // 読み込んだテーブルをfloat配列に変換
            flagLotsTableA = Array.ConvertAll(valueA, float.Parse);
            flagLotsTableB = Array.ConvertAll(valueB, float.Parse);
            flagLotsTableBIG = Array.ConvertAll(valueBIG, float.Parse);

            // JACはずれの設定
            this.jacNoneProb = jacNoneProb;

            Debug.Log("NormalA Table:");
            for (int i = 0; i < lotResultNormal.Length; i++)
            {
                Debug.Log(lotResultNormal[i].ToString() + ":" + flagLotsTableA[i]);
            }

            Debug.Log("NormalB Table:");
            for (int i = 0; i < lotResultNormal.Length; i++)
            {
                Debug.Log(lotResultNormal[i].ToString() + ":" + flagLotsTableB[i]);
            }

            Debug.Log("BIG Table:");
            for (int i = 0; i < lotResultBig.Length; i++)
            {
                Debug.Log(lotResultBig[i].ToString() + ":" + flagLotsTableBIG[i]);
            }

            Debug.Log("JAC None Probability:" + this.jacNoneProb);
        }

        // func
        // テーブル変更
        public void ChangeTable(FlagLotMode mode)
        {
            Debug.Log("Changed mode:" + mode);
            CurrentTable = mode;
        }

        // フラグ抽選の開始
        public void GetFlagLots()
        {
            // 現在の参照テーブルをもとに抽選
            switch (CurrentTable)
            {
                case FlagLotMode.NormalA:
                    CurrentFlag = CheckResultByTable(flagLotsTableA, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    CurrentFlag = CheckResultByTable(flagLotsTableB, lotResultNormal);
                    break;

                case FlagLotMode.BigBonus:
                    CurrentFlag = CheckResultByTable(flagLotsTableBIG, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    CurrentFlag = BonusGameLots();
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + CurrentFlag);
        }

        // テーブルからフラグ判定
        private FlagId CheckResultByTable(float[] lotsTable, FlagId[] lotResult)
        {
            // 16384フラグを得る
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;


            for (int i = 0; i < lotsTable.Length; i++)
            {
                //各役ごとに抽選
                flagCheckNum += Mathf.FloorToInt((float)MaxFlagLots / lotsTable[i]);

                if (flag < flagCheckNum)
                {
                    return lotResult[i];
                }
            }
            // 何も当たらなければ"はずれ"を返す
            return FlagId.FlagNone;
        }

        // BONUS GAME中の抽選
        private FlagId BonusGameLots()
        {
            // 16384フラグを得る(0~16383)
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選。端数切捨て)
            int flagCheckNum = 0;

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / jacNoneProb);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // 何も当たらなければ"JAC役"を返す
            return FlagId.FlagJAC;
        }
    }
}

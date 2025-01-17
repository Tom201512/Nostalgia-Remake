using ReelSpinGame_Lots.FlagProb;
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


        // 現在フラグ(プロパティ)
        public FlagId CurrentFlag { get; private set; } = FlagId.FlagNone;

        // 参照するテーブルID
        public FlagLotMode CurrentTable { get; private set; } = FlagLotMode.NormalA;

        // テーブル内数値
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;


        // 抽選順番(最終的に当選したフラグを参照するのに使う)
        private FlagId[] lotResultNormal = new FlagId[] {FlagId.FlagBig,
            FlagId.FlagReg,
            FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin};

        // BIG CHANCE中
        private FlagId[] lotResultBig = new FlagId[] {FlagId.FlagCherry2,
            FlagId.FlagCherry4,
            FlagId.FlagMelon,
            FlagId.FlagBell,
            FlagId.FlagReplayJACin};


        // コンストラクタ
        public FlagLots(int settingNum)
        {
            // 設定値をもとにテーブル作成
            Debug.Log("Lots Setting set by :" + settingNum);

            MakeTables(settingNum);
        }


        // func

        // テーブル作成(初期化時)
        private void MakeTables(int settingNum)
        {
            flagLotsTableA = new float[] { FlagLotsProb.BigProbability[settingNum - 1],
                    FlagLotsProb.RegProbability[settingNum- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbA,
                    FlagLotsProb.MelonProbA,
                    FlagLotsProb.BellProbA,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableB = new float[] { FlagLotsProb.BigProbability[settingNum - 1],
                    FlagLotsProb.RegProbability[settingNum- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbB,
                    FlagLotsProb.MelonProbB,
                    FlagLotsProb.BellProbB,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableBIG = new float[] {
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.MelonProbInBig,
                    FlagLotsProb.BigBellProbability[settingNum - 1],
                    FlagLotsProb.JACinProbInBig};

            Debug.Log("NormalA Table:");
            for(int i = 0; i < lotResultNormal.Length; i++)
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
        }

        public void ChangeTable(FlagLotMode mode) => CurrentTable = mode;

        // フラグ抽選の開始
        public void GetFlagLots()
        {
            // 現在の参照テーブルをもとに抽選

            switch(CurrentTable)
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

        // BONUS GAME中の抽選
        private FlagId BonusGameLots()
        {
            // 16384フラグを得る
            int flag = UnityEngine.Random.Range(0, MaxFlagLots - 1);
            Debug.Log("You get:" + flag);

            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt((float)MaxFlagLots / FlagLotsProb.JAC_NONE_PROB);
            if (flag < flagCheckNum)
            {
                return FlagId.FlagNone;
            }

            // 何も当たらなければ　JAC役　を返す
            return FlagId.FlagJAC;
        }

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

            return FlagId.FlagNone;
        }
    }
}

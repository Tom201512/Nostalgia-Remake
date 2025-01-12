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

        //小役カウンタ増加値
        //const int CounterIncrease = 256;

        //小役カウンタ減少値
        //const int CounterDecrease1to4 = 100;

        //const int CounterDecrease5 = 104;

        //const int CounterDecrease6 = 1808;


        // enum

        // フラグID
        public enum FlagId { FlagNone, FlagBig, FlagReg, FlagCherry2, FlagCherry4, FlagMelon, FlagBell, FlagReplayJACin, FlagJAC }

        // フラグテーブル
        public enum FlagLotMode { NormalA, NormalB, BigBonus, JacGame };

        // var

        // 台設定
        private int lotsSetting;

        // 現在フラグ
        private FlagId currentFlag = FlagId.FlagNone;

        // 参照するテーブルID
        private FlagLotMode currentTable = FlagLotMode.NormalB;


        // 小役カウンタ
        private int flagCounter = 0;

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
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting, int flagCounter)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // 設定値をもとにテーブル作成
            this.lotsSetting = lotsSetting;

            Debug.Log("Lots Setting set by :" + lotsSetting);

            MakeTables();

            this.flagCounter = flagCounter;
        }


        // func

        // テーブル作成(初期化時)
        private void MakeTables()
        {
            flagLotsTableA = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbA,
                    FlagLotsProb.MelonProbA,
                    FlagLotsProb.BellProbA,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableB = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.Cherry2Prob,
                    FlagLotsProb.Cherry4ProbB,
                    FlagLotsProb.MelonProbB,
                    FlagLotsProb.BellProbB,
                    FlagLotsProb.ReplayJACinProb};

            flagLotsTableBIG = new float[] {
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.CherryProbInBig,
                    FlagLotsProb.MelonProbInBig,
                    FlagLotsProb.BigBellProbability[lotsSetting - 1],
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

        // フラグ抽選の開始
        public void GetFlagLots()
        {
            // 現在の参照テーブルをもとに抽選

            switch(currentTable)
            {
                case FlagLotMode.NormalA:
                    currentFlag = CheckResultByTable(flagLotsTableA, lotResultNormal);
                    break;

                case FlagLotMode.NormalB:
                    currentFlag = CheckResultByTable(flagLotsTableB, lotResultNormal);
                    break;

                case FlagLotMode.BigBonus:
                    currentFlag = CheckResultByTable(flagLotsTableBIG, lotResultBig);
                    break;

                case FlagLotMode.JacGame:
                    currentFlag = BonusGameLots();
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + currentFlag);
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

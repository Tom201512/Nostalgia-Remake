using ReelSpinGame_Lots.FlagProb;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // フラグ抽選

        // const

        // 最大フラグ数
        const int MAX_FLAG_LOTS = 16384;

        // enum

        // フラグID
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY_JACIN, FLAG_JAC }

        // フラグテーブル
        public enum FLAG_LOT_MODE { LOT_NORMAL_A, LOT_NORMAL_B, LOT_BIGBONUS, LOT_JACGAME };

        // var

        // 台設定
        private int lotsSetting;

        // 現在フラグ
        private FLAG_ID currentFlag = FLAG_ID.FLAG_NONE;

        // 参照するテーブルID
        private FLAG_LOT_MODE currentTable = FLAG_LOT_MODE.LOT_NORMAL_A;

        // テーブル内数値
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;

        // 抽選順番(最終的に当選したフラグを参照するのに使う)
        private FLAG_ID[] lotResultNormal = new FLAG_ID[] {FLAG_ID.FLAG_BIG,
            FLAG_ID.FLAG_REG,
            FLAG_ID.FLAG_CHERRY2,
            FLAG_ID.FLAG_CHERRY4,
            FLAG_ID.FLAG_MELON,
            FLAG_ID.FLAG_BELL,
            FLAG_ID.FLAG_REPLAY_JACIN};

        // BIG CHANCE中
        private FLAG_ID[] lotResultBig = new FLAG_ID[] {FLAG_ID.FLAG_CHERRY2,
            FLAG_ID.FLAG_CHERRY4,
            FLAG_ID.FLAG_MELON,
            FLAG_ID.FLAG_BELL,
            FLAG_ID.FLAG_REPLAY_JACIN};



        // コンストラクタ
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // 設定値をもとにテーブル作成
            this.lotsSetting = lotsSetting;

            Debug.Log("Lots Setting set by :" + lotsSetting);

            MakeTables();
        }


        // func

        private void MakeTables()
        {
            flagLotsTableA = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.CHERRY2_PROB,
                    FlagLotsProb.CHERRY4_PROB_A,
                    FlagLotsProb.MELON_PROB_A,
                    FlagLotsProb.BELL_PROB_A,
                    FlagLotsProb.REPLAY_PROB};

            flagLotsTableB = new float[] { FlagLotsProb.BigProbability[lotsSetting - 1],
                    FlagLotsProb.RegProbability[lotsSetting- 1],
                    FlagLotsProb.CHERRY2_PROB,
                    FlagLotsProb.CHERRY4_PROB_B,
                    FlagLotsProb.MELON_PROB_B,
                    FlagLotsProb.BELL_PROB_B,
                    FlagLotsProb.REPLAY_PROB};

            flagLotsTableBIG = new float[] {
                    FlagLotsProb.BIG_CHERRY_PROB,
                    FlagLotsProb.BIG_CHERRY_PROB,
                    FlagLotsProb.BIG_MELON_PROB,
                    FlagLotsProb.BigBellProbability[lotsSetting - 1],
                    FlagLotsProb.BIG_JACIN_PROB};

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

        public void GetFlagLots()
        {
            // 16384フラグを得る
            int flag = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flag);

            // 現在の参照テーブルをもとに抽選

            switch(currentTable)
            {
                case FLAG_LOT_MODE.LOT_NORMAL_A:
                case FLAG_LOT_MODE.LOT_NORMAL_B:
                    currentFlag = LotsFlags(flag);
                    break;

                case FLAG_LOT_MODE.LOT_BIGBONUS:
                    currentFlag = BigChanceLots(flag);
                    break;

                case FLAG_LOT_MODE.LOT_JACGAME:
                    currentFlag = BonusGameLots(flag);
                    break;

                default:
                    Debug.LogError("No table found");
                    break;

            }
            Debug.Log("Flag:" + currentFlag);
        }

        private FLAG_ID LotsFlags(int _flag)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;

            // 小役カウンタが高確率なら
            if(currentTable == FLAG_LOT_MODE.LOT_NORMAL_B)
            {
                for (int i = 0; i < lotResultNormal.Length; i++)
                {
                    //各役ごとに抽選
                    flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableB[i]);

                    if (_flag < flagCheckNum)
                    {
                        return lotResultNormal[i];
                    }
                }
            }

            // 小役カウンタが低確率なら
            else
            {
                for (int i = 0; i < lotResultNormal.Length; i++)
                {
                    //各役ごとに抽選
                    flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableB[i]);

                    if (_flag < flagCheckNum)
                    {
                        return lotResultNormal[i];
                    }
                }
            }

            // 何も当たらなければ　はずれ　を返す
            return FLAG_ID.FLAG_NONE;
        }

        private FLAG_ID BigChanceLots(int _flag)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;

            for (int i = 0; i < lotResultBig.Length; i++)
            {
                //各役ごとに抽選
                flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / flagLotsTableBIG[i]);

                if (_flag < flagCheckNum)
                {
                    return lotResultBig[i];
                }
            }

            // 何も当たらなければ　はずれ　を返す
            return FLAG_ID.FLAG_NONE;
        }

        private FLAG_ID BonusGameLots(int _flag)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;

            // はずれ抽選
            flagCheckNum = Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.JAC_NONE_PROB);
            if (_flag < flagCheckNum)
            {
                return FLAG_ID.FLAG_NONE;
            }

            // 何も当たらなければ　JAC役　を返す
            return FLAG_ID.FLAG_JAC;
        }
    }
}

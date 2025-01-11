using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReelSpinGame_Lots.FlagProb;

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
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY, FLAG_JAC }

        // フラグテーブル
        public enum FLAG_LOT_MODE { LOT_NORMAL_A, LOT_NORMAL_B, LOT_BIGBONUS, LOT_JACGAME };


        // var

        // 台設定
        private int lotsSetting;

        // 現在フラグ
        private FLAG_ID currentFlag = FLAG_ID.FLAG_NONE;

        // 参照するテーブルID
        private FLAG_LOT_MODE currentTable = FLAG_LOT_MODE.LOT_NORMAL_A;

        //テーブル内数値
        private float[] flagLotsTableA;
        private float[] flagLotsTableB;
        private float[] flagLotsTableBIG;


        // コンストラクタ
        public FlagLots(FlagLotsTest flagLotsTest, int lotsSetting)
        {
            flagLotsTest.DrawLots += GetFlagLots;

            // 設定値をもとにテーブル作成
            this.lotsSetting = lotsSetting;
        }

        
        // func

        public void GetFlagLots()
        {
            // 16384フラグを得る
            int flag = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flag);

            // 現在の参照テーブルをもとに抽選
            currentFlag = LotsFlags(flag);
            Debug.Log("Flag:" + currentFlag);
        }

        private FLAG_ID LotsFlags(int _flag)
        {
            // 判定用の数値(16384/小役確率で求め、これより少ないフラグを引いたら当選)
            int flagCheckNum = 0;

            
            // BIG抽選
            flagCheckNum = Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.BigProbability[0]);
            if(_flag < flagCheckNum)
            {
                Debug.Log("BIG hits");
                return FLAG_ID.FLAG_BIG;
            }

            // REG抽選
            flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / FlagLotsProb.RegProbability[0]);
            if (_flag < flagCheckNum)
            {
                Debug.Log("REG hits");
                return FLAG_ID.FLAG_REG;
            }

            //ここから2枚チェリーなど抽選

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

            return FLAG_ID.FLAG_JAC;
        }
    }
}

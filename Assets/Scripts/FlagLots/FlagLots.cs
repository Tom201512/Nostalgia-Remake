using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Lots.Flag
{
    public class FlagLots
    {
        // フラグ抽選

        // const

        // 最大フラグ数
        const int MAX_FLAG_LOTS = 16384;

        // ボーナス確率

        // BIG CHANCE

        public const float BIG_PROB_ST1 = 294.0f;
        public const float BIG_PROB_ST2 = 285.0f;
        public const float BIG_PROB_ST3 = 274.0f;
        public const float BIG_PROB_ST4 = 254.0f;
        public const float BIG_PROB_ST5 = 244.0f;
        public const float BIG_PROB_ST6 = 240.0f;

        // BONUS GAME

        public const float REG_PROB_ST1 = 480.0f;
        public const float REG_PROB_ST2 = 471.0f;
        public const float REG_PROB_ST3 = 420.0f;
        public const float REG_PROB_ST4 = 415.0f;
        public const float REG_PROB_ST5 = 285.0f;
        public const float REG_PROB_ST6 = 260.0f;

        // 通常低確率時のフラグ

        public const float CHERRY4_PROB_A = 256.6f;
        public const float MELON_PROB_A = 78.5f;
        public const float BELL_PROB_A = 72.5f;

        // 通常高確率時のフラグ

        public const float CHERRY4_PROB_B = 256.6f;
        public const float MELON_PROB_B = 78.5f;
        public const float BELL_PROB_B = 72.5f;

        // それ以外の通常時(変動なし)
        public const float CHERRY2_PROB = 4.8f;
        public const float REPLAY_PROB = 7.1f;


        // BIG CHANCE中のフラグ

        // チェリー(2枚/4枚)
        public const float BIG_CHERRY_PROB = 512.0f;

        public const float BIG_MELON_PROB = 4.8f;
        public const float BIG_JACIN_PROB = 3.2f;

        // BIG中ベル確率

        public const float BIG_BELL_PROB_ST1 = 3.86f;
        public const float BIG_BELL_PROB_ST2 = 3.76f;
        public const float BIG_BELL_PROB_ST3 = 3.66f;
        public const float BIG_BELL_PROB_ST4 = 3.55f;
        public const float BIG_BELL_PROB_ST5 = 3.54f;
        public const float BIG_BELL_PROB_ST6 = 3.48f;

        // BONUS GAME中のはずれ確率
        public const float JAC_NONE_PROB = 256.0f;

        // enum
        // フラグID
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY, FLAG_JAC }

        // フラグテーブル
        public enum FLAG_LOT_MODE { LOT_NORMAL_A, LOT_NORMAL_B, LOT_BIGBONUS, LOT_JACGAME };


        // var

        // 現在フラグ
        int flagNum = 0;

        // 参照するテーブルID
        int currentTable = 0;


        public FlagLots(FlagLotsTest flagLotsTest)
        {
            flagLotsTest.DrawLots += GetFlagLots;
        }

        
        // func

        public void GetFlagLots()
        {
            flagNum = UnityEngine.Random.Range(0, MAX_FLAG_LOTS - 1);
            Debug.Log("You get:" + flagNum);
            Debug.Log("Flag:" + ChooseFlagID());
        }

        private FLAG_ID ChooseFlagID()
        {
            int flagCheckNum = 0;

            // BIG抽選
            flagCheckNum = Mathf.FloorToInt((float)MAX_FLAG_LOTS / BIG_PROB_ST1);
            if(flagNum < flagCheckNum)
            {
                Debug.Log("BIG hits");
                return FLAG_ID.FLAG_BIG;
            }

            // REG抽選
            flagCheckNum += Mathf.FloorToInt((float)MAX_FLAG_LOTS / REG_PROB_ST1);
            if (flagNum < flagCheckNum)
            {
                Debug.Log("REG hits");
                return FLAG_ID.FLAG_REG;
            }

            return FLAG_ID.FLAG_NONE;
        }
    }
}

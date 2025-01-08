using System;
using UnityEngine;

namespace ReelSpinGame_Rules
{
    //DataRules

    [Serializable]
    public static class DataRules
    {
        //System
        public const float WAIT_TIME = 4.1f;
        public enum SLOT_SETTING {SLOT_1, SLOT_2, SLOT_3, SLOT_4, SLOT_5, SLOT_6}

        //Medal
        public const int MAX_CREDITS = 50;
        public const int MAX_BET = 3;
        public const float MEDAL_UPDATETIME = 0.12f;
        public const int MAX_PAYOUT = 15;

        //Symbols
        public const int MAX_SYMBOL_ID = 10;

        //symbolID identifers(You can name IDs to make it clean what symbol it is.)
        public enum SYMBOL_ID { RED_SEVEN, BLUE_SEVEN, BAR ,CHERRY, MELON, BELL, REPLAY, SYMBOL_7, SYMBOL_8, SYMBOL_9 , ANY};

        //Reels
        public const int MAX_REEL_AMOUNTS = 3;
        public const int MAX_REEL_ARRAY = 21;
        public const float ROTATE_RPM = 79.7f;
        public const float REEL_WEIGHT = 25.5f; //kg
        public const float REEL_RADIUS = 12.75f; //cm
        public const int MAX_REEL_DELAY = 4;

        public enum REEL_COLUMN_ID { REEL_LEFT, REEL_MIDDLE, REEL_RIGHT};
        public enum REEL_POS_OFFSET { LOWER_3RD = -2, LOWER_2ND, LOWER, CENTER, UPPER , UPPER_2ND, UPPER_3RD}
        public const int REEL_POS_MAXCOUNT = 7;

        //Flag
        public enum FLAG_ID { FLAG_NONE, FLAG_BIG, FLAG_REG, FLAG_CHERRY2, FLAG_CHERRY4, FLAG_MELON, FLAG_BELL, FLAG_REPLAY, FLAG_JAC}
        public const int MAX_RANDOM = 6;
        public const int MAX_FLAG = 65536;
        public enum FLAG_LOT_MODE { LOT_NORMAL, LOT_BIGBONUS, LOT_JACGAME};

        //BonusType
        public enum BONUS_TYPE {NO_BONUS, BIG_BONUS, REG_BONUS, SIN_BONUS}
        public const int MAX_BIGBONUS_GAMES = 30;
        public const int MAX_JAC_GAMES = 12;
        public const int MAX_JAC_HITS = 8;
        public const int MAX_JAC_IN = 3;

        //ReelBehavior
        public const int CONDITION_MAXTOREAD = 6;
        public const int OTHERREEL_A_MAXTOREAD = CONDITION_MAXTOREAD + 10;
        public const int OTHERREEL_B_MAXTOREAD = OTHERREEL_A_MAXTOREAD + 10;
        public const int REEL_TABLE_ID_MAXTOREAD = OTHERREEL_B_MAXTOREAD + 1;

        public const int CONDITION_BITOFFSET = 4;

        public enum CONDITIONS{C_FLAG, C_PUSH, C_BONUS, C_BET, C_FIRST, C_RANDOM }
    }

    //Util
    public static class OriginalMathmatics
    {
        public static float ReturnAngularVelocity(float rpsValue)
        {
            //Radian
            float radian = rpsValue * 2.0f * MathF.PI;
            //ConvertRadian to angle per seconds
            return radian * 180.0f / MathF.PI;
        }

        //Use when you check Reel condition
        public static int ConvertToArrayBit(int data)
        {
            //Do not convert if data is 0
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }

        public static float ReturnReelAccerateSpeed(float rpsValue)
        {
            return DataRules.REEL_RADIUS / 100f * ReturnAngularVelocity(rpsValue) / 1000.0f;
        }
    }
}


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
        public const int MAX_PAYOUT = 15;

        //Reels
        public const int MAX_REEL_AMOUNTS = 3;
        public const int MAX_REEL_ARRAY = 21;
        public const int MAX_REEL_DELAY = 4;

        public enum REEL_COLUMN_ID { REEL_LEFT, REEL_MIDDLE, REEL_RIGHT};
        public enum REEL_POS_OFFSET { LOWER_3RD = -2, LOWER_2ND, LOWER, CENTER, UPPER , UPPER_2ND, UPPER_3RD}
        public const int REEL_POS_MAXCOUNT = 7;

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


        //Use when you check Reel condition
        public static int ConvertToArrayBit(int data)
        {
            //Do not convert if data is 0
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }
}


using System.Collections.ObjectModel;

namespace ReelSpinGame_Lots.FlagProb
{
    public static class FlagLotsProb
    {
        // 各種フラグ確率
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


        // BIG確率
        public static ReadOnlyCollection<float> BigProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    BIG_PROB_ST1,
                    BIG_PROB_ST2,
                    BIG_PROB_ST3,
                    BIG_PROB_ST4,
                    BIG_PROB_ST5,
                    BIG_PROB_ST6
                });

        // REG確率
        public static ReadOnlyCollection<float> RegProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    REG_PROB_ST1,
                    REG_PROB_ST2,
                    REG_PROB_ST3,
                    REG_PROB_ST4,
                    REG_PROB_ST5,
                    REG_PROB_ST6
                });

        // BIG CHANCE中ベル確率
        public static ReadOnlyCollection<float> BigBellProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    BIG_BELL_PROB_ST1,
                    BIG_BELL_PROB_ST2,
                    BIG_BELL_PROB_ST3,
                    BIG_BELL_PROB_ST4,
                    BIG_BELL_PROB_ST5,
                    BIG_BELL_PROB_ST6
                });
    }
}

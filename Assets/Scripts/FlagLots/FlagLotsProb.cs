using System.Collections.ObjectModel;

namespace ReelSpinGame_Lots.FlagProb
{
    public static class FlagLotsProb
    {
        // �e��t���O�m��
        // �{�[�i�X�m��

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

        // �ʏ��m�����̃t���O
        public const float CHERRY4_PROB_A = 256.6f;
        public const float MELON_PROB_A = 78.5f;
        public const float BELL_PROB_A = 72.5f;

        // �ʏ퍂�m�����̃t���O
        public const float CHERRY4_PROB_B = 256.6f;
        public const float MELON_PROB_B = 78.5f;
        public const float BELL_PROB_B = 72.5f;

        // ����ȊO�̒ʏ펞(�ϓ��Ȃ�)
        public const float CHERRY2_PROB = 4.8f;
        public const float REPLAY_PROB = 7.1f;


        // BIG CHANCE���̃t���O

        // �`�F���[(2��/4��)
        public const float BIG_CHERRY_PROB = 512.0f;

        public const float BIG_MELON_PROB = 4.8f;
        public const float BIG_JACIN_PROB = 3.2f;

        // BIG���x���m��

        public const float BIG_BELL_PROB_ST1 = 3.86f;
        public const float BIG_BELL_PROB_ST2 = 3.76f;
        public const float BIG_BELL_PROB_ST3 = 3.66f;
        public const float BIG_BELL_PROB_ST4 = 3.55f;
        public const float BIG_BELL_PROB_ST5 = 3.54f;
        public const float BIG_BELL_PROB_ST6 = 3.48f;

        // BONUS GAME���̂͂���m��
        public const float JAC_NONE_PROB = 256.0f;


        // BIG�m��
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

        // REG�m��
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

        // BIG CHANCE���x���m��
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

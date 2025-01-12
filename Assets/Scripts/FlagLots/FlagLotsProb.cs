using System.Collections.ObjectModel;

namespace ReelSpinGame_Lots.FlagProb
{
    public static class FlagLotsProb
    {
        // �e��t���O�m��
        // �{�[�i�X�m��

        // BIG CHANCE
        public const float BigProbST1 = 294.0f;
        public const float BigProbST2 = 285.0f;
        public const float BigProbST3 = 274.0f;
        public const float BigProbST4 = 254.0f;
        public const float BigProbST5 = 244.0f;
        public const float BigProbST6 = 240.0f;

        // BONUS GAME
        public const float RegProbST1 = 480.0f;
        public const float RegProbST2 = 471.0f;
        public const float RegProbST3 = 420.0f;
        public const float RegProbST4 = 415.0f;
        public const float RegProbST5 = 285.0f;
        public const float RegProbST6 = 260.0f;

        // �ʏ��m�����̃t���O
        public const float Cherry4ProbA = 256.6f;
        public const float MelonProbA = 78.5f;
        public const float BellProbA = 72.5f;

        // �ʏ퍂�m�����̃t���O
        public const float Cherry4ProbB = 16.8f;
        public const float MelonProbB = 25.5f;
        public const float BellProbB = 6.7f;

        // ����ȊO�̒ʏ펞(�ϓ��Ȃ�)
        public const float Cherry2Prob = 4.8f;
        public const float ReplayJACinProb = 7.1f;


        // BIG CHANCE���̃t���O

        // �`�F���[(2��/4��)
        public const float CherryProbInBig = 512.0f;

        public const float MelonProbInBig = 4.8f;
        public const float JACinProbInBig = 3.2f;

        // BIG���x���m��

        public const float BellProbInBigST1 = 3.86f;
        public const float BellProbInBigST2 = 3.76f;
        public const float BellProbInBigST3 = 3.66f;
        public const float BellProbInBigST4 = 3.55f;
        public const float BellProbInBigST5 = 3.54f;
        public const float BellProbInBigST6 = 3.48f;

        // BONUS GAME���̂͂���m��
        public const float JAC_NONE_PROB = 256.0f;


        // BIG�m��
        public static ReadOnlyCollection<float> BigProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    BigProbST1,
                    BigProbST2,
                    BigProbST3,
                    BigProbST4,
                    BigProbST5,
                    BigProbST6
                });

        // REG�m��
        public static ReadOnlyCollection<float> RegProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    RegProbST1,
                    RegProbST2,
                    RegProbST3,
                    RegProbST4,
                    RegProbST5,
                    RegProbST6
                });

        // BIG CHANCE���x���m��
        public static ReadOnlyCollection<float> BigBellProbability { get; }
            = new ReadOnlyCollection<float>(
                new[]
                {
                    BellProbInBigST1,
                    BellProbInBigST2,
                    BellProbInBigST3,
                    BellProbInBigST4,
                    BellProbInBigST5,
                    BellProbInBigST6
                });
    }
}

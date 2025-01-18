using System.Collections.ObjectModel;

namespace ReelSpinGame_Reels.Array
{
    public static class ReelArray
    {
        // ���[���z��

        // ���[���z��v�f��
        public const int MaxReelArray = 21;

        // �}��

        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay}

        // ���[���z��f�[�^(�ォ��1�Ԗ�)

        // �����[��
        public static ReadOnlyCollection<ReelSymbols> LeftReelArray { get; }
            = new ReadOnlyCollection<ReelSymbols>(
                new[]
                {
                    ReelSymbols.Melon,
                    ReelSymbols.Replay,
                    ReelSymbols.Bell,
                    ReelSymbols.Melon,
                    ReelSymbols.Replay,
                    ReelSymbols.Bell,
                    ReelSymbols.BlueSeven,
                    ReelSymbols.Cherry,
                    ReelSymbols.BAR,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.RedSeven,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.BAR,
                    ReelSymbols.Replay,
                    ReelSymbols.RedSeven,
                });

        // �����[��
        public static ReadOnlyCollection<ReelSymbols> MiddleReelArray { get; }
            = new ReadOnlyCollection<ReelSymbols>(
                new[]
                {
                    ReelSymbols.Cherry,
                    ReelSymbols.Bell,
                    ReelSymbols.Replay,
                    ReelSymbols.Cherry,
                    ReelSymbols.Melon,
                    ReelSymbols.Cherry,
                    ReelSymbols.Bell,
                    ReelSymbols.Replay,
                    ReelSymbols.BlueSeven,
                    ReelSymbols.Cherry,
                    ReelSymbols.Replay,
                    ReelSymbols.BAR,
                    ReelSymbols.Cherry,
                    ReelSymbols.Bell,
                    ReelSymbols.Replay,
                    ReelSymbols.BlueSeven,
                    ReelSymbols.Melon,
                    ReelSymbols.Cherry,
                    ReelSymbols.Bell,
                    ReelSymbols.Replay,
                    ReelSymbols.RedSeven,
                });

        // �E���[��
        public static ReadOnlyCollection<ReelSymbols> RightReelArray { get; }
            = new ReadOnlyCollection<ReelSymbols>(
                new[]
                {
                    ReelSymbols.Cherry,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.BAR,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.Cherry,
                    ReelSymbols.Replay,
                    ReelSymbols.Bell,
                    ReelSymbols.BlueSeven,
                    ReelSymbols.Cherry,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.BAR,
                    ReelSymbols.Replay,
                    ReelSymbols.Melon,
                    ReelSymbols.Bell,
                    ReelSymbols.RedSeven,
                });
    }
}

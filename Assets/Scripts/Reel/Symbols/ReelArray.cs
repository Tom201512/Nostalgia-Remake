using System.Collections.ObjectModel;

namespace ReelSpinGame_Reels.ReelArray
{
    public static class ReelArray
    {
        // リール配列

        // リール配列要素数
        public const int MaxReelArray = 21;

        // 図柄

        public enum ReelSymbols { RedSeven, BlueSeven, BAR, Cherry, Melon, Bell, Replay }

        // リール配列データ(上から1番目)

        // 左リール
        public static ReadOnlyCollection<ReelSymbols> LeftArray =
            new ReadOnlyCollection<ReelSymbols>(
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

        // 中リール
        public static ReadOnlyCollection<ReelSymbols> MiddleArray =
            new ReadOnlyCollection<ReelSymbols>(
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

        // 右リール

        public static ReadOnlyCollection<ReelSymbols> RightArray =
            new ReadOnlyCollection<ReelSymbols>(
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

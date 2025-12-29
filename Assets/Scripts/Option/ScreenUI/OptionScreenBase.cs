namespace ReelSpinGame_Option.MenuContent
{
    // オプション画面のインターフェース
    public interface IOptionScreenBase
    {
        // 画面を開いた時の挙動
        public void OpenScreen();

        // 画面を閉じた時の挙動
        public void CloseScreen();
    }

    public class OptionScreenFade
    {
        public const float FadeTime = 0.3f;     // フェード時間
    }
}

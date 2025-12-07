using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;

namespace ReelSpinGame_Effect.Data
{
    // 演出のデータ
    public interface IDoesEffect
    {
        // var
        public ReelEffectManager Reel { get; set; } // リール演出
        public FlashManager Flash { get; set; }     // フラッシュ
        public SoundManager Sound { get; set; }     // サウンド
    }

    // エフェクトのデータ
    public abstract class EffectData : IDoesEffect
    {
        // const

        // var
        public ReelEffectManager Reel { get; set; } // リール演出
        public FlashManager Flash { get; set; }     // フラッシュ
        public SoundManager Sound { get; set; }     // サウンド

        // コンストラクタ
        public EffectData(ReelEffectManager reelEffect, FlashManager flash, SoundManager sound)
        {
            Reel = reelEffect;
            Flash = flash;
            Sound = sound;
        }
    }
}


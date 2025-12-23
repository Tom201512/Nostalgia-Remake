using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Effect.Data
{
    // ボーナス中の演出
    public class BonusEffect : MonoBehaviour, IDoesEffect<BonusEffectCondition>
    {
        public bool HasEffect { get; set; }     // 演出処理中か
        SoundManager sound;                     // サウンド

        BigColor bigSymbolColor;       // 現在のBIG図柄色
        BonusStatus lastBonusStatus;    // 直近のボーナス状態

        void Awake()
        {
            bigSymbolColor = BigColor.None;
            lastBonusStatus = BonusStatus.BonusNone;
            HasEffect = false;
            sound = GetComponent<SoundManager>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // レバーオン時のエフェクト
        public void DoEffect(BonusEffectCondition bonusEffectCondition)
        {
            // BIG時の図柄色を登録
            bigSymbolColor = bonusEffectCondition.BigColor;
            // 前回とボーナス状態が変わっていればBGM再生(オート終了時も再生)
            if (lastBonusStatus != bonusEffectCondition.BonusStatus)
            {
                lastBonusStatus = bonusEffectCondition.BonusStatus;
                switch (lastBonusStatus)
                {
                    case BonusStatus.BonusBIGGames:
                        PlayBigGameBGM();
                        break;
                    case BonusStatus.BonusJACGames:
                        PlayBonusGameBGM();
                        break;
                    case BonusStatus.BonusNone:
                        sound.StopBGM();
                        break;
                }
            }
        }

        // 小役ゲーム中のBGM再生
        void PlayBigGameBGM()
        {
            switch (bigSymbolColor)
            {
                case BigColor.Red:
                    sound.PlayBGM(sound.SoundDB.BGM.RedBGM);
                    break;
                case BigColor.Blue:
                    sound.PlayBGM(sound.SoundDB.BGM.BlueBGM);
                    break;
                case BigColor.Black:
                    sound.PlayBGM(sound.SoundDB.BGM.BlackBGM);
                    break;
                default:
                    sound.PlayBGM(sound.SoundDB.BGM.RegJAC);
                    break;
            }
        }

        // ボーナスゲーム中のBGM再生
        void PlayBonusGameBGM()
        {
            switch (bigSymbolColor)
            {
                case BigColor.Red:
                    sound.PlayBGM(sound.SoundDB.BGM.RedJAC);
                    break;
                case BigColor.Blue:
                    sound.PlayBGM(sound.SoundDB.BGM.BlueJAC);
                    break;
                case BigColor.Black:
                    sound.PlayBGM(sound.SoundDB.BGM.BlackJAC);
                    break;
                default:
                    sound.PlayBGM(sound.SoundDB.BGM.RegJAC);
                    break;
            }
        }
    }
}
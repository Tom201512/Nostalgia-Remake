using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Lots;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect.Data
{
    // 払い出し前の演出
    public class BeforePayoutEffect : MonoBehaviour, IDoesEffect<BeforePayoutEffectCondition>
    {
        const float VFlashWaitTime = 2.0f;      // Vフラッシュ時の待機時間(秒)

        public bool HasEffect { get; set; } // 演出処理中か
        FlashManager flash;                 // リールフラッシュ
        SoundManager sound;                 // サウンド

        void Awake()
        {
            HasEffect = false;
            flash = GetComponent<FlashManager>();
            sound = GetComponent<SoundManager>();
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // レバーオン時のエフェクト
        public void DoEffect(BeforePayoutEffectCondition beforePayoutEffectCondition)
        {
            // 役ごとのフラッシュを発生
            switch (beforePayoutEffectCondition.Flag)
            {
                // BIG時、REG時, またはボーナス当選後のはずれのとき1/6でVフラッシュ発生
                case FlagID.FlagBig:
                case FlagID.FlagReg:
                case FlagID.FlagNone:
                    if (beforePayoutEffectCondition.HoldingBonus != BonusTypeID.BonusNone &&
                        OriginalRandomLot.LotRandomByNum(6))
                    {
                        flash.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        StartCoroutine(nameof(UpdateBeforePayoutEffect));
                    }
                    break;

                // チェリー2枚の場合
                case FlagID.FlagCherry2:
                // チェリー4枚の場合
                case FlagID.FlagCherry4:
                // ベルの場合:
                case FlagID.FlagBell:
                // スイカの場合
                case FlagID.FlagMelon:
                    break;

                // リプレイの場合
                case FlagID.FlagReplayJacIn:
                    // 小役ゲーム中にJACINが成立しビタハズシをした場合
                    if (beforePayoutEffectCondition.BonusStatus == BonusStatus.BonusBIGGames &&
                        (beforePayoutEffectCondition.LastLeftStoppedPos == 10 ||
                        beforePayoutEffectCondition.LastLeftStoppedPos == 16))
                    {
                        flash.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
                        StartCoroutine(nameof(UpdateBeforePayoutEffect));
                    }

                    break;

                default:
                    break;
            }
        }

        // 払い出し前演出の処理
        IEnumerator UpdateBeforePayoutEffect()
        {
            HasEffect = true;
            // 今鳴らしているジングルとフラッシュが止まるのを待つ
            while (!sound.GetSoundStopped() || !sound.GetJingleStopped() || flash.HasFlashWait)
            {
                yield return new WaitForEndOfFrame();
            }

            HasEffect = false;
        }
    }
}
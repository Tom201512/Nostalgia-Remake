using ReelSpinGame_Effect.Data.Condition;
using ReelSpinGame_Sound;
using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data
{
    // レバーオン時の演出
    public class LeverOnEffect : MonoBehaviour, IDoesEffect<LeverOnEffectCondition>
    {
        // var
        public bool HasEffect { get; }  // 演出処理中か
        SoundManager sound; // サウンド

        void Awake()
        {
            sound = GetComponent<SoundManager>();
        }

        // レバーオン時のエフェクト
        public void DoEffect(LeverOnEffectCondition leverOnEffectCondition)
        {
            // 通常時のみ特殊効果音再生
            if (leverOnEffectCondition.BonusStatus == BonusStatus.BonusNone)
            {
                // 以下の確率で告知音で再生(成立前)
                // BIG/REG成立時、成立後小役条件不問で1/4
                // スイカ、1/8
                // チェリー、発生しない
                // ベル、1/32
                // リプレイ、発生しない
                // はずれ、1/128

                if (leverOnEffectCondition.HoldingBonus == BonusTypeID.BonusNone)
                {
                    // BIG, REG
                    switch (leverOnEffectCondition.Flag)
                    {
                        case FlagID.FlagBig:
                        case FlagID.FlagReg:
                            LotStartSound(4);
                            break;

                        case FlagID.FlagMelon:
                            LotStartSound(8);
                            break;

                        case FlagID.FlagBell:
                            LotStartSound(32);
                            break;

                        case FlagID.FlagNone:
                            LotStartSound(128);
                            break;

                        default:
                            sound.PlaySE(sound.SoundDB.SE.Start);
                            break;
                    }
                }
                // 成立後は1/4で再生
                else
                {
                    LotStartSound(4);
                }

            }
            // その他の状態では鳴らさない
            else
            {
                sound.PlaySE(sound.SoundDB.SE.Start);
            }
        }

        // 指定した確率で再生音の抽選をする
        void LotStartSound(int probability)
        {
            // 確率が0以下は通常スタート音
            if (probability <= 0)
            {
                sound.PlaySE(sound.SoundDB.SE.Start);
            }
            // 確率が1以上なら抽選
            else if (OriginalRandomLot.LotRandomByNum(probability))
            {
                //Debug.Log("SP SOUND PLAYED");
                sound.PlaySE(sound.SoundDB.SE.SpStart);
            }
            else
            {
                sound.PlaySE(sound.SoundDB.SE.Start);
            }
        }
    }
}

using ReelSpinGame_Reels.Effect;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using ReelSpinGame_Util.OriginalInputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;

namespace ReelSpinGame_Effect.Data
{
    // レバーオン時の演出
    public class LeverOnEffect : EffectData
    {
        public LeverOnEffect(ReelEffectManager reelEffect, FlashManager flash, SoundManager sound) : 
            base(reelEffect, flash, sound) 
        { 

        }

        public void StartLeverOnEffect(FlagID flag, BonusTypeID holding, BonusStatus bonusStatus)
        {
            // 通常時のみ特殊効果音再生
            if (bonusStatus == BonusStatus.BonusNone)
            {
                // 以下の確率で告知音で再生(成立前)
                // BIG/REG成立時、成立後小役条件不問で1/4
                // スイカ、1/8
                // チェリー、発生しない
                // ベル、1/32
                // リプレイ、発生しない
                // はずれ、1/128

                if (holding == BonusTypeID.BonusNone)
                {
                    // BIG, REG
                    switch (flag)
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
                            Sound.PlaySE(Sound.SoundDB.SE.Start);
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
                Sound.PlaySE(Sound.SoundDB.SE.Start);
            }
        }

        // 指定した確率で再生音の抽選をする
        private void LotStartSound(int probability)
        {
            // 確率が0以下は通常スタート音
            if (probability <= 0)
            {
                Sound.PlaySE(Sound.SoundDB.SE.Start);
            }
            // 確率が1以上なら抽選
            else if (OriginalRandomLot.LotRandomByNum(probability))
            {
                //Debug.Log("SP SOUND PLAYED");
                Sound.PlaySE(Sound.SoundDB.SE.SpStart);
            }
            else
            {
                Sound.PlaySE(Sound.SoundDB.SE.Start);
            }
        }
    }
}

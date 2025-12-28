using ReelSpinGame_Lots;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;

namespace ReelSpinGame_AutoPlay.AI
{
    // オートプレイ時のAI
    public class AutoPlayAI
    {
        public bool HasTechnicalPlay { get; set; }                  // 技術介入をするか
        public bool HasWinningPatternStop { get; set; }             // リーチ目を止める制御を取るか
        public bool HasStoppedWinningPattern { get; set; }          // リーチ目を止めたか
        public BigColor BigLineUpSymbol { get; set; }              // 揃えるBIG色

        // 使用AI
        AutoRandomAI autoRandomAI;                              // 適当押し
        AutoNonLeftFirstReplayAI autoNonLeftFirstReplayAI;      // 変則押しリプレイ狙い
        AutoNormalBellAI autoNormalBellAI;                      // 通常時ベル
        AutoNonLeftFirstBellAI autoNonLeftFirstBellAI;          // 変則押し時ベル
        AutoMelonAI autoMelonAI;                                // スイカ時
        AutoCherryAI autoCherryAI;                              // チェリー時
        AutoNormalBonusGameAI autoNormalBonusGameAI;            // REG(3枚掛け)
        AutoOneBetBonusGameAI autoOneBetBonusGameAI;            // REG(1枚掛け)
        AutoNormalRedBigAI autoNormalRedBigAI;                  // 赤7
        AutoNormalBlueBigAI autoNormalBlueBigAI;                // 青7
        AutoNormalBB7BigAI autoNormalBB7BigAI;                  // BB7(3枚掛け)
        AutoOneBetBB7BigAI autoOneBetBB7BigAI;                  // BB7(1枚掛け, 4枚チェリー)
        AutoJacAvoidAI autoJacAvoidAI;                          // JACハズシ

        public AutoPlayAI()
        {
            HasTechnicalPlay = true;
            HasWinningPatternStop = false;
            HasStoppedWinningPattern = false;
            BigLineUpSymbol = BigColor.None;

            autoRandomAI = new AutoRandomAI();
            autoNonLeftFirstReplayAI = new AutoNonLeftFirstReplayAI();
            autoNormalBellAI = new AutoNormalBellAI();
            autoNonLeftFirstBellAI = new AutoNonLeftFirstBellAI();
            autoMelonAI = new AutoMelonAI();
            autoCherryAI = new AutoCherryAI();
            autoNormalBonusGameAI = new AutoNormalBonusGameAI();
            autoOneBetBonusGameAI = new AutoOneBetBonusGameAI();
            autoNormalRedBigAI = new AutoNormalRedBigAI();
            autoNormalBlueBigAI = new AutoNormalBlueBigAI();
            autoNormalBB7BigAI = new AutoNormalBB7BigAI();
            autoOneBetBB7BigAI = new AutoOneBetBB7BigAI();
            autoJacAvoidAI = new AutoJacAvoidAI();
        }

        // 停止位置を小役、第一停止、現在の状態に合わせて返す
        public int[] GetStopPos(AutoAIConditionClass autoAIConditions)
        {
            // 技術介入ありの場合
            if (HasTechnicalPlay)
            {
                // 小役ごとに挙動を変える
                switch (autoAIConditions.Flag)
                {
                    // BIG時(リーチ目を止める場合は適当押しをする)
                    case FlagID.FlagBig:
                        if (HasWinningPatternStop && !HasStoppedWinningPattern)
                        {
                            HasStoppedWinningPattern = true;
                            return autoRandomAI.SendStopPosData();
                        }
                        else
                        {
                            return AimBigChance(autoAIConditions.Flag, autoAIConditions.BetAmount);
                        }

                    // REG時(リーチ目を止める場合は適当押しをする)
                    case FlagID.FlagReg:
                        if (HasWinningPatternStop && !HasStoppedWinningPattern)
                        {
                            HasStoppedWinningPattern = true;
                            return autoRandomAI.SendStopPosData();
                        }
                        else
                        {
                            if (autoAIConditions.BetAmount == 1)
                            {
                                return autoOneBetBonusGameAI.SendStopPosData();
                            }
                            else
                            {
                                return autoNormalBonusGameAI.SendStopPosData();
                            }
                        }

                    // チェリー時
                    case FlagID.FlagCherry2:
                    case FlagID.FlagCherry4:
                        return AICherryBehavior(autoAIConditions.Flag, autoAIConditions.HoldingBonus, autoAIConditions.BetAmount);

                    // スイカ時
                    case FlagID.FlagMelon:
                        return autoMelonAI.SendStopPosData();

                    // ベル時
                    case FlagID.FlagBell:
                        // 変則押しかどうかで制御を変える
                        if (autoAIConditions.FirstPush != ReelID.ReelLeft || autoAIConditions.RemainingJacIn == 1)
                        {
                            return autoNonLeftFirstBellAI.SendStopPosData();
                        }
                        else
                        {
                            return autoNormalBellAI.SendStopPosData();
                        }

                    // リプレイ時(BIG中はJAC-INまたはハズシ)
                    case FlagID.FlagReplayJacIn:
                        // 残りJACが1回の場合はJACハズシをする(残りゲーム数が8回になるまで)
                        if (autoAIConditions.RemainingJacIn == 1 && autoAIConditions.BigChanceGames > 8)
                        {
                            return autoJacAvoidAI.SendStopPosData();
                        }
                        else if (autoAIConditions.FirstPush != ReelID.ReelLeft)
                        {
                            return autoNonLeftFirstReplayAI.SendStopPosData();
                        }
                        else
                        {
                            return autoRandomAI.SendStopPosData();
                        }

                    // はずれ、JAC中などははずれ制御
                    default:
                        return AINoneBehavior(autoAIConditions.HoldingBonus, autoAIConditions.BetAmount);
                }
            }

            // ない場合は適当押しをする
            else
            {
                return autoRandomAI.SendStopPosData();
            }
        }

        // はずれ時
        private int[] AINoneBehavior(BonusTypeID holdingBonus, int betAmount)
        {
            // リーチ目で止める設定があれば、リーチ目を止めたことにする
            if (HasWinningPatternStop && holdingBonus != BonusTypeID.BonusNone)
            {
                HasStoppedWinningPattern = true;
            }
            // ボーナスがある場合はそのボーナスを狙うように
            if (holdingBonus == BonusTypeID.BonusBIG)
            {
                return AimBigChance(FlagID.FlagNone, betAmount);
            }

            // REGの場合
            else if (holdingBonus == BonusTypeID.BonusREG)
            {
                if (betAmount == 1)
                {
                    return autoOneBetBonusGameAI.SendStopPosData();
                }
                else
                {
                    return autoNormalBonusGameAI.SendStopPosData();
                }
            }

            // はずれ時は適当押しをする
            return autoRandomAI.SendStopPosData();
        }

        // チェリー時
        private int[] AICherryBehavior(FlagID flag, BonusTypeID holdingBonus, int betAmount)
        {
            // ボーナスがある場合はそのボーナスを狙うように
            if (holdingBonus == BonusTypeID.BonusBIG)
            {
                return AimBigChance(flag, betAmount);
            }
            // REGの場合
            else if (holdingBonus == BonusTypeID.BonusREG)
            {
                if (flag == FlagID.FlagCherry4 || betAmount == 1)
                {
                    return autoOneBetBonusGameAI.SendStopPosData();
                }
                else
                {
                    return autoNormalBonusGameAI.SendStopPosData();
                }
            }
            // 何もなければチェリー狙い
            else
            {
                return autoCherryAI.SendStopPosData();
            }
        }

        // ビッグチャンスを狙う
        private int[] AimBigChance(FlagID flag, int betAmount)
        {
            int colorID = 0;

            // 揃えるBIGの色指定がなければランダムで選択
            if (BigLineUpSymbol == BigColor.None)
            {
                colorID = Random.Range((int)BigColor.Red, (int)BigColor.Black + 1);
            }
            else
            {
                colorID = (int)BigLineUpSymbol;
            }

            switch (colorID)
            {
                case (int)BigColor.Red:
                    return autoNormalRedBigAI.SendStopPosData();

                case (int)BigColor.Blue:
                    return autoNormalBlueBigAI.SendStopPosData();

                case (int)BigColor.Black:
                    // 4枚チェリーまたは1枚掛け
                    if (flag == FlagID.FlagCherry4 || betAmount == 1)
                    {
                        return autoOneBetBB7BigAI.SendStopPosData();
                    }
                    else
                    {
                        return autoNormalBB7BigAI.SendStopPosData();
                    }
                default:
                    return autoRandomAI.SendStopPosData();
            }
        }
    }
}
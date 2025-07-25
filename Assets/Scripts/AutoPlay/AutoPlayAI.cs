﻿using ReelSpinGame_Util.OriginalInputs;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_AutoPlay.AI
{
	// オートプレイ時のAI
	public class AutoPlayAI
	{
		// func
		// 停止位置を小役、第一停止、現在の状態に合わせて返す
		public int[] GetStopPos(FlagId flag, ReelID firstPush, BonusType holdingBonus, int bigChanceGames, int remainingJacIn)
		{
			// 小役ごとに挙動を変える
            switch(flag)
            {
                // BIG時
                case FlagId.FlagBig:
                    return AimBigChance();

                // REG時
                case FlagId.FlagReg:
                    return AimBonusGame();

                // チェリー時
                case FlagId.FlagCherry2:
                case FlagId.FlagCherry4:
                    return AICherryBehavior(holdingBonus);

                // スイカ時
                case FlagId.FlagMelon:
                    return AimMelon();

                // ベル時(BIG中は変則押し
                case FlagId.FlagBell:
                    return AimBell(firstPush, remainingJacIn);

                // リプレイ時(BIG中はJAC-INまたはハズシ)
                case FlagId.FlagReplayJacIn:
                    return AimReplay(bigChanceGames, remainingJacIn);

                // はずれ、JAC中などははずれ制御
                default:
                    return AINoneBehavior(holdingBonus);
            }
		}

		// はずれ時
		private int[] AINoneBehavior(BonusType holdingBonus)
		{
            // ボーナスがある場合はそのボーナスを狙うように
            if (holdingBonus == BonusType.BonusBIG)
			{
                return AimBigChance();
			}

            // REGの場合
            else if(holdingBonus == BonusType.BonusREG)
            {
                return AimBonusGame();
            }

            // はずれ時は適当押しをする
            else
            {
                return AimRandomly();
            }
        }

        // チェリー時
        private int[] AICherryBehavior(BonusType holdingBonus)
        {
            // ボーナスがある場合はそのボーナスを狙うように
            if (holdingBonus == BonusType.BonusBIG)
            {
                return AimBigChance();
            }

            // REGの場合
            else if (holdingBonus == BonusType.BonusREG)
            {
                return AimBonusGame();
            }

            // はずれ時はチェリー狙い
            else
            {
                return AimCherry();
            }
        }

        // ビッグチャンスを狙う
        private int[] AimBigChance()
		{
            int[] stopPos = new int[] { 0, 0, 0 };

            // 1/3で赤7, 青7, BB7のいずれかを選択(左制御のみ)
            switch (Random.Range((int)BigColor.Red, (int)BigColor.Black + 1))
            {
                case (int)BigColor.Red:

                    // 1/2でチェリーのない赤7を選択
                    if (OriginalRandomLot.LotRandomByNum(2))
                    {
                        stopPos[(int)ReelID.ReelLeft] = 10;
                    }
                    else
                    {
                        stopPos[(int)ReelID.ReelLeft] = 18;
                    }

                    stopPos[(int)ReelID.ReelMiddle] = 18;
                    stopPos[(int)ReelID.ReelRight] = 18;
                    break;

                case (int)BigColor.Blue:

                    stopPos[(int)ReelID.ReelLeft] = 3;

                    // 1/2でBAR上の青7を停止させる
                    if (OriginalRandomLot.LotRandomByNum(2))
                    {
                        stopPos[(int)ReelID.ReelMiddle] = 6;
                    }
                    else
                    {
                        stopPos[(int)ReelID.ReelMiddle] = 13;
                    }
                    stopPos[(int)ReelID.ReelRight] = 9;

                    break;
                case (int)BigColor.Black:

                    // 1/2で上にチェリーのあるBARを停止
                    if (OriginalRandomLot.LotRandomByNum(2))
                    {
                        stopPos[(int)ReelID.ReelLeft] = 8;
                    }
                    else
                    {
                        stopPos[(int)ReelID.ReelLeft] = 16;
                    }

                    stopPos[(int)ReelID.ReelMiddle] = 9;
                    stopPos[(int)ReelID.ReelRight] = 18;
                    break;
            }

            return stopPos;
        }

        // ボーナスゲーム図柄を狙う
        private int[] AimBonusGame()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // 1/2で上にチェリーのあるBARを停止
            if (OriginalRandomLot.LotRandomByNum(2))
            {
                stopPos[(int)ReelID.ReelLeft] = 8;
            }
            else
            {
                stopPos[(int)ReelID.ReelLeft] = 16;
            }

            stopPos[(int)ReelID.ReelMiddle] = 9;

            // 1/2で狙うBARを変える
            if (OriginalRandomLot.LotRandomByNum(2))
            {
                stopPos[(int)ReelID.ReelRight] = 2;
            }
            else
            {
                stopPos[(int)ReelID.ReelRight] = 14;
            }

            return stopPos;
        }

        // チェリーを狙う(通常時)
        private int[] AimCherry()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // 1/2で狙うチェリーを変える
            if (OriginalRandomLot.LotRandomByNum(2))
            {
                stopPos[(int)ReelID.ReelLeft] = 4;
            }
            else
            {
                stopPos[(int)ReelID.ReelLeft] = 16;
            }

            // その他は適当押し

            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }

        // スイカを狙う
        private int[] AimMelon()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // スイカを狙う候補を配列に(全て上段を狙う)
            int[] melonPosL = new int[] { 1, 8, 11, 14, 20 };
            int[] melonPosM = new int[] { 2, 14};
            int[] melonPosR = new int[] {0, 4, 12, 16};

            // 候補に入れた数字からランダムで狙う
            stopPos[(int)ReelID.ReelLeft] = melonPosL[Random.Range(0, melonPosL.Length)];
            stopPos[(int)ReelID.ReelMiddle] = melonPosM[Random.Range(0, melonPosM.Length)];
            stopPos[(int)ReelID.ReelRight] = melonPosR[Random.Range(0, melonPosR.Length)];

            return stopPos;
        }

        // ベルを狙う
        private int[] AimBell(ReelID firstPushReel, int remainingJacIn)
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            // ベルを狙う候補を配列に(中リールのみ)
            int[] bellPosM = new int[] {4,11,16,20};

            // 左は第一停止が左でない場合、常に12番のベルを狙う(下段受け対策)
            // BIG中はJACIN回数が1回の場合に狙う

            if (firstPushReel != ReelID.ReelLeft || remainingJacIn == 1)
            {
                stopPos[(int)ReelID.ReelLeft] = 11;
            }
            else
            {
                stopPos[(int)ReelID.ReelLeft] = Random.Range(0, MaxReelArray);
            }

            // 中は取りこぼし位置があるので候補から狙う
            stopPos[(int)ReelID.ReelMiddle] = bellPosM[Random.Range(0, bellPosM.Length)];

            // 右は取りこぼさないのでランダム
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }

        // リプレイを狙う
        private int[] AimReplay(int bigChanceGames, int remainingJac)
        {
            int[] stopPos = new int[] { 0, 0, 0 };
            // 基本は適当押しだが、JACの残り回数、BIGの残り回数ではJACハズシをする

            // 残りJACが1回の場合はJACハズシをする(残りゲーム数が8回になるまで)
            if (remainingJac == 1 && bigChanceGames > 8)
            {
                // 1/2で狙う位置を変える
                if (OriginalRandomLot.LotRandomByNum(2))
                {
                    stopPos[(int)ReelID.ReelLeft] = 10;
                }
                else
                {
                    stopPos[(int)ReelID.ReelLeft] = 16;
                }
            }
            else
            {
                stopPos[(int)ReelID.ReelLeft] = Random.Range(0, MaxReelArray);
            }

            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }

        // 適当押し
        private int[] AimRandomly()
        {
            int[] stopPos = new int[] { 0, 0, 0 };

            stopPos[(int)ReelID.ReelLeft] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelMiddle] = Random.Range(0, MaxReelArray);
            stopPos[(int)ReelID.ReelRight] = Random.Range(0, MaxReelArray);

            return stopPos;
        }
	}
}
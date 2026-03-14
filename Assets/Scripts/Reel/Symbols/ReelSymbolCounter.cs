using ReelSpinGame_Bonus;
using ReelSpinGame_Scriptable;
using System;
using UnityEngine;

namespace ReelSpinGame_Reel.Symbol
{
    // 図柄カウント
    public class ReelSymbolCounter : MonoBehaviour
    {
        [SerializeField] ReelGroupAccessor reelGroup;               // リールオブジェクトのグループ
        [SerializeField] PayoutDatabase payoutDatabase;             // 払い出しのデータ

        // 揃っているBIG図柄の数を返す
        public BonusModel.BigType GetBigLinedUpCount(int betAmount, int checkAmount)
        {
            // 赤7
            if (CountBonusSymbols(BonusModel.BigType.Red, betAmount) == checkAmount)
            {
                return BonusModel.BigType.Red;
            }
            // 青7
            if (CountBonusSymbols(BonusModel.BigType.Blue, betAmount) == checkAmount)
            {
                return BonusModel.BigType.Blue;
            }
            // BB7
            if (CountBonusSymbols(BonusModel.BigType.BB7, betAmount) == checkAmount)
            {
                return BonusModel.BigType.BB7;
            }

            return BonusModel.BigType.None;
        }

        // 指定したビッグチャンス図柄がいくつ揃っているか確認する
        private int CountBonusSymbols(BonusModel.BigType bigType, int betAmount)
        {
            // ボーナス図柄の数
            int bonusSymbolCount = 0;
            // 払い出しラインとベット枚数から確認
            foreach (PayoutLineData payoutLine in payoutDatabase.PayoutLines)
            {
                // ベット条件を満たしているか確認
                if (betAmount >= payoutLine.BetCondition)
                {
                    bonusSymbolCount = CountBonus(payoutLine, bigType);
                }
            }
            return bonusSymbolCount;
        }

        // ボーナスごとに図柄をカウントする
        private int CountBonus(PayoutLineData payoutLine, BonusModel.BigType bigType)
        {
            // 最も多かったカウント
            int currentCount = 0;

            // 停止状態になっている停止予定位置のリールからリーチ状態か確認
            foreach (ReelID reelID in Enum.GetValues(typeof(ReelID)))
            {
                if (reelGroup.GetReelStatus(reelID) != ReelStatus.Stopped)
                {
                    continue;
                }

                switch(bigType)
                {
                    case BonusModel.BigType.Red:
                        if (reelGroup.GetSymbolFromWillStop(reelID, payoutLine.PayoutLines[(int)reelID]) == ReelSymbols.RedSeven)
                        {
                            currentCount += 1;
                        }
                        break;

                    case BonusModel.BigType.Blue:
                        if (reelGroup.GetSymbolFromWillStop(reelID, payoutLine.PayoutLines[(int)reelID]) == ReelSymbols.BlueSeven)
                        {
                            currentCount += 1;
                        }
                        break;

                    case BonusModel.BigType.BB7:
                        // 右リールは赤7があるか確認
                        if (reelID == ReelID.ReelRight &&
                        reelGroup.GetSymbolFromWillStop(reelID, payoutLine.PayoutLines[(int)reelID]) == ReelSymbols.RedSeven)
                        {
                            currentCount += 1;
                        }
                        // 他のリールはBARがあるか確認
                        else if (reelID != ReelID.ReelRight && 
                            reelGroup.GetSymbolFromWillStop(reelID, payoutLine.PayoutLines[(int)reelID]) == ReelSymbols.BAR)
                        {
                            currentCount += 1;
                        }
                        break;
                }
            }

            return currentCount;
        }
    }
}
using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Reels.Symbol
{
    // 図柄カウント
    public class ReelSymbolCounter : MonoBehaviour
    {
        [SerializeField] List<ReelObjectPresenter> reelObjects;     // リールオブジェクト
        [SerializeField] PayoutDatabase payoutDatabase;             // 払い出しのデータ

        // 揃っているBIG図柄の数を返す
        public BigColor GetBigLinedUpCount(int betAmount, int checkAmount)
        {
            // 赤7
            if (CountBonusSymbols(BigColor.Red, betAmount) == checkAmount)
            {
                return BigColor.Red;
            }

            // 青7
            if (CountBonusSymbols(BigColor.Blue, betAmount) == checkAmount)
            {
                return BigColor.Blue;
            }

            // BB7
            if (CountBonusSymbols(BigColor.Black, betAmount) == checkAmount)
            {
                return BigColor.Black;
            }

            return BigColor.None;
        }

        // ビッグチャンス図柄がいくつ揃っているか確認する
        int CountBonusSymbols(BigColor bigColor, int betAmount)
        {
            int highestCount = 0;   // 最も多かった個数を記録
            // 払い出しラインとベット枚数から確認
            foreach (PayoutLineData line in payoutDatabase.PayoutLines)
            {
                // ベット条件を満たしているか確認
                if (betAmount >= line.BetCondition)
                {
                    int currentCount = 0;
                    // 停止状態になっている停止予定位置のリールからリーチ状態か確認
                    for (int i = 0; i < reelObjects.Count; i++)
                    {
                        if (reelObjects[i].ReelStatus != ReelStatus.Stopped)
                        {
                            continue;
                        }

                        // 赤7
                        if (bigColor == BigColor.Red &&
                            reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.RedSeven)
                        {
                            currentCount += 1;
                        }
                        // 青7
                        if (bigColor == BigColor.Blue &&
                            reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BlueSeven)
                        {
                            currentCount += 1;
                        }

                        // BB7
                        if (bigColor == BigColor.Black)
                        {
                            // 右リールは赤7があるか確認
                            if (i == (int)ReelID.ReelRight &&
                                reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.RedSeven)
                            {
                                currentCount += 1;
                            }
                            // 他のリールはBARがあるか確認
                            else if (i != (int)ReelID.ReelRight &&
                                reelObjects[i].GetSymbolFromWillStop(line.PayoutLines[i]) == ReelSymbols.BAR)
                            {
                                currentCount += 1;
                            }
                        }
                    }
                    // 最も多かったカウントを記録
                    if (currentCount > highestCount)
                    {
                        highestCount = currentCount;
                    }
                }
            }
            return highestCount;
        }
    }
}
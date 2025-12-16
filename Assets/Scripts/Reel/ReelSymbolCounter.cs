using ReelSpinGame_Datas;
using ReelSpinGame_Payout;
using System.Collections.Generic;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Reels.ReelObjectPresenter;
using static ReelSpinGame_Reels.Spin.ReelSpinModel;

namespace ReelSpinGame_Reels.Symbol
{
    // 図柄カウント
    public class ReelSymbolCounter
    {
        // ビッグチャンス図柄がいくつ揃っているか確認する
        public int CountBonusSymbols(BigColor bigColor, int betAmount, List<ReelObjectPresenter> reelObjects, List<PayoutLineData> payoutLines)
        {
            int highestCount = 0;   // 最も多かった個数を記録
            // 払い出しラインとベット枚数から確認
            foreach (PayoutLineData line in payoutLines)
            {
                // ベット条件を満たしているか確認
                if (betAmount >= line.BetCondition)
                {
                    int currentCount = 0;
                    // 停止状態になっている停止予定位置のリールからリーチ状態か確認
                    for (int i = 0; i < reelObjects.Count; i++)
                    {
                        if (reelObjects[i].GetCurrentReelStatus() != ReelStatus.Stopped)
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
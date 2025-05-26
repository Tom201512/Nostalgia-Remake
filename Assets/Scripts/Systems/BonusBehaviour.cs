using ReelSpinGame_Datas;
using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Bonus
{
	public class BonusBehaviour
	{
        // ボーナスの処理
        // const

        // ボーナスの種類
        public enum BonusType { BonusNone, BonusBIG, BonusREG }

        // ボーナスの状態
        public enum BonusStatus { BonusNone, BonusBIGGames, BonusJACGames };

        // BIGボーナスで当選した色
        public enum BigColor {None, Red, Blue, Black};

        // 残り小役ゲーム数
        public const int BigGames = 30;
        // 残りJACIN
        public const int JacInTimes = 3;

        // JACゲーム中
        // 残りJACゲーム数
        public const int JacGames = 12;
        // 残り当選回数
        public const int JacHits = 8;

        // var
        // 現在ストックしているボーナス
        public BonusType HoldingBonusID { get; private set; }
        // ボーナス状態
        public BonusStatus CurrentBonusStatus { get; private set; }
        // BIGボーナス当選時の色
        public BigColor BigBonusColor { get; private set; }

        // 残りゲーム数、当選回数(JAC-INまたはJAC役)

        // 小役ゲーム中
        // 残り小役ゲーム数
        public int RemainingBigGames { get; private set; }
        // 残りJACIN
        public int RemainingJacIn { get; private set; }

        // JACゲーム中
        // 残りJACゲーム数
        public int RemainingJacGames { get; private set; }
        // 残り当選回数
        public int RemainingJacHits { get; private set; }

        // コンストラクタ
        public BonusBehaviour()
        {
            HoldingBonusID = BonusType.BonusNone;
            CurrentBonusStatus = BonusStatus.BonusNone;
            BigBonusColor = BigColor.None;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
        }

        // func
        // ボーナス情報を読み込む
        public void SetBonusData(BonusType holdingBonusID, BonusStatus bonusStatus, int remainingBIGGames, int remainingJACIN,
            int remainingJACGames, int remainingJACHits)
        {
            HoldingBonusID = holdingBonusID;
            CurrentBonusStatus = bonusStatus;
            RemainingBigGames = remainingBIGGames;
            RemainingJacIn = remainingJACIN;
            RemainingJacGames = remainingJACGames;
            RemainingJacHits = remainingJACHits;
        }
        // ボーナスストック状態の更新
        public void SetBonusStock(BonusType bonusType) => HoldingBonusID = bonusType;

        public void StartBigChance(List<PayoutLineData> lastPayoutLines, LastStoppedReelData lastStopped)
        {
            Debug.Log("BIG CHANCE start");
            RemainingBigGames = BigGames;
            RemainingJacIn = JacInTimes;
            CurrentBonusStatus = BonusStatus.BonusBIGGames;
            HoldingBonusID = BonusType.BonusNone;
            CheckBonusColor(lastPayoutLines, lastStopped);
        }

        public void StartBonusGame()
        {
            if (RemainingJacIn > 0)
            {
                RemainingJacIn -= 1;
            }
            Debug.Log("BONUS GAME start");
            RemainingJacGames = JacGames;
            RemainingJacHits = JacHits;
            CurrentBonusStatus = BonusStatus.BonusJACGames;
            HoldingBonusID = BonusType.BonusNone;
        }

        public void DecreaseGames()
        {
            if (CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                RemainingBigGames -= 1;
            }
            else if (CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                RemainingJacGames -= 1;
            }
        }

        // 小役ゲーム中の状態遷移
        public void CheckBigGameStatus(bool hasJacIn)
        {
            // JAC-INなら
            if (hasJacIn)
            {
                StartBonusGame();
            }

            // 30ゲームを消化した場合
            else if (RemainingBigGames == 0)
            {
                Debug.Log("BIG CHANCE end");
                EndBonusStatus();
            }
        }

        // ボーナスゲームの状態遷移
        public void CheckBonusGameStatus(bool hasPayout)
        {
            if (hasPayout)
            {
                RemainingJacHits -= 1;
            }

            // JACゲーム数が0, または入賞回数が0の場合は終了
            if (RemainingJacGames == 0 || RemainingJacHits == 0)
            {
                Debug.Log("End Bonus Game");

                // BIG中なら残りJAC-INの数があれば小役ゲームへ移行
                if (RemainingJacIn > 0)
                {
                    CurrentBonusStatus = BonusStatus.BonusBIGGames;
                }
                else
                {
                    EndBonusStatus();
                }
            }
        }

        public void EndBonusStatus()
        {
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacGames = 0;
            RemainingJacHits = 0;
            CurrentBonusStatus = BonusStatus.BonusNone;
            Debug.Log("Bonus Reset");
        }

        private void CheckBonusColor(List<PayoutLineData> lastPayoutLines, LastStoppedReelData lastStopped)
        {
            BigBonusColor = BigColor.None;

            // 払い出しラインからボーナスが何色だったかを得る
            foreach (PayoutLineData payoutLine in lastPayoutLines)
            {
                int redCount = 0;
                int blueCount = 0;
                int barCount = 0;

                for (int i = 0; i < payoutLine.PayoutLines.Count; i++)
                {
                    if(lastStopped.GetLastStoppedSymbol(i, payoutLine.PayoutLines[i]) == ReelData.ReelSymbols.RedSeven)
                    {
                        redCount += 1;
                        Debug.Log("Red:" + redCount);
                    }
                    if (lastStopped.GetLastStoppedSymbol(i, payoutLine.PayoutLines[i]) == ReelData.ReelSymbols.BlueSeven)
                    {
                        blueCount += 1;
                        Debug.Log("Blue:" + blueCount);
                    }
                    if (lastStopped.GetLastStoppedSymbol(i, payoutLine.PayoutLines[i]) == ReelData.ReelSymbols.BAR)
                    {
                        barCount += 1;
                        Debug.Log("BAR:" + barCount);
                    }
                }

                // 赤7揃いの場合
                if(redCount == 3)
                {
                    BigBonusColor = BigColor.Red;
                }
                // 青7揃いの場合
                else if(blueCount == 3)
                {
                    BigBonusColor = BigColor.Blue;
                }
                // BB7揃いの場合
                else if(redCount == 1 && barCount == 2)
                {
                    BigBonusColor = BigColor.Black;
                }
            }

            Debug.Log("Bonus Color:" + BigBonusColor);
        }

        public void ResetBonusColor() => BigBonusColor = BigColor.None;
    }
}
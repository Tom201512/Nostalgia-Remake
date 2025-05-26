using UnityEngine;
using System.Collections;

namespace ReelSpinGame_Bonus
{
	public class BonusBehaviour
	{
        // ボーナスの処理
        // const
        public enum BonusType { BonusNone, BonusBIG, BonusREG }
        public enum BonusStatus { BonusNone, BonusBIGGames, BonusJACGames };

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
            HoldingBonusID = (int)BonusType.BonusNone;
            CurrentBonusStatus = (int)BonusStatus.BonusNone;
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

        public void StartBigChance()
        {
            Debug.Log("BIG CHANCE start");
            RemainingBigGames = BigGames;
            RemainingJacIn = JacInTimes;
            CurrentBonusStatus = BonusStatus.BonusBIGGames;
            HoldingBonusID = BonusType.BonusNone;
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
    }
}
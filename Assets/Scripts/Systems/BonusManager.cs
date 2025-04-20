using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Bonus
{
    public class BonusManager
    {
        // ボーナスのデータ

        // const
        public enum BonusType {BonusNone, BonusBIG, BonusREG }
        public enum BonusStatus {BonusNone, BonusBIGGames, BonusJACGames};

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
        public BonusManager()
        {
            HoldingBonusID = (int)BonusType.BonusNone;
            CurrentBonusStatus = (int)BonusStatus.BonusNone;
            RemainingBigGames = 0;
            RemainingJacIn = 0;
            RemainingJacHits = 0;
            RemainingJacGames = 0;
        }

        // ファイルを読み込む場合
        public BonusManager(BonusType holdingBonusID, BonusStatus bonusStatus, int remainingBIGGames, int remainingJACIN, 
            int remainingJACGames, int remainingJACHits)
        {
            HoldingBonusID = holdingBonusID;
            CurrentBonusStatus = bonusStatus;
            RemainingBigGames = remainingBIGGames;
            RemainingJacIn = remainingJACIN;
            RemainingJacGames = remainingJACGames;
            RemainingJacHits = remainingJACHits;
        }

        // func
        public void StartBigChance()
        {
            Debug.Log("BIG CHANCE start");
            RemainingBigGames = BigGames;
            RemainingJacIn = JacInTimes;
            CurrentBonusStatus = BonusStatus.BonusBIGGames;
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
        }

        // 小役ゲームを減らす
        public void DecreaseBigGames(bool hasJacIn)
        {
            RemainingBigGames -= 1;

            // JAC-INなら
            if (hasJacIn)
            {
                RemainingJacIn -= 1;
                StartBonusGame();
            }

            // 30ゲームを消化した場合
            else if (RemainingBigGames == 0)
            {
                Debug.Log("BIG CHANCE end");
                CurrentBonusStatus = BonusStatus.BonusNone;
            }
        }

        // ボーナスゲーム数を減らす
        public void DecreaseBonusGames(bool hasPayout)
        {
            RemainingJacGames -= 1;

            if (hasPayout)
            {
                RemainingJacHits -= 1;
            }

            // JACゲーム数が0, または入賞回数が0の場合は終了
            if (RemainingJacGames == 0 || RemainingJacHits == 0)
            {
                Debug.Log("End Bonus Game");
                CurrentBonusStatus = BonusStatus.BonusBIGGames;

                // BIG中ならJAC-INが0の場合終了する
                if (RemainingJacIn == 0)
                {
                    Debug.Log("BIG CHANCE end");
                    CurrentBonusStatus = BonusStatus.BonusNone;
                }
            }
        }
    }
}
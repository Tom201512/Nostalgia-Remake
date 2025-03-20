using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Bonus
{
    public class BonusManager
    {
        // ボーナスのデータ

        // const

        public enum BonusType {BonusNone, BonusBIG, BonusREG }

        // 残り小役ゲーム数
        public const int BigGames = 30;
        // 残りJACIN
        public const int JacInTimes = 3;

        // JACゲーム中
        // 残りJACゲーム数
        public const int JacGames = 12;
        // 残り当選回数
        public const int JacHits = 12;

        // var
        // 処理状態

        // ボーナス中か
        public bool IsBonus { get; private set; }
        // JACゲーム中か
        public bool IsJacGame { get; private set; }

        // 残りゲーム数、当選回数(JAC-INまたはJAC役)

        // 小役ゲーム中
        // 残り小役ゲーム数
        public int RemainingBIGGames { get; private set; }
        // 残りJACIN
        public int RemainingJACIN { get; private set; }

        // JACゲーム中
        // 残りJACゲーム数
        public int RemainingJACGames { get; private set; }
        // 残り当選回数
        public int RemainingJACHits { get; private set; }

        // コンストラクタ
        public BonusManager()
        {
            RemainingBIGGames = 0;
            RemainingJACIN = 0;
            RemainingJACHits = 0;
            RemainingJACGames = 0;
        }

        public BonusManager(int remainingBIGGames, int remainingJACIN, int remainingJACGames, int remainingJACHits)
        {
            RemainingBIGGames = remainingBIGGames;
            RemainingJACIN = remainingJACIN;
            RemainingJACGames = remainingJACGames;
            RemainingJACHits = remainingJACHits;
        }

        // func

        public void StartBigChance()
        {
            Debug.Log("BIG CHANCE start");
            RemainingBIGGames = BigGames;
            RemainingJACIN = JacInTimes;
            IsBonus = true;
        }

        public void StartBonusGame()
        {
            if (RemainingJACIN > 0)
            {
                RemainingJACIN -= 1;
            }
            Debug.Log("BONUS GAME start");
            RemainingJACGames = JacGames;
            RemainingJACHits = JacHits;

            IsBonus = true;
            IsJacGame = true;
        }

        // BIG中小役ゲーム数を減らす
        public void DecreaseBigGames(bool hasJacIn)
        {
            RemainingBIGGames -= 1;

            // JAC-INなら
            if (hasJacIn)
            {
                RemainingJACIN -= 1;
                StartBonusGame();
            }

            // 30ゲームを消化した場合
            else if (RemainingBIGGames == 0)
            {
                Debug.Log("BIG CHANCE end");
            }
        }

        // ボーナスゲーム数を減らす
        public void DecreaseBonusGames(bool hasPayout)
        {
            RemainingJACGames -= 1;

            if (hasPayout)
            {
                RemainingJACHits -= 1;
            }

            // JAC ゲーム数が0, または入賞回数が0の場合は終了
            if (RemainingJACGames == 0 || RemainingJACHits == 0)
            {
                Debug.Log("End Bonus Game");

                // BIG中ならJAC-INが0の場合終了する
                if (RemainingJACIN == 0)
                {
                    Debug.Log("BIG CHANCE end");
                }
            }
        }
    }
}
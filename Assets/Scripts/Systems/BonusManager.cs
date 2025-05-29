using ReelSpinGame_Sound;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_Bonus
{
    public class BonusManager : MonoBehaviour
    {
        // ボーナス処理

        // const

        // var
        // ボーナス処理のデータ
        private BonusBehaviour data;

        // ボーナス状態のセグメント
        [SerializeField] private BonusSevenSegment bonusSegments;
        // サウンド
        [SerializeField] private SoundManager soundManager;

        // func
        private void Awake()
        {
            data = new BonusBehaviour();
        }

        // 各種数値を得る
        // ストック中のボーナス
        public BonusType GetHoldingBonusID() => data.HoldingBonusID;
        // ボーナス状態
        public BonusStatus GetCurrentBonusStatus() => data.CurrentBonusStatus;
        // BIGボーナス当選時の色
        public BigColor GetBigChangeColor() => data.BigChanceColor;

        // 残り小役ゲーム数
        public int GetRemainingBigGames() => data.RemainingBigGames;
        // 残りJAC-IN回数
        public int GetRemainingJacIn() => data.RemainingJacIn;
        // 残りJACゲーム数
        public int GetRemainingJacGames() => data.RemainingJacGames;
        // 残りJACゲーム当選回数
        public int GetRemainingJacHits() => data.RemainingJacHits;

        // ボーナス情報を読み込む
        public void SetBonusData(BonusType holdingBonusID, BonusStatus bonusStatus, int remainingBIGGames, int remainingJACIN,
            int remainingJACGames, int remainingJACHits)
        {
            data.HoldingBonusID = holdingBonusID;
            data.CurrentBonusStatus = bonusStatus;
            data.RemainingBigGames = remainingBIGGames;
            data.RemainingJacIn = remainingJACIN;
            data.RemainingJacGames = remainingJACGames;
            data.RemainingJacHits = remainingJACHits;
        }

        // ボーナスストック状態の更新
        public void SetBonusStock(BonusType bonusType) => data.HoldingBonusID = bonusType;

        // ビッグチャンスの開始
        public void StartBigChance(BigColor bigColor)
        {
            // 対応したボーナスの演出を開始
            //Debug.Log("BIG CHANCE start");
            data.RemainingBigGames = BigGames;
            data.RemainingJacIn = JacInTimes;
            data.CurrentBonusStatus = BonusStatus.BonusBIGGames;
            data.HoldingBonusID = BonusType.BonusNone;
        }

        // ボーナスゲームの開始
        public void StartBonusGame()
        {
            if (data.RemainingJacIn > 0)
            {
                data.RemainingJacIn -= 1;
            }
            //Debug.Log("BONUS GAME start");
            data.RemainingJacGames = JacGames;
            data.RemainingJacHits = JacHits;
            data.CurrentBonusStatus = BonusStatus.BonusJACGames;
            data.HoldingBonusID = BonusType.BonusNone;
        }

        // ボーナス状態のセグメント更新
        public void UpdateSegments()
        {
            // BIG中
            if(data.CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                bonusSegments.ShowBigStatus(data.RemainingJacIn, data.RemainingBigGames);
            }

            // JAC中
            else if (data.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                bonusSegments.ShowJacStatus(data.RemainingJacIn + 1, data.RemainingJacHits);
            }

            // 通常時は消灯させる
            else
            {
                bonusSegments.TurnOffAllSegments();
            }
        }

        // ゲーム数を減らす
        public void DecreaseGames()
        {
            if (data.CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                data.RemainingBigGames -= 1;
            }
            else if (data.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                data.RemainingJacGames -= 1;
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
            else if (data.RemainingBigGames == 0)
            {
                //Debug.Log("BIG CHANCE end");
                EndBonusStatus();
            }
        }

        // ボーナスゲームの状態遷移
        public void CheckBonusGameStatus(bool hasPayout)
        {
            if (hasPayout)
            {
                data.RemainingJacHits -= 1;
            }

            // JACゲーム数が0, または入賞回数が0の場合は終了
            if (data.RemainingJacGames == 0 || data.RemainingJacHits == 0)
            {
                //Debug.Log("End Bonus Game");

                // BIG中なら残りJAC-INの数があれば小役ゲームへ移行
                if (data.RemainingJacIn > 0)
                {
                    data.CurrentBonusStatus = BonusStatus.BonusBIGGames;
                }
                else
                {
                    EndBonusStatus();
                }
            }
        }

        public void EndBonusStatus()
        {
            data.RemainingBigGames = 0;
            data.RemainingJacIn = 0;
            data.RemainingJacGames = 0;
            data.RemainingJacHits = 0;
            data.CurrentBonusStatus = BonusStatus.BonusNone;
            //Debug.Log("Bonus Reset");
        }

        // ボーナスの音声再生
        private IEnumerator UpdateBonusFanfare()
        {
            yield return null;
        }
    }
}
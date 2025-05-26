using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_Bonus
{
    public class BonusManager : MonoBehaviour
    {
        // ボーナスのデータ

        // const

        // var
        // ボーナス処理のデータ
        public BonusBehaviour Data { get; private set; }

        // ボーナス状態のセグメント
        [SerializeField] private BonusSevenSegment bonusSegments;

        // func
        private void Awake()
        {
            Data = new BonusBehaviour();
        }

        // ボーナス状態のセグメント更新
        public void UpdateSegments()
        {
            // BIG中
            if(Data.CurrentBonusStatus == BonusStatus.BonusBIGGames)
            {
                bonusSegments.ShowBigStatus(Data.RemainingJacIn, Data.RemainingBigGames);
            }

            // JAC中
            else if (Data.CurrentBonusStatus == BonusStatus.BonusJACGames)
            {
                bonusSegments.ShowJacStatus(Data.RemainingJacIn + 1, Data.RemainingJacHits);
            }

            // 通常時は消灯させる
            else
            {
                bonusSegments.TurnOffAllSegments();
            }
        }
    }
}
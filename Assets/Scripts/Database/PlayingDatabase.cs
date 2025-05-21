using ReelSpinGame_Bonus;
using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_System
{
    public class PlayingDatabase
    {
        // プレイ中情報

        // const

        // 記録可能ゲーム数
        public const int MaximumTotalGames = 99999;

        // var
        // ゲーム数情報
        // 総ゲーム数(ボーナス中除く)
        public int TotalGames { get; private set; }
        // ボーナス間ゲーム数
        public int CurrentGames { get; private set; }

        // メダル情報
        public MedalData PlayerMedalData { get; private set; }

        // 当選させたボーナス(IDごとに)
        public List<BonusHitData> BonusHitDatas { get; private set; }

        // ビッグチャンス成立回数
        public int BigTimes { get; private set; }
        // ボーナスゲーム成立回数
        public int RegTimes { get; private set; }

        // コンストラクタ
        public PlayingDatabase()
        {
            PlayerMedalData = new MedalData();
            BonusHitDatas = new List<BonusHitData>();
        }

        // セーブデータから読み込む場合
        public PlayingDatabase(MedalData savedMedalData, List<BonusHitData> savedHitDatas) : this()
        {
            PlayerMedalData = savedMedalData;
            BonusHitDatas = savedHitDatas;
        }

        // func

        // 各種データ数値変更

        // ゲーム数を増やす
        public void IncreaseGameValue()
        {
            TotalGames += 1;
            CurrentGames += 1;
        }
        // ボーナス間ゲーム数リセット
        public void ResetCurrentGame() => CurrentGames = 0;

        // ボーナス履歴追加(成立時に使用)
        public void AddBonusResult(BonusManager.BonusType bonusType)
        {
            Debug.Log("BonusHit");
            BonusHitData test = new BonusHitData(bonusType);
            BonusHitDatas.Add(test);
            BonusHitDatas[^1].SetBonusHitGame(CurrentGames);
        }

        // 直近のボーナス履歴の入賞を記録
        public void SetLastBonusStart()
        {
            BonusHitDatas[^1].SetBonusStartGame(CurrentGames);
        }

        // 現在のボーナス履歴に払い出しを追加する
        public void ChangeBonusPayoutToLast(int payouts) => BonusHitDatas[~1].ChangeBonusPayouts(payouts);

        // ビッグチャンス回数の増加
        public void IncreaseBigChance() => BigTimes += 1;
        // ボーナスゲーム回数の増加
        public void IncreaseBonusGame() => RegTimes += 1;
    }
}

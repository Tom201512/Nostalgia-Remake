using ReelSpinGame_Interface;
using ReelSpinGame_Save.Bonus;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_Bonus
{
    public class BonusManager : MonoBehaviour, IHasSave
    {
        // ボーナス処理

        // const
        // 獲得枚数の点滅時に点滅させる時間(秒)
        const float PayoutSegFlashTime = 0.5f;

        // var
        // ボーナス処理のデータ
        private BonusSystemData data;
        // ボーナス状態のセグメント
        [SerializeField] private BonusSevenSegment bonusSegments;
        // 獲得枚数を表示しているか
        public bool DisplayingTotalCount { get; private set; }

        // func
        private void Awake()
        {
            data = new BonusSystemData();
            DisplayingTotalCount = false;
        }

        // 各種数値を得る
        // ストック中のボーナス
        public BonusType GetHoldingBonusID() => data.HoldingBonusID;
        // ボーナス状態
        public BonusStatus GetCurrentBonusStatus() => data.CurrentBonusStatus;
        // BIGボーナス当選時の色
        public BigColor GetBigChanceColor() => data.BigChanceColor;

        // 残り小役ゲーム数
        public int GetRemainingBigGames() => data.RemainingBigGames;
        // 残りJAC-IN回数
        public int GetRemainingJacIn() => data.RemainingJacIn;
        // 残りJACゲーム数
        public int GetRemainingJacGames() => data.RemainingJacGames;
        // 残りJACゲーム当選回数
        public int GetRemainingJacHits() => data.RemainingJacHits;

        // 獲得した枚数を表示
        public int GetCurrentBonusPayouts() => data.CurrentBonusPayouts;
        // 連チャン区間中の枚数を表示
        public int GetCurrentZonePayouts() => data.CurrentZonePayouts;
        // 連チャン区間にいるか
        public bool GetHasZone() => data.HasZone;

        // 獲得枚数の増減
        public int ChangeBonusPayouts(int amounts) => data.CurrentBonusPayouts += amounts;
        public int ChangeZonePayouts(int amounts) => data.CurrentZonePayouts += amounts;

        // 連チャン区間枚数を消す
        public void ResetZonePayouts()
        {
            data.HasZone = false;
            data.CurrentZonePayouts = 0;
        }

        // セーブデータにする
        public ISavable MakeSaveData()
        {
            BonusSave save = new BonusSave();
            save.RecordData(data);

            return save;
        }

        // セーブを読み込む
        public void LoadSaveData(ISavable loadData)
        {
            if (loadData.GetType() == typeof(BonusSave))
            {
                BonusSave save = loadData as BonusSave;

                data.HoldingBonusID = save.HoldingBonusID;
                data.CurrentBonusStatus = save.CurrentBonusStatus;
                data.BigChanceColor = save.BigChanceColor;
                data.RemainingBigGames = save.RemainingBigGames;
                data.RemainingJacIn = save.RemainingJacIn;
                data.RemainingJacHits = save.RemainingJacHits;
                data.RemainingJacGames = save.RemainingJacGames;
                data.CurrentBonusPayouts = save.CurrentBonusPayouts;
                data.CurrentZonePayouts = save.CurrentZonePayouts;
                data.HasZone = save.HasZone;
            }
            else
            {
                throw new Exception("Loaded data is not BonusData");
            }
        }

        // ボーナスストック状態の更新
        public void SetBonusStock(BonusType bonusType) => data.HoldingBonusID = bonusType;

        // ビッグチャンスの開始
        public void StartBigChance(BigColor bigColor)
        {
            //Debug.Log("BIG CHANCE start");
            // ビッグチャンスの初期化
            data.CurrentBonusPayouts = 0;
            data.RemainingBigGames = BigGames;
            data.RemainingJacIn = JacInTimes;
            data.CurrentBonusStatus = BonusStatus.BonusBIGGames;
            data.HoldingBonusID = BonusType.BonusNone;
            data.BigChanceColor = bigColor;
            // 連チャン区間の記録開始
            data.HasZone = true;
        }

        // ボーナスゲームの開始
        public void StartBonusGame()
        {
            // 残りJAC-INがあれば減らす
            if (data.RemainingJacIn > 0)
            {
                data.RemainingJacIn -= 1;
            }
            //Debug.Log("BONUS GAME start");
            // BIG中でない場合はボーナス払い出し枚数リセット
            if (data.CurrentBonusStatus != BonusStatus.BonusBIGGames)
            {
                data.CurrentBonusPayouts = 0;
            }

            // ボーナスゲームの初期化
            data.RemainingJacGames = JacGames;
            data.RemainingJacHits = JacHits;
            data.CurrentBonusStatus = BonusStatus.BonusJACGames;
            data.HoldingBonusID = BonusType.BonusNone;
            // 連チャン区間の記録開始
            data.HasZone = true;
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
            // 通常時に戻った場合は獲得枚数表示とリセット
            else if(DisplayingTotalCount)
            {
                StartCoroutine(nameof(UpdateShowPayouts));
            }
        }

        // セグメントをすべて消す
        public void TurnOffSegments()
        {
            StopCoroutine(nameof(UpdateShowPayouts));
            bonusSegments.TurnOffAllSegments();
            DisplayingTotalCount = false;
        }

        // 小役ゲーム数、JACゲーム数を減らす
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
                EndBonusStatus();
            }
        }

        // ボーナスゲームの状態遷移
        public void CheckBonusGameStatus(bool hasPayout)
        {
            // JAC役が当選(払い出しがあった)場合は残り入賞回数を減らす
            if (hasPayout)
            {
                data.RemainingJacHits -= 1;
            }
            // JACゲーム数が0, または入賞回数が0の場合は終了(BIG中なら残りゲーム数0で終了)
            if (data.RemainingJacGames == 0 || data.RemainingJacHits == 0)
            {
                //Debug.Log("End Bonus Game");
                // BIG中なら残りJAC-INの数があれば小役ゲームへ移行
                if (data.RemainingJacIn > 0 && data.RemainingBigGames > 0)
                {
                    data.CurrentBonusStatus = BonusStatus.BonusBIGGames;
                }
                else
                {
                    EndBonusStatus();
                }
            }
        }

        // ボーナスの終了処理
        private void EndBonusStatus()
        {
            data.RemainingBigGames = 0;
            data.RemainingJacIn = 0;
            data.RemainingJacGames = 0;
            data.RemainingJacHits = 0;
            data.CurrentBonusStatus = BonusStatus.BonusNone;
            DisplayingTotalCount = true;
        }

        // 獲得枚数を点滅させる
        private IEnumerator UpdateShowPayouts()
        {
            while(DisplayingTotalCount)
            {
                bonusSegments.ShowTotalPayouts(data.CurrentZonePayouts);
                yield return new WaitForSeconds(PayoutSegFlashTime);
                bonusSegments.TurnOffAllSegments();
                yield return new WaitForSeconds(PayoutSegFlashTime);
            }
            bonusSegments.TurnOffAllSegments();
        }
    }
}
using ReelSpinGame_Sound;
using System;
using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_Bonus
{
    public class BonusManager : MonoBehaviour
    {
        // ボーナス処理

        // var
        // ボーナス処理のデータ
        private BonusBehaviour data;

        // ボーナス状態のセグメント
        [SerializeField] private BonusSevenSegment bonusSegments;
        // サウンド
        [SerializeField] private SoundManager soundManager;

        // ボーナスファンファーレ処理をしているか
        public bool HasFanfareUpdate { get; private set; }
        // 獲得枚数を表示しているか
        public bool DisplayingTotalCount { get; private set; }

        // 直前のボーナス状態(同じBGMが再生されていないかチェック用)
        private BonusStatus lastBonusStatus;

        // func
        private void Awake()
        {
            data = new BonusBehaviour();
            HasFanfareUpdate = false;
            DisplayingTotalCount = false;
            lastBonusStatus = BonusStatus.BonusNone;
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

        // 獲得した枚数を表示
        public int GetCurrentBonusPayouts() => data.CurrentBonusPayouts;
        // 連チャン区間中の枚数を表示
        public int GetCurrentZonePayouts() => data.CurrentZonePayouts;

        // 獲得枚数の増減
        public int ChangeBonusPayouts(int amounts) => data.CurrentBonusPayouts = Math.Clamp(data.CurrentBonusPayouts + amounts, 0, MaxRecordPayouts);
        public int ChangeZonePayouts(int amounts) => data.CurrentZonePayouts = Math.Clamp(data.CurrentZonePayouts + amounts, 0, MaxRecordPayouts);

        // 連チャン区間枚数を消す
        public void ResetZonePayouts() => data.CurrentZonePayouts = 0;

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
            data.BigChanceColor = bigColor;
            data.CurrentBonusPayouts = 0;
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
            data.CurrentBonusPayouts = 0;
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

            // 通常時に戻った場合は獲得枚数表示
            else if(DisplayingTotalCount)
            {
                bonusSegments.ShowTotalPayouts(data.CurrentZonePayouts);
            }
        }

        // セグメントをすべて消す
        public void TurnOffSegments()
        {
            bonusSegments.TurnOffAllSegments();
            DisplayingTotalCount = false;
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

        // ファンファーレ再生
        public void PlayBonusFanfare()
        {
            StartCoroutine(nameof(UpdateBonusFanfare));
        }

        // BGMを再生
        public void PlayBGM()
        {
            // 状態が変わっていたら変更
            if(GetCurrentBonusStatus() == BonusStatus.BonusBIGGames &&
                lastBonusStatus != GetCurrentBonusStatus())
            {
                PlayBigGameBGM();
            }
            else if(GetCurrentBonusStatus() == BonusStatus.BonusJACGames &&
                lastBonusStatus != GetCurrentBonusStatus())
            {
                PlayBonusGameBGM();
            }
            else if(GetCurrentBonusStatus() == BonusStatus.BonusNone)
            {
                soundManager.StopBGM();
            }

            lastBonusStatus = GetCurrentBonusStatus();
        }

        // 終了ファンファーレ再生(現状BIGのみ)
        public void PlayEndBonusFanfare()
        {
            StartCoroutine(nameof(UpdateEndFanfare));
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
            PlayEndBonusFanfare();
        }

        // ボーナス当選ファンファーレ再生処理
        private IEnumerator UpdateBonusFanfare()
        {
            HasFanfareUpdate = true;
            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            // ファンファーレを鳴らす
            PlayFanfare();

            // 今鳴らしているファンファーレが止まるのを待つ
            while (!soundManager.GetBGMStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            HasFanfareUpdate = false;
        }

        // ボーナス終了ファンファーレ再生処理
        private IEnumerator UpdateEndFanfare()
        {
            HasFanfareUpdate = true;

            // 今鳴らしている効果音が止まるのを待つ
            while (!soundManager.GetSoundEffectStopped())
            {
                yield return new WaitForEndOfFrame();
            }

            // BIGの時のみファンファーレを鳴らす
            if(GetBigChangeColor() != BigColor.None)
            {
                PlayBigEndFanfare();
                // 今鳴らしているファンファーレが止まるのを待つ
                while (!soundManager.GetBGMStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            HasFanfareUpdate = false;
            soundManager.StopBGM();
        }

        private void PlayFanfare()
        {
            // ファンファーレ再生
            switch (data.BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedStart, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueStart, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackStart, false);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegStart, false);
                    break;
            }
        }

        // 小役ゲーム中のBGM再生
        private void PlayBigGameBGM()
        {
            switch (data.BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedBGM, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueBGM, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackBGM, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegJAC, true);
                    break;
            }
        }

        // ボーナスゲーム中のBGM再生
        private void PlayBonusGameBGM()
        {
            switch (data.BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedJAC, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueJAC, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackJAC, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.BGMList.RegJAC, true);
                    break;
            }
        }

        // 終了ジングル再生(BIGのみ)
        private void PlayBigEndFanfare()
        {
            switch (data.BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.BGMList.RedEnd, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.BGMList.BlueEnd, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.BGMList.BlackEnd, false);
                    break;
            }
        }
    }
}
using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // リールフラッシュやサウンドの管理

        // const
        // リプレイ時に待機させる時間(秒)
        const float ReplayWaitTime = 1.0f;
        // Vフラッシュ確率(1/n)
        const int VFlashProb = 6;

        // var
        // フラッシュ機能
        private FlashManager flashManager;
        // サウンド機能
        private SoundManager soundManager;
        // フラッシュ中か
        public bool HasFlash { get; private set; }
        // フラッシュで待機中か
        public bool HasFlashWait { get; private set; }
        // 現在のフラッシュID
        public int CurrentFlashID { get; private set; }
        // ボーナス処理で待機中か
        public bool HasFanfareUpdate { get; private set; }
        // ビッグチャンス時の色
        public BigColor BigChanceColor { get; private set; }
        // 直前のボーナス状態(同じBGMが再生されていないかチェック用)
        private BonusStatus lastBonusStatus;

        // リールのオブジェクト
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
            soundManager = GetComponent<SoundManager>();
        }

        private void Start()
        {
            // リールオブジェクト割り当て
            flashManager.SetReelObjects(reelObjects);
        }

        // 演出が終了しているか(サウンド、フラッシュ、ボーナスファンファーレのすべてが停止中)
        public bool HasEffectFinished() => !flashManager.HasFlashWait && !soundManager.GetSoundEffectHasLoop() && !HasFanfareUpdate;

        // フラッシュ関連
        // リール全点灯
        public void TurnOnAllReels(bool isJacGame)
        {
            // JAC GAME中は中段のみ光らせる
            if (isJacGame)
            {
                flashManager.EnableJacGameLight();
            }
            else
            {
                flashManager.TurnOnAllReels();
            }

            // JAC中のライト処理をする
            foreach (ReelObject reel in reelObjects)
            {
                if (reel.HasJacModeLight != isJacGame)
                {
                    reel.HasJacModeLight = isJacGame;
                }
            }
        }

        // リールライト全消灯
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();
        // リールフラッシュを開始させる
        public void StartReelFlash(FlashID flashID) => flashManager.StartReelFlash(flashID);

        // サウンド
        // ベット音再生
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundEffectList.Wait);
        // スタート音
        public void StartLeverOnEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Start);
        // 停止音
        public void StartReelStopEffect() => soundManager.PlaySoundOneShot(soundManager.SoundEffectList.Stop);

        // リーチ時演出
        public void StartRiichiEffect(BigColor color)
        {
            switch (color)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.RedRiichiSound);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.BlueRiichiSound);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundEffectList.BB7RiichiSound);
                    break;
            }
        }

        // 払い出し演出開始
        public void StartPayoutReelFlash(List<PayoutLineData> lastPayoutLines, BonusStatus status, int payouts)
        {
            // フラッシュ再生
            flashManager.StartPayoutFlash(0f, lastPayoutLines);

            // サウンド再生(状態に合わせて変更)
            // JAC中の払い出し音
            if (status == BonusStatus.BonusJACGames)
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.JacPayout);
            }
            // 15枚の払い出し音
            else if (payouts >= 15)
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.MaxPayout);
            }
            //　それ以外は通常の払い出し音
            else
            {
                soundManager.PlaySoundLoop(soundManager.SoundEffectList.NormalPayout);
            }
        }

        // リプレイの演出
        public void StartReplayEffect(List<PayoutLineData> lastPayoutLines)
        {
            //音再生
            soundManager.PlaySoundLoop(soundManager.SoundEffectList.Replay);
            // フラッシュさせる
            flashManager.StartPayoutFlash(ReplayWaitTime, lastPayoutLines);
        }

        // リーチ目出現時の演出
        public void StartRiichiPatternEffect()
        {
            if (Random.Range(0, VFlashProb - 1) == 0)
            {
                flashManager.StartReelFlash(FlashID.V_Flash);
            }
        }

        // ボーナス開始時の演出
        public void StartBonusStartEffect(BigColor color)
        {
            // ビッグチャンス時は対応した色のファンファーレを再生
            BigChanceColor = color;
            StartCoroutine(nameof(UpdateBonusFanfare));
        }

        // ボーナス終了時の演出
        public void StartBonusEndEffect(BigColor color)
        {
            // ビッグチャンス時は対応した色のファンファーレを再生
            BigChanceColor = color;
            StartCoroutine(nameof(UpdateEndFanfare));
        }


        // フラッシュ停止
        public void StopReelFlash() => flashManager.StopFlash();

        // ループしている音を止める
        public void StopLoopSound() => soundManager.StopLoopSound();

        // BGMを再生
        public void PlayBonusBGM(BonusStatus status)
        {
            if(lastBonusStatus != status)
            {
                switch(status)
                {
                    case BonusStatus.BonusBIGGames:
                        PlayBigGameBGM();
                        break;
                    case BonusStatus.BonusJACGames:
                        PlayBonusGameBGM();
                        break;
                    case BonusStatus.BonusNone:
                        soundManager.StopBGM();
                        break;
                }
                lastBonusStatus = status;
            }
        }

        // ファンファーレ再生
        private void PlayFanfare()
        {
            switch (BigChanceColor)
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
            switch (BigChanceColor)
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
            switch (BigChanceColor)
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
            switch (BigChanceColor)
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

        // コルーチン

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
            if (BigChanceColor != BigColor.None)
            {
                PlayBigEndFanfare();
                // 今鳴らしているファンファーレが止まるのを待つ
                while (!soundManager.GetBGMStopped())
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            HasFanfareUpdate = false;
            BigChanceColor = BigColor.None;
            soundManager.StopBGM();
        }
    }
}

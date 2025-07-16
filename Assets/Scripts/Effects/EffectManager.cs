using ReelSpinGame_Datas;
using ReelSpinGame_Lots;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;
using static ReelSpinGame_Lots.FlagBehaviour;

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
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundDB.SE.Wait);
        // スタート音
        public void StartLeverOnEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);

        // テスト用(特殊スタート機能)
        public void StartLeverOnEffect(FlagId flag, BonusType holding, BonusStatus bonusStatus)
        {
            // BIG中
            if (bonusStatus == BonusStatus.BonusBIGGames)
            {
                // リプレイ、はずれ時に1/6で再生
                if(flag == FlagId.FlagNone || flag == FlagId.FlagReplayJacIn)
                {
                    LotStartSound(6);
                }
                else
                {
                    LotStartSound(0);
                }
            }
            // 通常時
            else if (bonusStatus == BonusStatus.BonusNone)
            {
                // 以下の確率で告知音で再生
                // BIG/REG成立時、成立後小役条件不問で1/6
                // スイカ、1/8
                // ベル、チェリー、1/16
                // リプレイ、発生しない
                // はずれ、1/128

                if(holding != BonusType.BonusNone)
                {
                    // BIG, REG
                    switch(flag)
                    {
                        case FlagId.FlagBig:
                        case FlagId.FlagReg:
                            LotStartSound(6);
                            break;

                        case FlagId.FlagMelon:
                            LotStartSound(8);
                            break;

                        case FlagId.FlagCherry2:
                        case FlagId.FlagCherry4:
                        case FlagId.FlagBell:
                            LotStartSound(16);
                            break;

                        case FlagId.FlagReplayJacIn:
                            LotStartSound(0);
                            break;

                        default:
                            LotStartSound(128);
                            break;
                    }
                }

                // 成立後は1/6で再生
                else
                {
                    LotStartSound(6);
                }

            }
            // JAC中(鳴らさない)
            else
            {
                LotStartSound(0);
            }
        }

        // 指定した確率で再生音の抽選をする
        private void LotStartSound(int probability)
        {
            // 確率が0より低い場合は通常スタート音
            if(probability > 0)
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
            // 確率が1以上なら抽選
            else if (Random.Range(0, probability - 1) == 0)
            {
                Debug.Log("SP SOUND PLAYED");
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.SpStart);
            }
            else
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
        }

        // 停止音
        public void StartReelStopEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Stop);

        // リーチ時演出
        public void StartRiichiEffect(BigColor color)
        {
            switch (color)
            {
                case BigColor.Red:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.RedRiichiSound);
                    break;
                case BigColor.Blue:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BlueRiichiSound);
                    break;
                case BigColor.Black:
                    soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.BB7RiichiSound);
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
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.JacPayout);
            }
            // 15枚の払い出し音
            else if (payouts >= 15)
            {
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.MaxPayout);
            }
            //　それ以外は通常の払い出し音
            else
            {
                soundManager.PlaySoundLoop(soundManager.SoundDB.SE.NormalPayout);
            }
        }

        // リプレイの演出
        public void StartReplayEffect(List<PayoutLineData> lastPayoutLines)
        {
            //音再生
            soundManager.PlaySoundAndWait(soundManager.SoundDB.SE.Replay);
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
        public void StartBonusEndEffect()
        {
            // ビッグチャンス時は対応した色のファンファーレを再生
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
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedStart, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueStart, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackStart, false);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegStart, false);
                    break;
            }
        }

        // 小役ゲーム中のBGM再生
        private void PlayBigGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedBGM, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueBGM, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackBGM, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC, true);
                    break;
            }
        }

        // ボーナスゲーム中のBGM再生
        private void PlayBonusGameBGM()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedJAC, true);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueJAC, true);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackJAC, true);
                    break;
                default:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RegJAC, true);
                    break;
            }
        }

        // 終了ジングル再生(BIGのみ)
        private void PlayBigEndFanfare()
        {
            switch (BigChanceColor)
            {
                case BigColor.Red:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.RedEnd, false);
                    break;
                case BigColor.Blue:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlueEnd, false);
                    break;
                case BigColor.Black:
                    soundManager.PlayBGM(soundManager.SoundDB.BGM.BlackEnd, false);
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

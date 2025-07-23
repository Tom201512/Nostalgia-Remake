using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using ReelSpinGame_Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // リールフラッシュやサウンドの管理

        // const
        // リプレイ時に待機させる時間(秒)
        const float ReplayWaitTime = 1.0f;

        // Vフラッシュ時の待機時間(秒)
        const float VFlashWaitTime = 1.0f;

        // var
        // フラッシュ機能
        private FlashManager flashManager;
        // サウンド機能
        private SoundManager soundManager;

        // スタート音に予告音をつけるか
        [SerializeField] private bool hasSPStartSound;
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

        // サウンド
        // ベット音再生
        public void StartBetEffect() => soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Bet);
        // ウェイト音再生
        public void StartWaitEffect() => soundManager.PlaySoundLoop(soundManager.SoundDB.SE.Wait);

        // スタート音
        public void StartLeverOnEffect(FlagId flag, BonusType holding, BonusStatus bonusStatus)
        {
            if(hasSPStartSound)
            {
                // BIG中
                if (bonusStatus == BonusStatus.BonusBIGGames)
                {
                    // リプレイ時に1/6で再生
                    if (flag == FlagId.FlagReplayJacIn)
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
                    // BIG/REG成立時、成立後小役条件不問で1/4
                    // スイカ、1/8
                    // チェリー、発生しない
                    // ベル、1/32
                    // リプレイ、発生しない
                    // はずれ、1/128

                    if (holding != BonusType.BonusNone)
                    {
                        // BIG, REG
                        switch (flag)
                        {
                            case FlagId.FlagBig:
                            case FlagId.FlagReg:
                                LotStartSound(4);
                                break;

                            case FlagId.FlagMelon:
                                LotStartSound(8);
                                break;

                            case FlagId.FlagBell:
                                LotStartSound(32);
                                break;

                            case FlagId.FlagCherry2:
                            case FlagId.FlagCherry4:
                            case FlagId.FlagReplayJacIn:
                                LotStartSound(0);
                                break;

                            default:
                                LotStartSound(128);
                                break;
                        }
                    }

                    // 成立後は1/4で再生
                    else
                    {
                        LotStartSound(4);
                    }

                }
                // JAC中(鳴らさない)
                else
                {
                    LotStartSound(0);
                }
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
        public void StartPayoutReelFlash(List<PayoutLineData> lastPayoutLines, bool isJacFlag, int payouts)
        {
            // フラッシュ再生
            flashManager.StartPayoutFlash(0f, lastPayoutLines);

            // サウンド再生(状態に合わせて変更)
            // JAC中の払い出し音
            if (isJacFlag)
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

        // Vフラッシュ演出
        public void StartVFlash(int probability)
        {
            // 確率が0以上ならフラッシュ抽選
            if (probability > 0 && Random.Range(0, probability - 1) == 0)
            {
                flashManager.StartReelFlash(VFlashWaitTime, FlashID.V_Flash);
            }
        }

        // ビッグ時の色を割り当てる
        public void SetBigColor(BigColor color) => BigChanceColor = color;

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

        // 指定した確率で再生音の抽選をする
        private void LotStartSound(int probability)
        {
            // 確率が0以下は通常スタート音
            if (probability <= 0)
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
            }
            // 確率が1以上なら抽選
            else if (Random.Range(0, probability - 1) == 0)
            {
                //Debug.Log("SP SOUND PLAYED");
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.SpStart);
            }
            else
            {
                soundManager.PlaySoundOneShot(soundManager.SoundDB.SE.Start);
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

            Debug.Log("Event End");
            HasFanfareUpdate = false;
            BigChanceColor = BigColor.None;
            soundManager.StopBGM();
        }
    }
}

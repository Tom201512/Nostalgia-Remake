using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class BgmPlayer : MonoBehaviour
    {
        // BGMプレイヤー

        // const
        // サンプリングレート
        const double SampleRate = 44100.0;

        // var
        // コンポーネント
        [SerializeField] AudioSource[] sources;

        // 再生が終了したか
        public bool HasSoundStopped { get; private set; }
        // ループしている音があるか
        public bool HasLoop { get; private set; }
        // ループ開始位置
        public int LoopStart { get; private set; }
        // ループ長さ
        public int LoopLength { get; private set; }
        // 鳴らさないようにするか
        public bool HasLockPlaying;

        // ループ時の時間
        private double loopTime;

        void Awake()
        {
            HasSoundStopped = true;
            HasLockPlaying = false;
            Debug.Log("Count:" + sources.Length);

            LoopStart = -1;
            LoopLength = -1;
            loopTime = 0.0;
        }

        private void Update()
        {
            // ループがあるときの処理
            if(HasLoop && LoopStart > -1)
            {
                if (AudioSettings.dspTime > loopTime)
                {
                    sources[1].timeSamples = LoopStart;
                    Debug.Log("Looped");
                    Debug.Log("DSP:" + AudioSettings.dspTime);
                    sources[1].PlayScheduled(loopTime);

                    double duration = 0.0;
                    if (LoopLength > -1)
                    {
                        duration = LoopStart + LoopLength / SampleRate;
                    }
                    else
                    {
                        duration = LoopStart / SampleRate;
                    }

                    Debug.Log("Duration:" + duration);
                    loopTime += duration;
                    Debug.Log("Next loop is:" + loopTime);
                }
            }
        }

        private void OnDestroy()
        {
            StopAudio();
            StopAllCoroutines();
        }

        // func

        // 再生
        public void PlayBGM(AudioClip soundSource)
        {
            if (!HasLockPlaying)
            {
                StopLoopCheck();
                sources[0].loop = false;
                sources[0].clip = soundSource;
                sources[0].Play();
                StartCoroutine(nameof(CheckAudioStopped));
            }
        }

        // ループ再生
        public void PlayBGMLoop(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                // イントロ付きループならソースを2つ使ってループ再生を実現
                if (loopStart > -1)
                {
                    // 初期化
                    loopTime = AudioSettings.dspTime;
                    LoopStart = loopStart;

                    // 長さ付きの場合は追加
                    if(loopLength > -1)
                    {
                        LoopLength = loopLength;
                    }

                    sources[0].loop = false;
                    sources[0].clip = soundSource;
                    sources[0].PlayScheduled(loopTime);

                    double duration;
                    // ループさせる長さがある場合はその長さまで計算
                    if (loopLength > -1)
                    {
                         duration = (loopStart + loopLength) / SampleRate;
                    }
                    // ない場合は再生し終えたところを長さとする
                    else
                    {
                         duration = sources[0].clip.samples / SampleRate;
                    }

                    Debug.Log("Duration:" + duration);
                    loopTime += duration;
                    Debug.Log("Next loop is:" + loopTime);

                    // ループを準備
                    sources[1].clip = soundSource;
                    sources[1].loop = false;
                }
                // ない場合は普通のループ再生をする
                else
                {
                    StopLoopCheck();
                    sources[0].loop = true;
                    sources[0].clip = soundSource;
                    sources[0].Play();
                }

                HasLoop = true;
            }
        }

        // 音停止
        public void StopAudio()
        {
            sources[0].loop = false;
            sources[0].Stop();
            StopLoopCheck();
            HasLoop = false;
        }

        // ループ処理を切る
        private void StopLoopCheck()
        {
            sources[1].Stop();
            LoopStart = -1;
            LoopLength = -1;
        }

        // ボリューム調整
        public void AdjustVolume(float volume)
        {
            sources[0].volume = volume;
            sources[1].volume = volume;
        }
        // ミュートにするか
        public void ChangeMute(bool value)
        {
            sources[0].mute = value;
            sources[1].mute = value;
        }

        // 再生不能にするか(いかなる場合でも音を鳴らせなくする)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;

        // 音声が止まったかの処理
        private IEnumerator CheckAudioStopped()
        {
            HasSoundStopped = false;

            while (sources[0].isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            HasSoundStopped = true;
            Debug.Log("Sound Stopped");
        }
    }
}


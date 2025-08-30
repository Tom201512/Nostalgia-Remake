using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class BgmPlayer : MonoBehaviour
    {
        // BGMプレイヤー

        // const
        // サンプリングレート
        const int SampleRate = 44100;

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
        public double LoopTime { get; private set; }
        // 現在サンプル数
        public int SampleCount { get; private set; }
        // 現在のループ長さサンプル数で表示
        public int DurationSample { get; private set; }
        // 実際の差分
        public double Diff {  get; private set; }

        void Awake()
        {
            HasSoundStopped = true;
            HasLockPlaying = false;
            Debug.Log("Count:" + sources.Length);

            LoopStart = -1;
            LoopLength = -1;
            LoopTime = 0;
            DurationSample = 0;
            Diff = 0;
        }

        void Update()
        {
            if (sources[0].isPlaying)
            {
                SampleCount = sources[0].timeSamples;
            }
            else
            {
                SampleCount = sources[1].timeSamples;
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
                StopAudio();
                sources[0].loop = false;
                sources[0].clip = soundSource;
                sources[0].clip.LoadAudioData();
                sources[0].Play();
                StartCoroutine(nameof(CheckAudioStopped));
            }
        }

        // ループ再生
        public void PlayBGMLoop(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                StopAudio();
                HasLoop = true;
                // ソースを2つ使ってループ再生を実現
                sources[0].loop = false;
                sources[0].clip = soundSource;
                sources[0].clip.LoadAudioData();
                sources[0].PlayScheduled(AudioSettings.dspTime);

                // 初期化

                // ループを準備
                sources[1].clip = soundSource;
                sources[1].clip.LoadAudioData();
                sources[1].loop = false;

                // 開始時間記録
                Debug.Log("LoopStartTime:" + AudioSettings.dspTime);

                if (loopStart > -1)
                {
                    LoopStart = loopStart;
                }
                else
                {
                    LoopStart = 0;
                }
                sources[1].timeSamples = LoopStart;


                // 長さ付きの場合は追加
                if (loopLength > -1)
                {
                    LoopLength = loopLength;
                }
                // ない場合は自動計算
                else
                {
                    LoopLength = sources[0].clip.samples - loopStart;
                }

                // 長さのサンプル数計算
                if (loopLength > -1)
                {
                    DurationSample = loopStart + loopLength;
                }
                else
                {
                    DurationSample = sources[0].clip.samples;
                }

                Debug.Log("Duration:" + DurationSample);
                LoopTime = AudioSettings.dspTime + (double)DurationSample / SampleRate;
                Debug.Log("Next loop is:" + LoopTime);

                StartCoroutine(nameof(LoopCheck));
            }
        }

        // 音停止
        public void StopAudio()
        {
            sources[0].loop = false;
            sources[0].Stop();
            sources[1].Stop();
            StopLoopCheck();

            LoopStart = -1;
            LoopLength = -1;
            HasLoop = false;
        }

        // ループ処理を切る
        public void StopLoopCheck() => StopCoroutine(nameof(LoopCheck));

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

        // 開始位置までシークする
        public void SeekToStartLoop()
        {
            Debug.Log("Looped");
            Debug.Log("DSP:" + AudioSettings.dspTime);
            sources[0].Stop();

            Diff = AudioSettings.dspTime - LoopTime;
            Debug.Log("Diff:" + Diff);
            sources[1].PlayScheduled(AudioSettings.dspTime);

            Debug.Log("LoopStartTime:" + AudioSettings.dspTime);

            if (LoopStart > -1)
            {
                sources[1].timeSamples = LoopStart;
            }
            else
            {
                sources[1].timeSamples = 0;
            }

            if (LoopLength > -1)
            {
                DurationSample = LoopLength;
            }
            else
            {
                DurationSample = sources[1].clip.samples;
            }

            Debug.Log("Duration:" + DurationSample);
            LoopTime = AudioSettings.dspTime + (double)DurationSample / SampleRate;
            Debug.Log("Next loop is:" + LoopTime);
        }

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

        private IEnumerator LoopCheck()
        {
            while(HasLoop)
            {
                Debug.Log("Loop Check start");
                
                while(AudioSettings.dspTime < LoopTime)
                {
                    yield return new WaitForEndOfFrame();
                }
                SeekToStartLoop();
            }
        }
    }
}


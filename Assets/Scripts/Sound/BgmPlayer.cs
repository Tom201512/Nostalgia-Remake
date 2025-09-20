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
        public double LoopTime { get; private set; }

        // 使用中のトラック
        private int usingTrackIndex;

        void Awake()
        {
            HasSoundStopped = true;
            HasLockPlaying = false;
            usingTrackIndex = 0;

            LoopStart = -1;
            LoopLength = -1;
            LoopTime = 0;
        }

        void Update()
        {
            if (HasLoop)
            {
                // ループ時間を過ぎたら次のトラックを準備
                if(AudioSettings.dspTime + 1.0 >= LoopTime)
                {
                    PrepareLoopTrack();
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
                StopAudio();
                sources[usingTrackIndex].loop = false;
                sources[usingTrackIndex].clip = soundSource;
                sources[usingTrackIndex].Play();
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

                foreach (AudioSource source in sources)
                {
                    source.loop = false;
                    source.clip = soundSource;
                    source.clip.LoadAudioData();
                }

                // ループ始点記録
                if (loopStart > -1)
                {
                    LoopStart = loopStart;
                }
                else
                {
                    LoopStart = 0;
                }
                // ループ長さ記録
                if (loopLength > -1)
                {
                    LoopLength = loopLength;
                }
                // ない場合は音源の長さを割り当てる
                else
                {
                    LoopLength = sources[usingTrackIndex].clip.samples;
                }

                //Debug.Log("Loop start at:" + AudioSettings.dspTime);

                // 最初のトラックを再生。ループのスタート位置になったらトラックを切り替える
                // 初回は0.1秒ほど遅らせる
                LoopTime = AudioSettings.dspTime + 0.1;
                sources[usingTrackIndex].PlayScheduled(LoopTime);
                LoopTime += (double)LoopStart / SampleRate;
                // ループさせる長さのサンプルを計算
                //Debug.Log("Loop Samples:" + samples);
                Debug.Log("Loop start at:" + LoopTime);
            }
        }

        // 音停止
        public void StopAudio()
        {
            sources[0].Stop();
            sources[1].Stop();
            LoopStart = -1;
            LoopLength = -1;
            HasLoop = false;
            usingTrackIndex = 0;
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

        // ループ音源の準備関数
        private void PrepareLoopTrack()
        {
            // 今鳴らしている方を終了させる
            sources[usingTrackIndex].SetScheduledEndTime(LoopTime);
            //Debug.Log("Current:" + usingTrackIndex);
            // トラック切り替え
            if (usingTrackIndex < sources.Length - 1)
            {
                usingTrackIndex++;
            }
            else
            {
                usingTrackIndex = 0;
            }

            // ループを準備
            sources[usingTrackIndex].timeSamples = LoopStart;
            sources[usingTrackIndex].PlayScheduled(LoopTime);

            //Debug.Log("Loop Samples:" + samples);
            LoopTime += (double)LoopLength / SampleRate;
            Debug.Log("Next loop is:" + LoopTime);
            Debug.Log("Prepared Loop");
        }

        // 音声が止まったかの処理
        private IEnumerator CheckAudioStopped()
        {
            HasSoundStopped = false;

            while (sources[usingTrackIndex].isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            HasSoundStopped = true;
            //Debug.Log("Sound Stopped");
        }
    }
}


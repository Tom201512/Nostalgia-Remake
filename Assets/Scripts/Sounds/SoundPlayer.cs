using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace ReelSpinGame_Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        // サウンドプレイヤー

        // var
        // コンポーネント
        private AudioSource audioSource;
        // 再生が終了したか
        public bool HasSoundStopped { get; private set; }
        // ループしている音があるか
        public bool HasLoop { get; private set; }
        // ループ開始位置
        public int LoopStart {  get; private set; }
        // ループ長さ
        public int LoopLength { get; private set; }
        // 鳴らさないようにするか
        public bool HasLockPlaying;

        void Awake()
        {
            HasSoundStopped = true;
            HasLockPlaying = false;
            audioSource = GetComponent<AudioSource>();
            LoopStart = -1;
            LoopLength = -1;
        }

        private void Update()
        {
            // 位置指定付きのループがある場合は巻き戻す
            if(HasLoop && LoopStart > -1 && LoopLength > -1)
            {
                if (audioSource.timeSamples >= LoopStart + LoopLength)
                {
                    audioSource.timeSamples -= LoopLength;
                }
            }
        }

        private void OnDestroy()
        {
            StopAudio();
            StopAllCoroutines();
        }

        // func
        // 音再生
        public void PlayAudio(AudioClip soundSource, bool hasLoop)
        {
            if(!HasLockPlaying)
            {
                LoopStart = -1;
                LoopLength = -1;

                audioSource.loop = hasLoop;
                audioSource.clip = soundSource;
                audioSource.Play();
                StartCoroutine(nameof(CheckAudioStopped));

                HasLoop = hasLoop;
            }
        }

        // 音再生(ループ位置指定あり)
        public void PlayAudio(AudioClip soundSource, bool hasLoop, int loopStart, int loopLength)
        {
            // ループ位置指定を指定して再生
            if (!HasLockPlaying)
            {
                PlayAudio(soundSource, hasLoop);
                LoopStart = loopStart;
                LoopLength = loopLength;
            }
        }

        // 音停止
        public void StopAudio()
        {
            audioSource.Stop();
            HasLoop = false;
        }

        // 一回だけ再生
        public void PlayAudioOneShot(AudioClip soundSource)
        {
            if(!HasLockPlaying)
            {
                audioSource.PlayOneShot(soundSource);
            }
        }

        // 音声が止まったかの処理
        public IEnumerator CheckAudioStopped()
        {
            HasSoundStopped = false;

            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            HasSoundStopped = true;
            //Debug.Log("Sound Stopped");
        }

        // ボリューム調整
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // ミュートにするか
        public void ChangeMute(bool value) => audioSource.mute = value;
        // 再生不能にするか(いかなる場合でも音を鳴らせなくする)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;
    }
}

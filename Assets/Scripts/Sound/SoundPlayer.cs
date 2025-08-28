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
            if(audioSource.loop && LoopStart > -1 && LoopLength > -1)
            {
                Debug.Log("Sample" + audioSource.timeSamples);
                Debug.Log("LoopEnd" + (LoopStart + LoopLength));

                if (audioSource.timeSamples >= LoopStart + LoopLength)
                {
                    audioSource.timeSamples -= LoopLength;
                    Debug.Log("Looped at:" + audioSource.timeSamples);
                }
            }
        }

        private void OnDestroy()
        {
            StopAudio();
            StopAllCoroutines();
        }

        // func
        // 一回だけ再生(重複可能)
        public void PlayAudioOneShot(AudioClip soundSource)
        {
            if (!HasLockPlaying)
            {
                audioSource.PlayOneShot(soundSource);
            }
        }

        // 再生&待機
        public void PlayAudioAndWait(AudioClip soundSource)
        {
            if (!HasLockPlaying)
            {
                audioSource.loop = false;
                audioSource.clip = soundSource;
                audioSource.Play();
                StartCoroutine(nameof(CheckAudioStopped));
            }
        }

        // ループ再生
        public void PlayLoopAudio(AudioClip soundSource)
        {
            if(!HasLockPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = soundSource;
                audioSource.Play();
                HasLoop = true;
                Debug.Log(audioSource.loop);
            }
        }

        // ループ再生(ループ位置指定あり)
        public void PlayLoopAudio(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                PlayLoopAudio(soundSource);
                LoopStart = loopStart;
                LoopLength = loopLength;
                Debug.Log("LoopStart:" + LoopStart);
                Debug.Log("Length:" + LoopLength);
            }
        }

        // 音停止
        public void StopAudio()
        {
            audioSource.loop = false;
            audioSource.Stop();
            HasLoop = false;
            LoopStart = -1;
            LoopLength = -1;
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
            Debug.Log("Sound Stopped");
        }

        // ボリューム調整
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // ミュートにするか
        public void ChangeMute(bool value) => audioSource.mute = value;
        // 再生不能にするか(いかなる場合でも音を鳴らせなくする)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;
    }
}

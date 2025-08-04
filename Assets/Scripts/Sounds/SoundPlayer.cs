using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        // サウンドプレイヤー

        // const 

        // var
        // コンポーネント
        private AudioSource audioSource;
        // 再生が終了したか
        public bool HasSoundStopped { get; private set; }
        // ループしている音があるか
        public bool HasLoop { get; private set; }
        // 鳴らさないようにするか
        public bool HasLockPlaying;


        void Awake()
        {
            HasSoundStopped = true;
            HasLockPlaying = false;
            audioSource = GetComponent<AudioSource>();
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
                audioSource.loop = hasLoop;
                audioSource.clip = soundSource;
                audioSource.Play();
                StartCoroutine(nameof(CheckAudioStopped));

                HasLoop = hasLoop;
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

        // 再生ロックをかけるか
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;
    }
}

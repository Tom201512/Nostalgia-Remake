using System.Collections;
using UnityEngine;

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
        // 鳴らさないようにするか
        public bool HasLockPlaying;

        private void Awake()
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
            }
        }

        // 音停止
        public void StopAudio()
        {
            audioSource.loop = false;
            audioSource.Stop();
            HasLoop = false;
            StopCoroutine(nameof(CheckAudioStopped));
        }

        // ボリューム調整
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // ミュートにするか
        public void ChangeMute(bool value) => audioSource.mute = value;
        // 再生不能にするか(いかなる場合でも音を鳴らせなくする)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;

        // 音声が止まったかの処理
        private IEnumerator CheckAudioStopped()
        {
            HasSoundStopped = false;

            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            HasSoundStopped = true;
        }
    }
}

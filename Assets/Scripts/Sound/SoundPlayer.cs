using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        // �T�E���h�v���C���[

        // var
        // �R���|�[�l���g
        private AudioSource audioSource;

        // �Đ����I��������
        public bool HasSoundStopped { get; private set; }
        // ���[�v���Ă��鉹�����邩
        public bool HasLoop { get; private set; }
        // �炳�Ȃ��悤�ɂ��邩
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
        // ��񂾂��Đ�(�d���\)
        public void PlayAudioOneShot(AudioClip soundSource)
        {
            if (!HasLockPlaying)
            {
                audioSource.PlayOneShot(soundSource);
            }
        }

        // �Đ�&�ҋ@
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

        // ���[�v�Đ�
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

        // ����~
        public void StopAudio()
        {
            audioSource.loop = false;
            audioSource.Stop();
            HasLoop = false;
            StopCoroutine(nameof(CheckAudioStopped));
        }

        // �{�����[������
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // �~���[�g�ɂ��邩
        public void ChangeMute(bool value) => audioSource.mute = value;
        // �Đ��s�\�ɂ��邩(�����Ȃ�ꍇ�ł�����点�Ȃ�����)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;

        // �������~�܂������̏���
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

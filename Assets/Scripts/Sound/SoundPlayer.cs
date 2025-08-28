using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

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
        // ���[�v�J�n�ʒu
        public int LoopStart {  get; private set; }
        // ���[�v����
        public int LoopLength { get; private set; }
        // �炳�Ȃ��悤�ɂ��邩
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
            // �ʒu�w��t���̃��[�v������ꍇ�͊����߂�
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
                Debug.Log(audioSource.loop);
            }
        }

        // ���[�v�Đ�(���[�v�ʒu�w�肠��)
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

        // ����~
        public void StopAudio()
        {
            audioSource.loop = false;
            audioSource.Stop();
            HasLoop = false;
            LoopStart = -1;
            LoopLength = -1;
        }

        // �������~�܂������̏���
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

        // �{�����[������
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // �~���[�g�ɂ��邩
        public void ChangeMute(bool value) => audioSource.mute = value;
        // �Đ��s�\�ɂ��邩(�����Ȃ�ꍇ�ł�����点�Ȃ�����)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;
    }
}

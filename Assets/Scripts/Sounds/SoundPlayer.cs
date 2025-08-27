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
        // ���Đ�
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

        // ���Đ�(���[�v�ʒu�w�肠��)
        public void PlayAudio(AudioClip soundSource, bool hasLoop, int loopStart, int loopLength)
        {
            // ���[�v�ʒu�w����w�肵�čĐ�
            if (!HasLockPlaying)
            {
                PlayAudio(soundSource, hasLoop);
                LoopStart = loopStart;
                LoopLength = loopLength;
            }
        }

        // ����~
        public void StopAudio()
        {
            audioSource.Stop();
            HasLoop = false;
        }

        // ��񂾂��Đ�
        public void PlayAudioOneShot(AudioClip soundSource)
        {
            if(!HasLockPlaying)
            {
                audioSource.PlayOneShot(soundSource);
            }
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
            //Debug.Log("Sound Stopped");
        }

        // �{�����[������
        public void AdjustVolume(float volume) => audioSource.volume = volume;
        // �~���[�g�ɂ��邩
        public void ChangeMute(bool value) => audioSource.mute = value;
        // �Đ��s�\�ɂ��邩(�����Ȃ�ꍇ�ł�����点�Ȃ�����)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;
    }
}

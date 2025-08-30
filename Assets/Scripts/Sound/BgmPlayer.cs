using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class BgmPlayer : MonoBehaviour
    {
        // BGM�v���C���[

        // const
        // �T���v�����O���[�g
        const double SampleRate = 44100.0;

        // var
        // �R���|�[�l���g
        [SerializeField] AudioSource[] sources;

        // �Đ����I��������
        public bool HasSoundStopped { get; private set; }
        // ���[�v���Ă��鉹�����邩
        public bool HasLoop { get; private set; }
        // ���[�v�J�n�ʒu
        public int LoopStart { get; private set; }
        // ���[�v����
        public int LoopLength { get; private set; }
        // �炳�Ȃ��悤�ɂ��邩
        public bool HasLockPlaying;

        // ���[�v���̎���
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
            // ���[�v������Ƃ��̏���
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

        // �Đ�
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

        // ���[�v�Đ�
        public void PlayBGMLoop(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                // �C���g���t�����[�v�Ȃ�\�[�X��2�g���ă��[�v�Đ�������
                if (loopStart > -1)
                {
                    // ������
                    loopTime = AudioSettings.dspTime;
                    LoopStart = loopStart;

                    // �����t���̏ꍇ�͒ǉ�
                    if(loopLength > -1)
                    {
                        LoopLength = loopLength;
                    }

                    sources[0].loop = false;
                    sources[0].clip = soundSource;
                    sources[0].PlayScheduled(loopTime);

                    double duration;
                    // ���[�v�����钷��������ꍇ�͂��̒����܂Ōv�Z
                    if (loopLength > -1)
                    {
                         duration = (loopStart + loopLength) / SampleRate;
                    }
                    // �Ȃ��ꍇ�͍Đ����I�����Ƃ���𒷂��Ƃ���
                    else
                    {
                         duration = sources[0].clip.samples / SampleRate;
                    }

                    Debug.Log("Duration:" + duration);
                    loopTime += duration;
                    Debug.Log("Next loop is:" + loopTime);

                    // ���[�v������
                    sources[1].clip = soundSource;
                    sources[1].loop = false;
                }
                // �Ȃ��ꍇ�͕��ʂ̃��[�v�Đ�������
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

        // ����~
        public void StopAudio()
        {
            sources[0].loop = false;
            sources[0].Stop();
            StopLoopCheck();
            HasLoop = false;
        }

        // ���[�v������؂�
        private void StopLoopCheck()
        {
            sources[1].Stop();
            LoopStart = -1;
            LoopLength = -1;
        }

        // �{�����[������
        public void AdjustVolume(float volume)
        {
            sources[0].volume = volume;
            sources[1].volume = volume;
        }
        // �~���[�g�ɂ��邩
        public void ChangeMute(bool value)
        {
            sources[0].mute = value;
            sources[1].mute = value;
        }

        // �Đ��s�\�ɂ��邩(�����Ȃ�ꍇ�ł�����点�Ȃ�����)
        public void ChangeLockPlaying(bool value) => HasLockPlaying = value;

        // �������~�܂������̏���
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


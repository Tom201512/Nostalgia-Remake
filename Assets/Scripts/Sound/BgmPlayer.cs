using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class BgmPlayer : MonoBehaviour
    {
        // BGM�v���C���[

        // const
        // �T���v�����O���[�g
        const int SampleRate = 44100;

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
        public double LoopTime { get; private set; }
        // ���݃T���v����
        public int SampleCount { get; private set; }
        // ���݂̃��[�v�����T���v�����ŕ\��
        public int DurationSample { get; private set; }
        // ���ۂ̍���
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

        // �Đ�
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

        // ���[�v�Đ�
        public void PlayBGMLoop(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                StopAudio();
                HasLoop = true;
                // �\�[�X��2�g���ă��[�v�Đ�������
                sources[0].loop = false;
                sources[0].clip = soundSource;
                sources[0].clip.LoadAudioData();
                sources[0].PlayScheduled(AudioSettings.dspTime);

                // ������

                // ���[�v������
                sources[1].clip = soundSource;
                sources[1].clip.LoadAudioData();
                sources[1].loop = false;

                // �J�n���ԋL�^
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


                // �����t���̏ꍇ�͒ǉ�
                if (loopLength > -1)
                {
                    LoopLength = loopLength;
                }
                // �Ȃ��ꍇ�͎����v�Z
                else
                {
                    LoopLength = sources[0].clip.samples - loopStart;
                }

                // �����̃T���v�����v�Z
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

        // ����~
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

        // ���[�v������؂�
        public void StopLoopCheck() => StopCoroutine(nameof(LoopCheck));

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

        // �J�n�ʒu�܂ŃV�[�N����
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


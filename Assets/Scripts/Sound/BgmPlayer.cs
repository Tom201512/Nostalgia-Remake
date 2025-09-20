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
        public double LoopTime { get; private set; }

        // �g�p���̃g���b�N
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
                // ���[�v���Ԃ��߂����玟�̃g���b�N������
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

        // �Đ�
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

        // ���[�v�Đ�
        public void PlayBGMLoop(AudioClip soundSource, int loopStart, int loopLength)
        {
            if (!HasLockPlaying)
            {
                StopAudio();
                HasLoop = true;
                // �\�[�X��2�g���ă��[�v�Đ�������

                foreach (AudioSource source in sources)
                {
                    source.loop = false;
                    source.clip = soundSource;
                    source.clip.LoadAudioData();
                }

                // ���[�v�n�_�L�^
                if (loopStart > -1)
                {
                    LoopStart = loopStart;
                }
                else
                {
                    LoopStart = 0;
                }
                // ���[�v�����L�^
                if (loopLength > -1)
                {
                    LoopLength = loopLength;
                }
                // �Ȃ��ꍇ�͉����̒��������蓖�Ă�
                else
                {
                    LoopLength = sources[usingTrackIndex].clip.samples;
                }

                //Debug.Log("Loop start at:" + AudioSettings.dspTime);

                // �ŏ��̃g���b�N���Đ��B���[�v�̃X�^�[�g�ʒu�ɂȂ�����g���b�N��؂�ւ���
                // �����0.1�b�قǒx�点��
                LoopTime = AudioSettings.dspTime + 0.1;
                sources[usingTrackIndex].PlayScheduled(LoopTime);
                LoopTime += (double)LoopStart / SampleRate;
                // ���[�v�����钷���̃T���v�����v�Z
                //Debug.Log("Loop Samples:" + samples);
                Debug.Log("Loop start at:" + LoopTime);
            }
        }

        // ����~
        public void StopAudio()
        {
            sources[0].Stop();
            sources[1].Stop();
            LoopStart = -1;
            LoopLength = -1;
            HasLoop = false;
            usingTrackIndex = 0;
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

        // ���[�v�����̏����֐�
        private void PrepareLoopTrack()
        {
            // ���炵�Ă�������I��������
            sources[usingTrackIndex].SetScheduledEndTime(LoopTime);
            //Debug.Log("Current:" + usingTrackIndex);
            // �g���b�N�؂�ւ�
            if (usingTrackIndex < sources.Length - 1)
            {
                usingTrackIndex++;
            }
            else
            {
                usingTrackIndex = 0;
            }

            // ���[�v������
            sources[usingTrackIndex].timeSamples = LoopStart;
            sources[usingTrackIndex].PlayScheduled(LoopTime);

            //Debug.Log("Loop Samples:" + samples);
            LoopTime += (double)LoopLength / SampleRate;
            Debug.Log("Next loop is:" + LoopTime);
            Debug.Log("Prepared Loop");
        }

        // �������~�܂������̏���
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


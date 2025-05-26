using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        // �T�E���h�v���C���[

        // const 

        // var
        // �R���|�[�l���g
        private AudioSource audioSource;
        // �Đ����I��������
        public bool HasSoundStopped { get; private set; }


        void Awake()
        {
            HasSoundStopped = true;
            audioSource = GetComponent<AudioSource>();
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
            audioSource.loop = hasLoop;
            audioSource.clip = soundSource;
            audioSource.Play();
            StartCoroutine(nameof(CheckAudioStopped));

        }

        // ����~
        public void StopAudio()
        {
            audioSource.Stop();
        }

        // ��񂾂��Đ�
        public void PlayAudioOneShot(AudioClip soundSource) => audioSource.PlayOneShot(soundSource);

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
    }
}

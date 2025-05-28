using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundManager : MonoBehaviour
    {
        // ���Ǘ�

        // var
        // ���ʉ����X�g
        [SerializeField] private SoundEffectPack soundEffectList;
        [SerializeField] private MusicPack bgmList;

        // SE�Đ�
        [SerializeField] private SoundPlayer soundEffectPlayer;
        // BGM�Đ�
        [SerializeField] private SoundPlayer bgmPlayer;

        public SoundEffectPack SoundEffectList { get { return soundEffectList; } }
        public MusicPack BGMList { get { return bgmList; } }

        // func
        // �w�肵������1��Đ�
        public void PlaySoundOneShot(AudioClip sound)
        {
            ////Debug.Log("Played");
            soundEffectPlayer.PlayAudioOneShot(sound);
        }

        // �w�肵���������[�v�ōĐ�
        public void PlaySoundLoop(AudioClip sound)
        {
            ////Debug.Log("LoopPlayed");
            soundEffectPlayer.PlayAudio(sound, true);
        }

        // ���[�v���̉���~
        public void StopLoopSound()
        {
            if(!soundEffectPlayer.HasSoundStopped)
            {
                soundEffectPlayer.StopAudio();
            }
        }
    }
}

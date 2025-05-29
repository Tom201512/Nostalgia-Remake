using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

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

        public void Start()
        {
            //PlayBGM(bgmList.RedBGM, true);
        }

        // func
        // ���ʉ�����~�������m�F
        public bool GetSoundEffectStopped() => soundEffectPlayer.HasSoundStopped;
        // ���y����~�������m�F
        public bool GetBGMStopped() => bgmPlayer.HasSoundStopped;

        // �w�肵������1��Đ�
        public void PlaySoundOneShot(AudioClip sound)
        {
            ////Debug.Log("Played");
            soundEffectPlayer.PlayAudioOneShot(sound);
        }

        // �w�肵���������[�v�ōĐ�
        public void PlaySoundLoop(AudioClip sound)
        {
            soundEffectPlayer.PlayAudio(sound, true);
        }

        // ���[�v���̉���~
        public void StopLoopSound()
        {
            if(soundEffectPlayer.HasLoop)
            {
                soundEffectPlayer.StopAudio();
            }
        }

        // �w�肵�������Đ����I���܂ő҂�
        public void PlaySoundAndWait(AudioClip sound)
        {
            soundEffectPlayer.PlayAudio(sound, false);
        }

        // �w�肵�����y�Đ�
        public void PlayBGM(AudioClip bgm, bool hasLoop)
        {
            bgmPlayer.PlayAudio(bgm, hasLoop);
        }

        // ���y��~
        public void StopBGM() => bgmPlayer.StopAudio();
    }
}

using JetBrains.Annotations;
using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundManager : MonoBehaviour
    {
        // ���Ǘ�

        // const
        // �T�E���h�p�b�N���ʗpID
        public enum SoundDatabaseID { DefaultSound, ArrangeSound };

        // var
        // �g�p���̃T�E���h�p�b�N]
        [SerializeField] private List<SoundDatabase> SoundDatabases;

        // SE�Đ�
        [SerializeField] private SoundPlayer soundEffectPlayer;
        // BGM�Đ�
        [SerializeField] private SoundPlayer bgmPlayer;

        // �g�p���̃T�E���h�p�b�N
        public SoundDatabase SoundDB { get; private set; }

        private void Awake()
        {
            ChangeSoundPack((int)SoundDatabaseID.DefaultSound);
        }

        // func
        // ���ʉ�����~�������m�F
        public bool GetSoundEffectStopped() => soundEffectPlayer.HasSoundStopped;
        // ���ʉ������[�v���Ă��邩�m�F
        public bool GetSoundEffectHasLoop() => soundEffectPlayer.HasLoop;
        // ���y����~�������m�F
        public bool GetBGMStopped() => bgmPlayer.HasSoundStopped;
        // ���y�����[�v���Ă��邩�m�F
        public bool GetBGMHasLoop() => bgmPlayer.HasLoop;

        // �T�E���h�p�b�N�̍����ւ�
        public void ChangeSoundPack(int databaseID)
        {
            if (databaseID >= SoundDatabases.Count && databaseID < 0)
            {
               throw new System.Exception("Selected sound data is not found");
            }

            SoundDB = SoundDatabases[databaseID];
        }

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

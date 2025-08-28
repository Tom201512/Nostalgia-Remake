using ReelSpinGame_Datas;
using System.Collections.Generic;
using UnityEngine;
using ReelSpinGame_Sound.SE;
using ReelSpinGame_Sound.BGM;

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
        [SerializeField] private List<SoundPack> SoundDatabases;

        // SE�Đ�
        [SerializeField] private SoundPlayer sePlayer;
        // BGM�Đ��p
        [SerializeField] private SoundPlayer bgmPlayer;

        // �g�p���̃T�E���h�p�b�N
        public SoundPack SoundDB { get; private set; }

        private void Awake()
        {
            ChangeSoundPack((int)SoundDatabaseID.DefaultSound);
        }

        // func
        // ���ʉ�����~�������m�F
        public bool GetSoundEffectStopped() => sePlayer.HasSoundStopped;
        // ���ʉ������[�v���Ă��邩�m�F
        public bool GetSoundEffectHasLoop() => sePlayer.HasLoop;
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

        // �w�肵��SE��1��Đ�
        public void PlaySoundOneShot(SeFile se)
        {
            ////Debug.Log("Played");
            sePlayer.PlayAudioOneShot(se.SourceFile);
        }

        // �w�肵�������Đ����I���܂ő҂�
        public void PlaySEAndWait(SeFile se)
        {
            sePlayer.PlayAudio(se.SourceFile, false);
        }

        // �w�肵���������[�v�ōĐ�
        public void PlaySoundLoop(SeFile se)
        {
            sePlayer.PlayAudio(se.SourceFile, true);
        }

        // ���[�v����SE��~
        public void StopLoopSE()
        {
            if(sePlayer.HasLoop)
            {
                sePlayer.StopAudio();
            }
        }

        // �w�肵�����y�Đ�
        public void PlayBGM(BgmFile bgm)
        {
            bgmPlayer.PlayAudio(bgm.SourceFile, bgm.HasLoop, bgm.LoopStart, bgm.LoopLength);
        }

        // ���y��~
        public void StopBGM() => bgmPlayer.StopAudio();

        // �{�����[������(SE)
        public void ChangeSEVolume(float volume) => sePlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // �{�����[������(BGM
        public void ChangeBGMVolume(float volume) => bgmPlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // SE�~���[�g�؂�ւ�
        public void ChangeMuteSEPlayer(bool value) => sePlayer.ChangeMute(value);
        // BGM�~���[�g�؂�ւ�
        public void ChangeMuteBGMPlayer(bool value) => bgmPlayer.ChangeMute(value);
        // SE�Đ��s�؂�ւ�
        public void ChangeLockSEPlayer(bool value) => sePlayer.ChangeLockPlaying(value);
    }
}

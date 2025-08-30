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
        // �W���O���Đ�(�Z��BGM)
        [SerializeField] private SoundPlayer jinglePlayer;
        // BGM�Đ��p
        [SerializeField] private BgmPlayer bgmPlayer;

        // �g�p���̃T�E���h�p�b�N
        public SoundPack SoundDB { get; private set; }

        private void Awake()
        {
            ChangeSoundPack((int)SoundDatabaseID.DefaultSound);
        }

        // func
        // ���ʉ�����~�������m�F
        public bool GetSoundStopped() => sePlayer.HasSoundStopped;
        // �W���O������~�������m�F
        public bool GetJingleStopped() => jinglePlayer.HasSoundStopped;
        // ���ʉ������[�v���Ă��邩�m�F
        public bool GetSoundEffectHasLoop() => sePlayer.HasLoop;

        // �T�E���h�p�b�N�̍����ւ�
        public void ChangeSoundPack(int databaseID)
        {
            if (databaseID >= SoundDatabases.Count && databaseID < 0)
            {
               throw new System.Exception("Selected sound data is not found");
            }

            SoundDB = SoundDatabases[databaseID];
        }

        // SE�Đ�
        public void PlaySE(SeFile se)
        {
            switch(se.SeType)
            {
                case SeFile.SeFileType.Oneshot:
                    sePlayer.PlayAudioOneShot(se.SourceFile);
                    break;

                case SeFile.SeFileType.Wait:
                    sePlayer.PlayAudioAndWait(se.SourceFile);
                    break;

                case SeFile.SeFileType.Jingle:
                    jinglePlayer.PlayAudioAndWait(se.SourceFile);
                    break;

                case SeFile.SeFileType.Loop:
                    sePlayer.PlayLoopAudio(se.SourceFile);
                    break;

                default:
                    break;
            }
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
            if(bgm.HasLoop)
            {
                bgmPlayer.PlayBGMLoop(bgm.SourceFile, bgm.LoopStart, bgm.LoopLength);
            }
            else
            {
                bgmPlayer.PlayBGM(bgm.SourceFile);
            }
        }

        // ���y��~
        public void StopBGM() => bgmPlayer.StopAudio();

        // �{�����[������(SE)
        public void ChangeSEVolume(float volume) => sePlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // �{�����[������(BGM
        public void ChangeBGMVolume(float volume) => bgmPlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // SE�~���[�g�؂�ւ�
        public void ChangeMuteSEPlayer(bool value) => sePlayer.ChangeMute(value);
        // BGM, �~���[�g�؂�ւ�
        public void ChangeMuteBGMPlayer(bool value) => bgmPlayer.ChangeMute(value);
        // SE�Đ��s�؂�ւ�
        public void ChangeLockSEPlayer(bool value) => sePlayer.ChangeLockPlaying(value);
    }
}

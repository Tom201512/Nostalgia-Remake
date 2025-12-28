using ReelSpinGame_Datas;
using ReelSpinGame_Sound.BGM;
using ReelSpinGame_Sound.SE;
using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_Sound
{
    public class SoundManager : MonoBehaviour
    {
        // 音管理

        // const
        // サウンドパック識別用ID
        public enum SoundDatabaseID { DefaultSound, ArrangeSound };

        // var
        // 使用候補のサウンドパック]
        [SerializeField] private List<SoundPack> SoundDatabases;
        // SE再生
        [SerializeField] private SoundPlayer sePlayer;
        // ジングル再生(短いBGM)
        [SerializeField] private SoundPlayer jinglePlayer;
        // BGM再生用
        [SerializeField] private BgmPlayer bgmPlayer;

        // 使用中のサウンドパック
        public SoundPack SoundDB { get; private set; }

        private void Awake()
        {
            ChangeSoundPack((int)SoundDatabaseID.DefaultSound);
        }

        // func
        // 効果音が停止したか確認
        public bool GetSoundStopped() => sePlayer.HasSoundStopped;
        // ジングルが停止したか確認
        public bool GetJingleStopped() => jinglePlayer.HasSoundStopped;
        // 効果音がループしているか確認
        public bool GetSoundEffectHasLoop() => sePlayer.HasLoop;

        // サウンドパックの差し替え
        public void ChangeSoundPack(int databaseID)
        {
            if (databaseID >= SoundDatabases.Count && databaseID < 0)
            {
                throw new System.Exception("Selected sound data is not found");
            }

            SoundDB = SoundDatabases[databaseID];
        }

        // SE再生
        public void PlaySE(SeFile se)
        {
            switch (se.SeType)
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

        // ループ中のSE停止
        public void StopLoopSE()
        {
            if (sePlayer.HasLoop)
            {
                sePlayer.StopAudio();
            }
        }

        // 指定した音楽再生
        public void PlayBGM(BgmFile bgm)
        {
            if (bgm.HasLoop)
            {
                bgmPlayer.PlayBGMLoop(bgm.SourceFile, bgm.LoopStart, bgm.LoopLength);
            }
            else
            {
                bgmPlayer.PlayBGM(bgm.SourceFile);
            }
        }

        // 音楽停止
        public void StopBGM() => bgmPlayer.StopAudio();

        // ボリューム調整(SE)
        public void ChangeSEVolume(float volume) => sePlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // ボリューム調整(BGM
        public void ChangeBGMVolume(float volume) => bgmPlayer.AdjustVolume(Mathf.Clamp(volume, 0f, 1f));
        // SEミュート切り替え
        public void ChangeMuteSEPlayer(bool value) => sePlayer.ChangeMute(value);
        // BGM, ミュート切り替え
        public void ChangeMuteBGMPlayer(bool value) => bgmPlayer.ChangeMute(value);
        // SE再生不可切り替え
        public void ChangeLockSEPlayer(bool value) => sePlayer.ChangeLockPlaying(value);
    }
}

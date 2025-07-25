using JetBrains.Annotations;
using ReelSpinGame_Datas;
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
        [SerializeField] private List<SoundDatabase> SoundDatabases;

        // SE再生
        [SerializeField] private SoundPlayer soundEffectPlayer;
        // BGM再生
        [SerializeField] private SoundPlayer bgmPlayer;

        // 使用中のサウンドパック
        public SoundDatabase SoundDB { get; private set; }

        private void Awake()
        {
            ChangeSoundPack((int)SoundDatabaseID.DefaultSound);
        }

        // func
        // 効果音が停止したか確認
        public bool GetSoundEffectStopped() => soundEffectPlayer.HasSoundStopped;
        // 効果音がループしているか確認
        public bool GetSoundEffectHasLoop() => soundEffectPlayer.HasLoop;
        // 音楽が停止したか確認
        public bool GetBGMStopped() => bgmPlayer.HasSoundStopped;
        // 音楽がループしているか確認
        public bool GetBGMHasLoop() => bgmPlayer.HasLoop;

        // サウンドパックの差し替え
        public void ChangeSoundPack(int databaseID)
        {
            if (databaseID >= SoundDatabases.Count && databaseID < 0)
            {
               throw new System.Exception("Selected sound data is not found");
            }

            SoundDB = SoundDatabases[databaseID];
        }

        // 指定した音を1回再生
        public void PlaySoundOneShot(AudioClip sound)
        {
            ////Debug.Log("Played");
            soundEffectPlayer.PlayAudioOneShot(sound);
        }

        // 指定した音をループで再生
        public void PlaySoundLoop(AudioClip sound)
        {
            soundEffectPlayer.PlayAudio(sound, true);
        }

        // ループ中の音停止
        public void StopLoopSound()
        {
            if(soundEffectPlayer.HasLoop)
            {
                soundEffectPlayer.StopAudio();
            }
        }

        // 指定した音を再生し終わるまで待つ
        public void PlaySoundAndWait(AudioClip sound)
        {
            soundEffectPlayer.PlayAudio(sound, false);
        }

        // 指定した音楽再生
        public void PlayBGM(AudioClip bgm, bool hasLoop)
        {
            bgmPlayer.PlayAudio(bgm, hasLoop);
        }

        // 音楽停止
        public void StopBGM() => bgmPlayer.StopAudio();
    }
}

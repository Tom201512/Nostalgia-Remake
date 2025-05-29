using System.Collections;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;

namespace ReelSpinGame_Sound
{
    public class SoundManager : MonoBehaviour
    {
        // 音管理

        // var
        // 効果音リスト
        [SerializeField] private SoundEffectPack soundEffectList;
        [SerializeField] private MusicPack bgmList;

        // SE再生
        [SerializeField] private SoundPlayer soundEffectPlayer;
        // BGM再生
        [SerializeField] private SoundPlayer bgmPlayer;

        public SoundEffectPack SoundEffectList { get { return soundEffectList; } }
        public MusicPack BGMList { get { return bgmList; } }

        public void Start()
        {
            //PlayBGM(bgmList.RedBGM, true);
        }

        // func
        // 効果音が停止したか確認
        public bool GetSoundEffectStopped() => soundEffectPlayer.HasSoundStopped;
        // 音楽が停止したか確認
        public bool GetBGMStopped() => bgmPlayer.HasSoundStopped;

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

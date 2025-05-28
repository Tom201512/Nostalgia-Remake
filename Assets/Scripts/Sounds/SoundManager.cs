using UnityEngine;

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

        // func
        // 指定した音を1回再生
        public void PlaySoundOneShot(AudioClip sound)
        {
            ////Debug.Log("Played");
            soundEffectPlayer.PlayAudioOneShot(sound);
        }

        // 指定した音をループで再生
        public void PlaySoundLoop(AudioClip sound)
        {
            ////Debug.Log("LoopPlayed");
            soundEffectPlayer.PlayAudio(sound, true);
        }

        // ループ中の音停止
        public void StopLoopSound()
        {
            if(!soundEffectPlayer.HasSoundStopped)
            {
                soundEffectPlayer.StopAudio();
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using ReelSpinGame_Sound;
using System.Collections.Generic;

namespace ReelSpinGame_Datas
{
	// サウンドデータベース
	public class SoundDatabase : ScriptableObject
    {
        // var
        // 効果音データ
        [SerializeField] private SoundEffectPack sound;
        // 音楽データ
        [SerializeField] private MusicPack music;

        public SoundEffectPack Sound { get { return sound; } }
        public MusicPack Music { get { return music; } }

        // func
        public void SetSoundEffectPack(SoundEffectPack sound) => this.sound = sound;
        public void SetMusicPack(MusicPack music) => this.music = music;
    }
}
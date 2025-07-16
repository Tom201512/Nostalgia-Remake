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
        [SerializeField] private SoundEffectPack se;
        // 音楽データ
        [SerializeField] private BGMPack bgm;

        public SoundEffectPack SE { get { return se; } }
        public BGMPack BGM { get { return bgm; } }

        // func
        public void SetSoundEffectPack(SoundEffectPack sound) => this.se = sound;
        public void SetMusicPack(BGMPack music) => this.bgm = music;
    }
}
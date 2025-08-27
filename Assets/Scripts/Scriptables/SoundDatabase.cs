using ReelSpinGame_Sound;
using UnityEngine;

namespace ReelSpinGame_Datas
{
	// サウンドデータベース
	public class SoundDatabase : ScriptableObject
    {
        // var
        // 効果音データ
        [SerializeField] private SEPack se;
        // 音楽データ
        [SerializeField] private BGMPack bgm;

        public SEPack SE { get { return se; } }
        public BGMPack BGM { get { return bgm; } }

        // func
        public void SetSoundEffectPack(SEPack sound) => se = sound;
        public void SetMusicPack(BGMPack music) => bgm = music;
    }
}
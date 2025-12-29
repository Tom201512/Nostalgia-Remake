using ReelSpinGame_Sound.BGM;
using ReelSpinGame_Sound.SE;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // サウンドデータベース
    [CreateAssetMenu(fileName = "SoundPack", menuName = "Nostalgia/GenerateSoundPack", order = 5)]
    public class SoundPack : ScriptableObject
    {
        [SerializeField] private SePack se;        // 効果音データ
        [SerializeField] private BgmPack bgm;       // 音楽データ

        public SePack SE { get => se; }
        public BgmPack BGM { get => bgm; }
    }
}
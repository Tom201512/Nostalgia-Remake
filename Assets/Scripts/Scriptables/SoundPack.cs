using ReelSpinGame_Sound.SE;
using ReelSpinGame_Sound.BGM;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // サウンドデータベース
    [CreateAssetMenu(fileName = "SoundPack", menuName = "Nostalgia/GenerateSoundPack", order = 5)]
    public class SoundPack : ScriptableObject
    {
        // var
        // 効果音データ
        [SerializeField] private SePack se;
        // 音楽データ
        [SerializeField] private BgmPack bgm;

        public SePack SE
        { 
            get { return se; }
        }
        public BgmPack BGM
        {
            get { return bgm; }
        }
    }
}
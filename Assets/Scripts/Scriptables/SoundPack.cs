using ReelSpinGame_Sound.SE;
using ReelSpinGame_Sound.BGM;
using UnityEngine;

namespace ReelSpinGame_Datas
{
	// サウンドデータベース
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

        public void SetSE(SePack se) => this.se = se;
        public void SetBGM(BgmPack bgm) => this.bgm = bgm;
    }
}
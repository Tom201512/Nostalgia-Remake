// BGMのデータセット
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.BGM
{
    [Serializable]
    public class BgmPack : ScriptableObject
    {
        // var

        // 赤7
        [SerializeField] private BgmFile redBGM;
        [SerializeField] private BgmFile redJAC;

        public BgmFile RedBGM
        {
            get { return redBGM; }
#if UNITY_EDITOR
            set { redBGM = value; }
#endif
        }

        public BgmFile RedJAC
        {
            get { return redJAC; }
#if UNITY_EDITOR
            set { redJAC = value; }
#endif
        }

        // 青7
        [SerializeField] private BgmFile blueBGM;
        [SerializeField] private BgmFile blueJAC;

        public BgmFile BlueBGM
        {
            get { return blueBGM; }
#if UNITY_EDITOR
            set { blueBGM = value; }
#endif
        }

        public BgmFile BlueJAC
        {
            get { return blueJAC; }
#if UNITY_EDITOR
            set { blueJAC = value; }
#endif
        }

        // BB7(黒BBと称する)
        [SerializeField] private BgmFile blackBGM;
        [SerializeField] private BgmFile blackJAC;

        public BgmFile BlackBGM
        {
            get { return blackBGM; }
#if UNITY_EDITOR
            set { blackBGM = value; }
#endif
        }

        public BgmFile BlackJAC
        {
            get { return blackJAC; }
#if UNITY_EDITOR
            set { blackJAC = value; }
#endif
        }

        // REG
        [SerializeField] private BgmFile regJAC;
        public BgmFile RegJAC
        {
            get { return regJAC; }
#if UNITY_EDITOR
            set { regJAC = value; }
#endif
        }

        // Nullチェック
        public bool NullCheck()
        {
            BgmFile[] soundLists = new BgmFile[]
            {
                redBGM,
                redJAC,
                blueBGM,
                blueJAC,
                blackBGM,
                blackJAC,
                regJAC
            };

            foreach (BgmFile bgm in soundLists)
            {
                if (bgm == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
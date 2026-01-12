// BGMのデータセット
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.BGM
{
    [CreateAssetMenu(fileName = "BGM_Pack", menuName = "Nostalgia/GenerateBGMPack", order = 4)]
    [Serializable]
    public class BgmPack : ScriptableObject
    {
        // 赤7
        [SerializeField] private BgmFile redBGM;
        [SerializeField] private BgmFile redJAC;

        public BgmFile RedBGM
        {
            get => redBGM;
#if UNITY_EDITOR
            set => redBGM = value;
#endif
        }

        public BgmFile RedJAC
        {
            get => redJAC;
#if UNITY_EDITOR
            set => redJAC = value;
#endif
        }

        // 青7
        [SerializeField] private BgmFile blueBGM;
        [SerializeField] private BgmFile blueJAC;

        public BgmFile BlueBGM
        {
            get => blueBGM;
#if UNITY_EDITOR
            set { blueBGM = value; }
#endif
        }

        public BgmFile BlueJAC
        {
            get => blueJAC;
#if UNITY_EDITOR
            set => blueJAC = value;
#endif
        }

        // BB7(黒BBと称する)
        [SerializeField] private BgmFile blackBGM;
        [SerializeField] private BgmFile blackJAC;

        public BgmFile BlackBGM
        {
            get => blackBGM;
#if UNITY_EDITOR
            set => blackBGM = value;
#endif
        }

        public BgmFile BlackJAC
        {
            get => blackJAC;
#if UNITY_EDITOR
            set => blackJAC = value;
#endif
        }

        // REG
        [SerializeField] private BgmFile regJAC;
        public BgmFile RegJAC
        {
            get => regJAC;
#if UNITY_EDITOR
            set => regJAC = value;
#endif
        }

        // エラー時
        [SerializeField] private BgmFile error;
        public BgmFile Error
        {
            get => error;
#if UNITY_EDITOR
            set => error = value;
#endif
        }

        // 打ち止め時
        [SerializeField] private BgmFile gameover;
        public BgmFile GameOver
        {
            get => gameover;
#if UNITY_EDITOR
            set => gameover = value;
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
                regJAC,
                error,
                gameover,
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
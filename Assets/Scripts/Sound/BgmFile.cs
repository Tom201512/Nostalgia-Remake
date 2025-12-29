// BGMファイル
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.BGM
{
    [CreateAssetMenu(fileName = "BGM_File", menuName = "Nostalgia/GenerateBGMFile", order = 3)]
    [Serializable]
    public class BgmFile : ScriptableObject
    {
        [SerializeField] private AudioClip sourceFile;      // 音源ファイル
        [SerializeField] private bool hasLoop;              // ループするか
        [SerializeField] private int loopStart = -1;        // ループ始点
        [SerializeField] private int loopLength = -1;       // ループ長さ

        public AudioClip SourceFile
        {
            get => sourceFile;
#if UNITY_EDITOR
            set => sourceFile = value;
#endif
        }

        public bool HasLoop { get { return hasLoop; } }
        public int LoopStart { get { return loopStart; } }
        public int LoopLength { get { return loopLength; } }
    }
}
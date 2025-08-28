// BGMファイル
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.BGM
{
    [Serializable]
    public class BgmFile : ScriptableObject
    {
        // 音源ファイル
        [SerializeField] private AudioClip sourceFile;
        // ループするか
        [SerializeField] private bool hasLoop;
        // ループ始点
        [SerializeField] private int loopStart = -1;
        // ループ長さ
        [SerializeField] private int loopLength = -1;

        public AudioClip SourceFile
        {
            get { return sourceFile; }
#if UNITY_EDITOR
            set { sourceFile = value; }
#endif
        }

        public bool HasLoop { get { return hasLoop; } }
        public int LoopStart { get { return loopStart; } }
        public int LoopLength { get { return loopLength; } }
    }
}
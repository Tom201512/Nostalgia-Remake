// SEファイル
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    [Serializable]
    public class SeFile : ScriptableObject
    {
        // 音源ファイル
        [SerializeField] private AudioClip sourceFile;
        // ループするか(位置指定には非対応)
        [SerializeField] private bool hasLoop;

        public AudioClip SourceFile
        {
            get { return sourceFile; }
#if UNITY_EDITOR
            set { sourceFile = value; }
#endif
        }

        public bool HasLoop { get { return hasLoop; } }
    }
}
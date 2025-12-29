// SEファイル
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    [CreateAssetMenu(fileName = "SE_File", menuName = "Nostalgia/GenerateSEFile", order = 1)]
    [Serializable]
    public class SeFile : ScriptableObject
    {
        // 音声ファイルのタイプ
        public enum SeFileType { Oneshot, Wait, Jingle, Loop }

        // 音源ファイル
        [SerializeField] private AudioClip sourceFile;
        [SerializeField] private SeFileType seType;

        public AudioClip SourceFile
        {
            get => sourceFile;
#if UNITY_EDITOR
            set => sourceFile = value;
#endif
        }

        public SeFileType SeType { get => seType; }
    }
}
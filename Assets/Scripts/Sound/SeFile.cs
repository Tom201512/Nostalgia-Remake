// SEファイル
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    [CreateAssetMenu(fileName = "SE_File", menuName = "Nostalgia/GenerateSEFile", order = 1)]
    [Serializable]
    public class SeFile : ScriptableObject
    {
        // const
        // 音声ファイルのタイプ(Oneshot: 一度だけ鳴らす, Wait: 鳴らしたら一度待機させる, Loop: 鳴らしたらループさせる)
        public enum SeFileType {Oneshot, Jingle, Loop}

        // var
        // 音源ファイル
        [SerializeField] private AudioClip sourceFile;
        [SerializeField] private SeFileType seType;

        public AudioClip SourceFile
        {
            get { return sourceFile; }
#if UNITY_EDITOR
            set { sourceFile = value; }
#endif
        }

        public SeFileType SeType { get { return seType; } }
    }
}
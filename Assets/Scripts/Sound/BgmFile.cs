// BGM�t�@�C��
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.BGM
{
    [Serializable]
    public class BgmFile : ScriptableObject
    {
        // �����t�@�C��
        [SerializeField] private AudioClip sourceFile;
        // ���[�v���邩
        [SerializeField] private bool hasLoop;
        // ���[�v�n�_
        [SerializeField] private int loopStart = -1;
        // ���[�v����
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
// SE�t�@�C��
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    [Serializable]
    public class SeFile : ScriptableObject
    {
        // �����t�@�C��
        [SerializeField] private AudioClip sourceFile;
        // ���[�v���邩(�ʒu�w��ɂ͔�Ή�)
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
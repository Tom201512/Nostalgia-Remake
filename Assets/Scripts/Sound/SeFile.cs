// SE�t�@�C��
using System;
using UnityEngine;

namespace ReelSpinGame_Sound.SE
{
    [CreateAssetMenu(fileName = "SE_File", menuName = "Nostalgia/GenerateSEFile", order = 1)]
    [Serializable]
    public class SeFile : ScriptableObject
    {
        // const
        // �����t�@�C���̃^�C�v(Oneshot: ��x�����炷, Wait: �炵�����x�ҋ@������, Loop: �炵���烋�[�v������)
        public enum SeFileType {Oneshot, Jingle, Loop}

        // var
        // �����t�@�C��
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
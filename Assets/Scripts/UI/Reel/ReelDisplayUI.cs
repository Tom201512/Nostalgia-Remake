using ReelSpinGame_Reels;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // ���[�����ʕ\��

        // var
        // ���[���f�B�X�v���C
        [SerializeField] List<ReelDisplayer> reelDisplayers;

        // ���[���I�u�W�F�N�g
        public List<ReelObject> ReelObjects { get; private set; }

        // ���[���̃Z�b�g
        public void SetReels(List<ReelObject> reelObjects) => ReelObjects = reelObjects;

        // �w��ʒu�̃��[���}����\��������
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayReel(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayReel(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            reelDisplayers[(int)ReelID.ReelRight].DisplayReel(rightLower, ReelObjects[(int)ReelID.ReelRight]);
        }
    }
}
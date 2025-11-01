using ReelSpinGame_Reels;
using ReelSpinGame_Reels.Array;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // ���[�����ʕ\��

        // var
        // ���[���f�B�X�v���C
        [SerializeField] List<ReelDisplayer> reelDisplayers;

        // ���[���f�[�^�𓾂�
        public List<ReelObjectPresenter> ReelObjects { get; private set; }

        // ���[���̃Z�b�g
        public void SetReels(List<ReelObjectPresenter> reelObjects) => ReelObjects = reelObjects;

        // ���[���}����\��������
        public void DisplayReels(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayReel(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayReel(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            reelDisplayers[(int)ReelID.ReelRight].DisplayReel(rightLower, ReelObjects[(int)ReelID.ReelRight]);
        }
    }
}
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
            reelDisplayers[(int)ReelID.ReelLeft].DisplayReelSymbols(leftLower, ReelObjects[(int)ReelID.ReelLeft]);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayReelSymbols(middleLower, ReelObjects[(int)ReelID.ReelMiddle]);
            reelDisplayers[(int)ReelID.ReelRight].DisplayReelSymbols(rightLower, ReelObjects[(int)ReelID.ReelRight]);
        }

        // ���[����~�ʒu��\��������
        public void DisplayPos(int leftLower, int middleLower, int rightLower)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayPos(leftLower);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayPos(middleLower);
            reelDisplayers[(int)ReelID.ReelRight].DisplayPos(rightLower);
        }

        // ���[���X�x���R�}
        public void DisplayDelay(int leftDelay, int middleDelay, int rightDelay)
        {
            reelDisplayers[(int)ReelID.ReelLeft].DisplayDelay(leftDelay);
            reelDisplayers[(int)ReelID.ReelMiddle].DisplayDelay(middleDelay);
            reelDisplayers[(int)ReelID.ReelRight].DisplayDelay(rightDelay);
        }
    }
}
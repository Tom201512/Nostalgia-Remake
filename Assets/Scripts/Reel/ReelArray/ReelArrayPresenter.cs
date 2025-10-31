using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;

namespace ReelSpinGame_Reels.Array
{
    public class ReelArrayPresenter : MonoBehaviour
    {
        // ���[���z��v���[���^�[

        // var
        // ���[���z�񃂃f��
        private ReelArrayModel reelArrayModel;

        // �}���}�l�[�W���[
        private SymbolManager symbolManager;

        void Awake()
        {
            reelArrayModel = new ReelArrayModel();
            symbolManager = GetComponentInChildren<SymbolManager>();
        }

        // ���[���z��v���[���^�[�̏�����
        public void SetReelArrayPresenter(byte[] reelArray)
        {
            reelArrayModel.ReelArray = reelArray;
        }

        // �w��ʒu����̃��[���ʒu�𓾂�
        public int GetReelPos(int currentLower, sbyte posID) => ReelObjectPresenter.OffsetReelPos(currentLower, posID);
        // �w��ʒu����̃��[���}���𓾂�
        public ReelSymbols GetReelSymbol(int currentLower, sbyte posID) => SymbolManager.ReturnSymbol(reelArrayModel.ReelArray[ReelObjectPresenter.OffsetReelPos(currentLower, posID)]);

        // �}���ʒu�̍X�V
        public void UpdateReelSymbols(int currentLower) => symbolManager.UpdateSymbolsObjects(currentLower, reelArrayModel.ReelArray);
    }
}

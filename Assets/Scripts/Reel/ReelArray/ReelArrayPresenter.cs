using System.Collections;
using System.Collections.Generic;
using TreeEditor;
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

        void Start()
        {
            symbolManager.UpdateSymbolsObjects(reelArrayModel.CurrentLower, reelArrayModel.ReelArray);
        }

        void Update()
        {

        }

        // ���[���z��v���[���^�[�̏�����
        public void SetReelArrayPresenter(int currentLower, byte[] reelArray)
        {
            reelArrayModel.CurrentLower = currentLower;
            reelArrayModel.ReelArray = reelArray;
        }

        // ���i�ʒu�𓮂���
        public void SetCurrentLower(int currentLower) => reelArrayModel.CurrentLower = currentLower;
    }
}

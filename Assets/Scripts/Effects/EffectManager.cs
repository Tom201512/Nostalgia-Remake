using ReelSpinGame_Datas;
using ReelSpinGame_Reels.Flash;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.Flash.FlashManager;

namespace ReelSpinGame_Effect
{
    public class EffectManager : MonoBehaviour
    {
        // ���[���t���b�V����T�E���h�̊Ǘ�

        // const

        // var
        // �t���b�V���@�\
        private FlashManager flashManager;
        // ���[���̃I�u�W�F�N�g
        [SerializeField] private List<ReelObject> reelObjects;

        // func 
        private void Awake()
        {
            flashManager = GetComponent<FlashManager>();
        }

        private void Start()
        {
            // ���[���I�u�W�F�N�g���蓖��
            flashManager.SetReelObjects(reelObjects);
        }

        // �t���b�V���őҋ@����
        public bool GetHasFlashWait() => flashManager.HasFlashWait;
        // �t���b�V���֘A
        // ���[���S�_��
        public void TurnOnAllReels(bool isJacGame)
        {
            // JAC GAME���͒��i�̂݌��点��
            if (isJacGame)
            {
                flashManager.EnableJacGameLight();
            }
            else
            {
                flashManager.TurnOnAllReels();
            }

            // JAC���̃��C�g����������
            foreach (ReelObject reel in reelObjects)
            {
                if (reel.HasJacModeLight != isJacGame)
                {
                    reel.HasJacModeLight = isJacGame;
                }
            }
        }

        // ���[�����C�g�S����
        public void TurnOffAllReels() => flashManager.TurnOffAllReels();
        // ���[���t���b�V�����J�n������
        public void StartReelFlash(FlashID flashID) => flashManager.StartReelFlash(flashID);
        // �����o���̃��[���t���b�V�����J�n������
        public void StartPayoutReelFlash(float waitSeconds, List<PayoutLineData> lastPayoutLines) => flashManager.StartPayoutFlash(waitSeconds, lastPayoutLines);
        // �t���b�V����~
        public void StopReelFlash() => flashManager.StopFlash();
    }
}

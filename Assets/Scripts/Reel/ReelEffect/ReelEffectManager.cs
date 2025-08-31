using ReelSpinGame_Reels.Symbol;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_Reels.Effect
{
    // ���[���G�t�F�N�g�}�l�[�W���[
    public class ReelEffectManager : MonoBehaviour
    {
        // var
        // ���[���G�t�F�N�g�̔z��
        [SerializeField] private List<ReelEffect> reelEffectList;

        private void Awake()
        {
            
        }

        // �w�肵�����[���̃o�b�N���C�g�ύX
        public void ChangeReelBackLight(int reelID, byte brightness) => reelEffectList[reelID].ChangeReelBrightness(brightness);

        // �w�肵�����[���Ɛ}���ʒu�̃��C�g�ύX
        public void ChangeReelSymbolLight(int reelID, int posID, byte brightness) => reelEffectList[reelID].ChangeSymbolBrightness(posID, brightness);

        // �S���[���̖��邳�ꊇ�ύX
        public void ChangeAllReelBrightness(byte brightness)
        {
            foreach (ReelEffect reel in reelEffectList)
            {
                reel.ChangeReelBrightness(brightness);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.ChangeSymbolBrightness(i, brightness);
                }
            }
        }

        // JAC GAME���̃��C�g�_��
        public void EnableJacGameLight()
        {
            foreach (ReelEffect reel in reelEffectList)
            {
                reel.ChangeReelBrightness(ReelBase.TurnOffValue);

                // �^�񒆈ȊO�_��
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.ChangeSymbolBrightness(i, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reel.ChangeSymbolBrightness(i, SymbolLight.TurnOffValue);
                    }
                }
            }
        }
    }
}


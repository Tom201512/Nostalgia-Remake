using ReelSpinGame_Reels.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.Array.ReelArrayModel;
using static ReelSpinGame_Reels.ReelManagerModel;

namespace ReelSpinGame_Reels.Effect
{
    // ���[���G�t�F�N�g�}�l�[�W���[
    public class ReelEffectManager : MonoBehaviour
    {
        // var
        // ���[���I�u�W�F�N�g�v���[���^�[
        public List<ReelObjectPresenter> ReelObjects { get; private set; }

        // �^���V�Z����
        public bool HasFakeSpin { get; private set; }

        private void Awake()
        {
            HasFakeSpin = false;
        }

        // ���[���I�u�W�F�N�g���Z�b�g����
        public void SetReels(List<ReelObjectPresenter> reels)
        {
            ReelObjects = reels;
        }

        // �w�肵�����[���̃o�b�N���C�g�ύX
        public void ChangeReelBackLight(int reelID, byte brightness) => ReelObjects[reelID].ReelEffectManager.ChangeReelBrightness(brightness);

        // �w�肵�����[���Ɛ}���ʒu�̃��C�g�ύX
        public void ChangeReelSymbolLight(int reelID, int posID, byte brightness) => ReelObjects[reelID].ReelEffectManager.ChangeSymbolBrightness(posID, brightness);

        // �S���[���̖��邳�ꊇ�ύX
        public void ChangeAllReelBrightness(byte brightness)
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(brightness);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.ReelEffectManager.ChangeSymbolBrightness(i, brightness);
                }
            }
        }

        // JAC GAME���̃��C�g�_��
        public void EnableJacGameLight()
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.ReelEffectManager.ChangeReelBrightness(ReelBase.TurnOffValue);

                // �^�񒆈ȊO�_��
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(i, SymbolLight.TurnOnValue);
                    }
                    else
                    {
                        reel.ReelEffectManager.ChangeSymbolBrightness(i, SymbolLight.TurnOffValue);
                    }
                }
            }
        }

        // JAC���̖��邳�v�Z�̐ݒ�
        public void SetJacBrightnessCalculation(bool value)
        {
            foreach (ReelObjectPresenter reel in ReelObjects)
            {
                reel.SetJacBrightnessCalculate(value);
            }
        }

        // �^���V�Z
        public void StartFakeSpin()
        {
            HasFakeSpin = true;
            StartCoroutine(nameof(FakeSpinTestA));
        }

        // �^���V�Z�e�X�g
        private IEnumerator FakeSpinTestA()
        {
            Debug.Log("�^���V�Z���J�n���܂����B");
            
            Debug.Log("�E���[����1�b��ɉ�]�����܂��B");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelRight].StartReel(0.98f, true);

            Debug.Log("�����[����1�b��ɉ�]�����܂��B");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelMiddle].StartReel(0.98f, true);

            Debug.Log("�����[����1�b��ɉ�]�����܂��B");
            yield return new WaitForSeconds(1.0f);
            ReelObjects[(int)ReelID.ReelLeft].StartReel(0.98f, true);

            Debug.Log("3�b��ɋ^���V�Z���I�����܂�");
            yield return new WaitForSeconds(3.0f);
            Debug.Log("�^���V�Z���I�����܂���");
            HasFakeSpin = false;
        }
    }
}


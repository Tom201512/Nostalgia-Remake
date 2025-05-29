using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_Reels.Flash
{
    public class FlashManager : MonoBehaviour
    {
        // ���[���t���b�V���@�\

        // const
        // ���[���t���b�V���̊Ԋu(�b�Ԋu)
        public const float ReelFlashTime = 0.01f;
        // �����o�����̃t���b�V���ɗv����t���[����(0.01�b�Ԋu)
        public const int PayoutFlashFrames = 15;
        // �V�[�N�ʒu�I�t�Z�b�g�p
        const int SeekOffset = 4;
        // �ύX���Ȃ��Ƃ��̐��l
        const int NoChangeValue = -1;

        // �f�t�H���g�̃t���b�V��ID
        public enum FlashID { V_Flash };
        // �����o�����C����ID
        public enum PayoutLineID { PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB };

        // var
        // ���݂̃t���[����(1�t���[��0.1�b)
        public int CurrentFrame { get; set; }
        // �t���b�V������
        public bool HasFlash { get; set; }
        // �t���b�V���őҋ@����
        public bool HasFlashWait { get; set; }
        // ���݂̃t���b�V��ID
        public int CurrentFlashID { get; set; }
        // �t���b�V���f�[�^
        public List<FlashData> FlashDatabase { get; private set; }
        [SerializeField] private List<TextAsset> testAssetList;

        // func
        public void Awake()
        {
            HasFlash = false;
            HasFlashWait = false;
            CurrentFlashID = 0;
            FlashDatabase = new List<FlashData>();

            foreach (TextAsset textAsset in testAssetList)
            {
                StringReader buffer = new StringReader(textAsset.text);
                FlashDatabase.Add(new FlashData(buffer));
            }

            ////Debug.Log("FlashManager awaken");
        }

        // �t���b�V���f�[�^�̏����𔽉f����
        public void ReadFlashData(ReelObject[] reelObjects)
        {
            if (CurrentFlashID >= FlashDatabase.Count)
            {
                throw new Exception("FlashID is Overflow the flashDatabase");
            }

            int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();

            // ���݂̃t���[���ƈ�v���Ȃ���Γǂݍ��܂Ȃ�
            if (CurrentFrame == flashData[(int)FlashData.PropertyID.FrameID])
            {
                // ���[���S�ĕύX
                foreach (ReelObject reel in reelObjects)
                {
                    // �{�̂Ƙg���g��̐}���ύX
                    int bodyBright = flashData[(int)FlashData.PropertyID.Body + reel.GetReelID() * SeekOffset];
                    if (bodyBright != NoChangeValue)
                    {
                        reel.SetReelBaseBrightness((byte)bodyBright);
                        reel.SetSymbolBrightness((int)ReelPosID.Lower2nd, (byte)bodyBright);
                        reel.SetSymbolBrightness((int)ReelPosID.Upper2nd, (byte)bodyBright);
                    }

                    // �}���̖��邳�ύX
                    for (int i = (int)ReelPosID.Lower2nd; i < (int)ReelPosID.Upper3rd; i++)
                    {
                        int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + i + reel.GetReelID() * SeekOffset];

                        //Debug.Log("Symbol:" + i + "Bright:" + symbolBright);
                        if (symbolBright != NoChangeValue)
                        {
                            reel.SetSymbolBrightness(i, (byte)symbolBright);
                        }
                    }
                }

                // �f�[�^�̃V�[�N�ʒu�ύX
                if (!FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
                {
                    CurrentFrame += 1;
                    FlashDatabase[CurrentFlashID].MoveNextSeek();
                }
                // ���[�v�����邩(���[�v�̏ꍇ�͓���t���[���܂ňړ�������)
                // ���Ȃ��ꍇ�͒�~����B
                if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
                {
                    CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                    FlashDatabase[CurrentFlashID].SetSeek(flashData[(int)FlashData.PropertyID.LoopPosition]);
                }
                // �ŏI�s�܂ł��Ń��[�v���Ȃ��ꍇ�͏I��
                else if (FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
                {
                    HasFlash = false;
                }
            }
            else
            {
                CurrentFrame += 1;
            }
        }

        // �����o�����̃t���b�V��
        public void PayoutFlash(List<PayoutLineData> lastPayoutLines, ReelObject[] reelObjects)
        {
            // �Â�����ʂ��v�Z
            byte brightness = CalculateBrightness(SymbolChange.TurnOffValue, PayoutFlashFrames);
            //�S�Ă̕����o���̂��������C�����t���b�V��������
            foreach (PayoutLineData payoutLine in lastPayoutLines)
            {
                for (int i = 0; i < payoutLine.PayoutLines.Count; i++)
                {
                    // �}���_��
                    reelObjects[i].SetSymbolBrightness(payoutLine.PayoutLines[i], brightness);

                    // �����[���Ƀ`�F���[������ꍇ�̓`�F���[�̂ݓ_��
                    if (reelObjects[(int)ReelID.ReelLeft].GetReelSymbol
                        (payoutLine.PayoutLines[(int)ReelID.ReelLeft]) == ReelSymbols.Cherry)
                    {
                        break;
                    }
                }
            }
            // ���[�v������
            CurrentFrame += 1;

            if (CurrentFrame == PayoutFlashFrames)
            {
                CurrentFrame = 0;
            }
        }

        private byte CalculateBrightness(int turnOffValue, int frame)
        {
            // ���邳�̌v�Z(0.01�b��25������)
            int distance = SymbolChange.TurnOnValue - turnOffValue;
            float changeValue = distance / frame;
            // 0.01�b�ŉ����閾�邳�̗�(0.08�b�ł��Ƃɖ߂�)
            float result = SymbolChange.TurnOnValue - CurrentFrame * changeValue;
            // ���l�𒴂��Ȃ��悤�ɒ���
            result = Math.Clamp(result, SymbolChange.TurnOffValue, SymbolChange.TurnOnValue);
            // byte�^�ɕϊ�
            return (byte)Math.Round(result);
        }
    }

}
using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections;
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
        // ���[���I�u�W�F�N�g
        private List<ReelObject> reelObjects;
        // �Ō�ɂ����������o�����C��
        private List<PayoutLineData> lastPayoutLines;

        // ���݂̃t���[����(1�t���[��0.1�b)
        private int currentFrame;
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
            reelObjects = new List<ReelObject>();
            lastPayoutLines = new List<PayoutLineData>();

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

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // func
        // �f�[�^��n��
        // ���[���I�u�W�F�N�g
        public void SetReelObjects(List<ReelObject> reelObjects) => this.reelObjects = reelObjects;

        // ���[���t���b�V�����Đ�������
        public void StartReelFlash(float waitSeconds, FlashID flashID)
        {
            currentFrame = 0;
            HasFlash = true;
            FlashDatabase[(int)flashID].SetSeek(0);
            StartCoroutine(nameof(UpdateFlash));

            if (waitSeconds > 0)
            {
                HasFlashWait = true;
                StartCoroutine(nameof(SetFlashWait), waitSeconds);
            }
        }

        // �����o���t���b�V���̍Đ�
        public void StartPayoutFlash(float waitSeconds, List<PayoutLineData> lastPayoutLines)
        {
            this.lastPayoutLines.Clear();
            this.lastPayoutLines = lastPayoutLines;
            currentFrame = 0;
            HasFlash = true;
            StartCoroutine(nameof(UpdatePayoutFlash));

            if (waitSeconds > 0)
            {
                HasFlashWait = true;
                StartCoroutine(nameof(SetTimeOut), waitSeconds);
            }
        }

        // �����o���t���b�V���̒�~
        public void StopFlash()
        {
            HasFlash = false;
            HasFlashWait = false;
        }

        // ���[�����C�g�����ׂĖ��邭����
        public void TurnOnAllReels()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(ReelBase.TurnOnValue);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.SetSymbolBrightness(i, ReelBase.TurnOnValue);
                }
            }
        }

        // ���[�����C�g�����ׂĈÂ�����
        public void TurnOffAllReels()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(ReelBase.TurnOffValue);
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    reel.SetSymbolBrightness(i, ReelBase.TurnOffValue);
                }
            }
        }

        // JAC GAME���̃��C�g�_��
        public void EnableJacGameLight()
        {
            foreach (ReelObject reel in reelObjects)
            {
                reel.SetReelBaseBrightness(SymbolChange.TurnOffValue);

                // �^�񒆈ȊO�_��
                for (int i = (int)ReelPosID.Lower2nd; i <= (int)ReelPosID.Upper2nd; i++)
                {
                    if (i == (int)ReelPosID.Center)
                    {
                        reel.SetSymbolBrightness(i, SymbolChange.TurnOnValue);
                    }
                    else
                    {
                        reel.SetSymbolBrightness(i, SymbolChange.TurnOffValue);
                    }
                }
            }
        }

        // �^�񒆂ɋ߂��Ȃ������[�������X�Ɍ��点��

        // �t���b�V���f�[�^�̏����𔽉f����
        private void ReadFlashData()
        {
            if (CurrentFlashID >= FlashDatabase.Count)
            {
                throw new Exception("FlashID is Overflow the flashDatabase");
            }

            int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();

            // ���݂̃t���[���ƈ�v���Ȃ���Γǂݍ��܂Ȃ�
            if (currentFrame == flashData[(int)FlashData.PropertyID.FrameID])
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
                    for (int i = (int)ReelPosID.Lower2nd; i < (int)ReelPosID.Upper2nd; i++)
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
                    currentFrame += 1;
                    FlashDatabase[CurrentFlashID].MoveNextSeek();
                }
                // ���[�v�����邩(���[�v�̏ꍇ�͓���t���[���܂ňړ�������)
                // ���Ȃ��ꍇ�͒�~����B
                if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
                {
                    currentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
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
                currentFrame += 1;
            }
        }

        // �����o�����̃t���b�V��
        private void PayoutFlash()
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
            currentFrame += 1;

            if (currentFrame == PayoutFlashFrames)
            {
                currentFrame = 0;
            }
        }

        private byte CalculateBrightness(int turnOffValue, int frame)
        {
            // ���邳�̌v�Z(0.01�b��25������)
            int distance = SymbolChange.TurnOnValue - turnOffValue;
            float changeValue = distance / frame;
            // 0.01�b�ŉ����閾�邳�̗�(0.08�b�ł��Ƃɖ߂�)
            float result = SymbolChange.TurnOnValue - currentFrame * changeValue;
            // ���l�𒴂��Ȃ��悤�ɒ���
            result = Math.Clamp(result, SymbolChange.TurnOffValue, SymbolChange.TurnOnValue);
            // byte�^�ɕϊ�
            return (byte)Math.Round(result);
        }

        // ���[���t���b�V���̃C�x���g
        private IEnumerator UpdateFlash()
        {
            while (HasFlash)
            {
                ReadFlashData();
                yield return new WaitForSeconds(ReelFlashTime);
            }
        }

        // �����o���t���b�V���̃C�x���g
        private IEnumerator UpdatePayoutFlash()
        {
            while (HasFlash)
            {
                PayoutFlash();
                yield return new WaitForSeconds(ReelFlashTime);
            }
            yield break;
        }

        // �^�C���A�E�g�p�C�x���g(���Ԍo�߂Ńt���b�V���I��)
        private IEnumerator SetTimeOut(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            ////Debug.Log("Replay Finished");
            HasFlashWait = false;
            HasFlash = false;
            TurnOnAllReels();
        }

        // �t���b�V���p�E�F�C�g�C�x���g
        private IEnumerator SetFlashWait(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            ////Debug.Log("Replay Finished");
            HasFlashWait = false;
        }
    }
}
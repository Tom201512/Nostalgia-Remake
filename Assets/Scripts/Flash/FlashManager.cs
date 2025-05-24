using ReelSpinGame_Datas;
using ReelSpinGame_Flash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ReelSpinGame_Reels.ReelData;

public class FlashManager : MonoBehaviour
{
    // �t���b�V���@�\

    // const
    // ���[���t���b�V���̊Ԋu(�b�Ԋu)
    const float ReelFlashTime = 0.01f;
    // �����o�����̃t���b�V���ɗv����t���[����(0.01�b�Ԋu)
    const int PayoutFlashFrames = 15;
    // �f�t�H���g�̖��邳(�_����)
    const int TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    const int TurnOffValue = 180;
    // �}����ύX����ۂ̃��[�v��
    const int SymbolLoops = 3;
    // �V�[�N�ʒu�I�t�Z�b�g�p
    const int SeekOffset = 4;
    // �ύX���Ȃ��Ƃ��̐��l
    const int NoChangeValue = -1;

    // �����o�����C����ID
    public enum PayoutLineID {PayoutMiddle, PayoutLower, PayoutUpper, PayoutDiagonalA, PayoutDiagonalB};

    // var
    // ���݂̃t���[����(1�t���[��0.1�b)
    public int CurrentFrame { get; private set; }
    // �t���b�V������
    public bool HasFlash { get; private set; }
    // ���݂̃t���b�V��ID
    public int CurrentFlashID { get; private set; }

    // ���[���I�u�W�F�N�g
    public ReelObject[] ReelObjects { get; private set; }
    // �t���b�V���f�[�^
    public List<FlashData> FlashDatabase { get; private set; }
    [SerializeField] private List<TextAsset> testAssetList;

    // func
    public void Awake()
    {
        HasFlash = false;
        CurrentFlashID = 0;
        FlashDatabase = new List<FlashData>();

        foreach(TextAsset textAsset in testAssetList)
        {
            StringReader buffer = new StringReader(textAsset.text);
            FlashDatabase.Add(new FlashData(buffer));
        }

        Debug.Log("FlashManager awaken");
    }

    public void Start()
    {
        //StartFlash(0);
    }

    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // �t���b�V���Đ�
    public void StartFlash(int flashID)
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("FlashUpdate");
        Debug.Log("Flash started");
    }

    public void StartPayoutFlash(List<PayoutLineData> lastPayoutLines)
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("PayoutFlashUpdate",lastPayoutLines);
        Debug.Log("Flash started");
    }

    // �t���b�V����~
    public void StopFlash()
    {
        HasFlash = false;
    }

    // ���[�����C�g�����ׂĖ��邭����
    public void TurnOnAllReels()
    {
        foreach(ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOnValue);
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
            }
        }
        Debug.Log("All reels are turned on");
    }

    // ���[�����C�g�����ׂĈÂ�����
    public void TurnOffAllReels()
    {
        foreach (ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOffValue);
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                Debug.Log("PosID:" + i);
                reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
            }
        }
        Debug.Log("All reels are turned off");
    }

    // JAC GAME���̃��C�g
    public void EnableJacGameLight()
    {
        foreach (ReelObject reel in ReelObjects)
        {
            reel.SetReelBaseBrightness(TurnOffValue);

            // �^�񒆈ȊO�_��
            for (int i = (int)ReelPosID.Lower3rd; i < (int)ReelPosID.Upper3rd; i++)
            {
                if (i == GetReelArrayIndex((int)ReelPosID.Center))
                {
                    reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
                }
                else
                {
                    reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
                }
            }
        }
        Debug.Log("turned on JACGAME lights");
    }

    // �t���b�V���̍X�V����
    public IEnumerator FlashUpdate()
    {
        Debug.Log("Coroutine called");
        while (HasFlash)
        {
            ReadFlashData();
            Debug.Log("Flash:" + CurrentFrame);
            yield return new WaitForSeconds(ReelFlashTime);
        }

        Debug.Log("Flash Stopped by Insert");
        yield break;
    }

    public IEnumerator PayoutFlashUpdate(List<PayoutLineData> lastPayoutLines)
    {
        Debug.Log("Payout Coroutine called");
        while (HasFlash)
        {
            PayoutFlash(lastPayoutLines);
            Debug.Log("Flash:" + CurrentFrame);
            yield return new WaitForSeconds(ReelFlashTime);
        }

        Debug.Log("Flash Stopped by Insert");
        yield break;
    }

    // �t���b�V���f�[�^�̏����𔽉f����
    public void ReadFlashData()
    {
        if(CurrentFlashID >= FlashDatabase.Count)
        {
            throw new System.Exception("FlashID is Overflow the flashDatabase");
        }

        int[] flashData = FlashDatabase[CurrentFlashID].GetCurrentFlashData();
        Debug.Log("Seek:"+ FlashDatabase[CurrentFlashID].CurrentSeekPos);

        // ���݂̃t���[���ƈ�v���Ȃ���Γǂݍ��܂Ȃ�
        Debug.Log("Segment:" + flashData[(int)FlashData.PropertyID.FrameID]);
        if (CurrentFrame == flashData[(int)FlashData.PropertyID.FrameID])
        {
            // ���[���S�ĕύX
            foreach (ReelObject reel in ReelObjects)
            {
                // �{�̕ύX
                int bodyBright = flashData[(int)FlashData.PropertyID.Body + reel.ReelData.ReelID * SeekOffset];
                Debug.Log("Change Body:" + reel.ReelData.ReelID + "Bright:" + bodyBright);
                if (bodyBright != NoChangeValue)
                {
                    reel.SetReelBaseBrightness((byte)bodyBright);
                }
                else
                {
                    Debug.Log("No changes");
                }

                Debug.Log("Change Symbols");
                // �}���̖��邳�ύX
                for (int i = 0; i < SymbolLoops; i++)
                {
                    int symbolBright = flashData[(int)FlashData.PropertyID.SymbolLower + i + reel.ReelData.ReelID * SeekOffset];

                    Debug.Log("Symbol:" + ((int)FlashData.PropertyID.SymbolLower + i) + "Bright:" + symbolBright);
                    if(symbolBright != NoChangeValue)
                    {
                        reel.SetSymbolBrightness((int)FlashData.PropertyID.SymbolLower + i, (byte)symbolBright, (byte)symbolBright, (byte)symbolBright);
                    }
                    else
                    {
                        Debug.Log("No changes");
                    }
                }
            }

            // �f�[�^�̃V�[�N�ʒu�ύX
            if (!FlashDatabase[CurrentFlashID].HasSeekReachedEnd())
            {
                CurrentFrame += 1;
                FlashDatabase[CurrentFlashID].MoveNextSeek();
                Debug.Log("SeekMoved");
                Debug.Log("NextFrame");
            }

            // ���[�v�����邩(���[�v�̏ꍇ�͓���t���[���܂ňړ�������)
            // ���Ȃ��ꍇ�͒�~����B
            if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
            {
                CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                FlashDatabase[CurrentFlashID].ResetSeek();
                Debug.Log("LoopFlash");
            }
        }
        else
        {
            CurrentFrame += 1;
            Debug.Log("NextFrame");
        }
    }

    // �����o�����̃t���b�V��
    public void PayoutFlash(List<PayoutLineData> lastPayoutLines)
    {
        // ���邳�̌v�Z(0.01�b��25������)
        int distance = TurnOnValue - TurnOffValue;
        float changeValue = distance / PayoutFlashFrames;
        // 0.01�b�ŉ����閾�邳�̗�(0.08�b�ł��Ƃɖ߂�)
        float result = TurnOnValue - CurrentFrame * changeValue;
        // ���l�𒴂��Ȃ��悤�ɒ���
        result = Math.Clamp(result, TurnOffValue, TurnOnValue);
        // byte�^�ɕϊ�
        byte brightness = (byte)Math.Round(result);

        //�S�Ă̕����o���̂��������C�����t���b�V��������
        foreach (PayoutLineData payoutLine in lastPayoutLines)
        {
            for(int i = 0; i < payoutLine.PayoutLines.Count; i++)
            {
                // �}���_��
                ReelObjects[i].SetSymbolBrightness(payoutLine.PayoutLines[i],brightness, brightness, brightness);

                // �����[���Ƀ`�F���[������ꍇ�̓`�F���[�̂ݓ_��
                if (ReelObjects[(int)ReelManager.ReelID.ReelLeft].GetReelSymbol
                    (payoutLine.PayoutLines[(int)ReelManager.ReelID.ReelLeft]) == ReelSymbols.Cherry)
                {
                    break;
                }
            }
        }

        // ���[�v������
        CurrentFrame += 1;

        if(CurrentFrame == PayoutFlashFrames)
        {
            CurrentFrame = 0;
        }
    }
}

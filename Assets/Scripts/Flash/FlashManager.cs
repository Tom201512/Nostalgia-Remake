using ReelSpinGame_Flash;
using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlashManager : MonoBehaviour
{
    // �t���b�V���@�\

    // const
    // ���[���t���b�V���̊Ԋu(�b�Ԋu)
    const float ReelFlashTime = 0.01f;
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

    // var
    // ���݂̃t���[����(1�t���[��0.1�b)
    public int CurrentFrame { get; private set; }
    // �t���b�V������
    public bool HasFlash { get; private set; }

    // ���[���I�u�W�F�N�g
    public ReelObject[] ReelObjects { get; private set; }
    // �t���b�V���f�[�^
    public List<FlashData> FlashDatabase { get; private set; }
    [SerializeField] private TextAsset testAsset;

    // func
    public void Awake()
    {
        HasFlash = false;
        FlashDatabase = new List<FlashData>();
        StringReader buffer = new StringReader(testAsset.text);
        FlashDatabase.Add(new FlashData(buffer));
        Debug.Log("FlashManager awaken");
    }

    public void Start()
    {
        StartFlash();
    }

    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // �t���b�V���Đ�
    public void StartFlash()
    {
        CurrentFrame = 0;
        HasFlash = true;
        StartCoroutine("FlashUpdate");
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
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
            for (int i = 0; i < (int)ReelData.ReelPosArrayID.Upper3rd; i++)
            {
                if(i == (int)ReelData.ReelPosArrayID.Center)
                {
                    reel.SetSymbolBrightness(i, TurnOnValue, TurnOnValue, TurnOnValue);
                }
                else
                {
                    reel.SetSymbolBrightness(i, TurnOffValue, TurnOffValue, TurnOffValue);
                }
            }
        }
        Debug.Log("All reels are turned on");
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

    // �t���b�V���f�[�^�̏����𔽉f����
    public void ReadFlashData()
    {
        int[] flashData = FlashDatabase[0].GetCurrentFlashData();
        Debug.Log("Seek:"+ FlashDatabase[0].CurrentSeekPos);

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
            if (!FlashDatabase[0].HasSeekReachedEnd())
            {
                CurrentFrame += 1;
                FlashDatabase[0].MoveNextSeek();
                Debug.Log("SeekMoved");
                Debug.Log("NextFrame");
            }

            // ���[�v�����邩(���[�v�̏ꍇ�͓���t���[���܂ňړ�������)
            // ���Ȃ��ꍇ�͒�~����B
            if (flashData[(int)FlashData.PropertyID.LoopPosition] != NoChangeValue)
            {
                CurrentFrame = flashData[(int)FlashData.PropertyID.LoopPosition];
                FlashDatabase[0].ResetSeek();
                Debug.Log("LoopFlash");
            }
        }
        else
        {
            CurrentFrame += 1;
            Debug.Log("NextFrame");
        }
    }
}

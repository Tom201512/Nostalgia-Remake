using ReelSpinGame_Reels;
using System.Collections;
using UnityEngine;

public class FlashManager : MonoBehaviour
{
    // �t���b�V���@�\

    // const
    // ���[���t���b�V���̊Ԋu(�b�Ԋu)
    const float ReelFlashTime = 0.1f;
    // �f�t�H���g�̖��邳(�_����)
    const int TurnOnValue = 255;
    // �f�t�H���g�̈Â�(������)
    const int TurnOffValue = 80;

    // var
    // ���݂̃t���[����(1�t���[��0.1�b)
    public int CurrentFrame { get; private set; }

    // ���[���I�u�W�F�N�g
    public ReelObject[] ReelObjects { get; private set; }

    // func
    public void SetReelObjects(ReelObject[] reelObjects) => ReelObjects = reelObjects;

    // �t���b�V���Đ�
    public void StartFlash()
    {
        CurrentFrame = 0;
        StartCoroutine("FlashUpdate");
        Debug.Log("Flash started");
    }

    // �t���b�V����~
    public void StopFlash()
    {
        StopCoroutine("FlashUpdate");
        Debug.Log("Flash stopped");
        TurnOnAllReels();
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

    // �t���b�V���̍X�V����
    public IEnumerator FlashUpdate()
    {
        Debug.Log("Coroutine called");
        while (true)
        {
            // �e�X�g�p
            if (CurrentFrame < 5)
            {
                Debug.Log("FlashA");
                TurnOffAllReels();
            }
            else
            {
                Debug.Log("FlashB");
                TurnOnAllReels();
            }

            CurrentFrame += 1;

            if (CurrentFrame == 9)
            {
                CurrentFrame = 0;
            }
            Debug.Log("Flash:" + CurrentFrame);

            yield return new WaitForSeconds(ReelFlashTime);
        }
    }
}

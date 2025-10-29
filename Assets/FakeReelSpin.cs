using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

public class FakeReelSpin : MonoBehaviour
{
    // �^���V�Z�p
    // var
    // �^���V�Z�Ŏg�����[���I�u�W�F�N�g
    [SerializeField] List<ReelObject> reelObjects;

    // �^���V�Z����
    public bool HasFakeReel { get; private set; }

    void Awake()
    {
        HasFakeReel = false;
    }

    public void StartFakeReelSpin()
    {
        HasFakeReel = true;
        StartCoroutine(nameof(FakeReelSpinA));
    }

    private IEnumerator FakeReelSpinA()
    {
        reelObjects[(int)ReelID.ReelLeft].StartReel(-1.5f, false);
        reelObjects[(int)ReelID.ReelMiddle].StartReel(1.5f, false);
        reelObjects[(int)ReelID.ReelRight].StartReel(-1.5f, false);

        reelObjects[(int)ReelID.ReelLeft].ChangeBlurSetting(true);
        reelObjects[(int)ReelID.ReelMiddle].ChangeBlurSetting(true);
        reelObjects[(int)ReelID.ReelRight].ChangeBlurSetting(true);

        yield return new WaitForSeconds(5.0f);
        reelObjects[(int)ReelID.ReelLeft].AdjustMaxSpeed(0.0f);
        reelObjects[(int)ReelID.ReelMiddle].AdjustMaxSpeed(0.0f);
        reelObjects[(int)ReelID.ReelRight].AdjustMaxSpeed(0.0f);

        yield return new WaitForSeconds(2.0f);

        HasFakeReel = false;
    }
}

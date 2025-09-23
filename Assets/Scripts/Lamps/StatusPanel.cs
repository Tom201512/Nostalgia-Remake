using System.Collections;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    // ステータスランプ部分
    // const
    // 定期的に点滅させるときの時間数(INSERTとSTARTの点滅)
    private const float flashBetweenTime = 0.25f;

    private bool hasInsertTurnOn;
    private bool hasStartTurnOn;

    // アニメーション中か
    private bool hasAnimation;

    // var
    [SerializeField] LampComponent replayLamp;
    [SerializeField] LampComponent waitLamp;
    [SerializeField] LampComponent startLamp;
    [SerializeField] LampComponent insertLamp;

    private void Awake()
    {
        hasInsertTurnOn = false;
        hasStartTurnOn = false;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void TurnOnInsertLamp()
    {
        hasInsertTurnOn = true;
        if(!hasAnimation)
        {
            StartCoroutine(nameof(FlashInsertAndStart));
        }
    }

    public void TurnOnStartLamp()
    {
        hasStartTurnOn = true;
        if (!hasAnimation)
        {
            StartCoroutine(nameof(FlashInsertAndStart));
        }
    }

    public void TurnOffInsertAndStartlamp()
    {
        hasInsertTurnOn = false;
        hasStartTurnOn = false;
        hasAnimation = false;
        StopCoroutine(nameof(FlashInsertAndStart));

        startLamp.TurnOff();
        insertLamp.TurnOff();
    }

    public void TurnOnWaitLamp() => waitLamp.TurnOn();
    public void TurnOffWaitLamp() => waitLamp.TurnOff();

    public void TurnOnReplayLamp() => replayLamp.TurnOn();
    public void TurnOffReplayLamp() => replayLamp.TurnOff();

    // INSERTとSTARTを点滅させる

    private IEnumerator FlashInsertAndStart()
    {
        hasAnimation = true;
        while (hasInsertTurnOn || hasStartTurnOn)
        {
            if(hasInsertTurnOn)
            {
                insertLamp.TurnOn();
            }
            if (hasStartTurnOn)
            {
                startLamp.TurnOff();
            }
            yield return new WaitForSeconds(flashBetweenTime);

            if (hasInsertTurnOn)
            {
                insertLamp.TurnOff();
            }
            if (hasStartTurnOn)
            {
                startLamp.TurnOn();
            }
            yield return new WaitForSeconds(flashBetweenTime);
        }
    }
}

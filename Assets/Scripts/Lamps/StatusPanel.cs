using System.Collections;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    // �X�e�[�^�X�����v����
    // const
    // ����I�ɓ_�ł�����Ƃ��̎��Ԑ�(INSERT��START�̓_��)
    private const float flashBetweenTime = 0.25f;

    private bool hasInsertTurnOn;
    private bool hasStartTurnOn;

    // �A�j���[�V��������
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

    // INSERT��START��_�ł�����

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

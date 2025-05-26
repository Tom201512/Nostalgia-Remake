using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    // �X�e�[�^�X�����v����
    // const
    // ����I�ɓ_�ł�����Ƃ��̎��Ԑ�

    // var
    [SerializeField] LampComponent replayLamp;
    [SerializeField] LampComponent waitLamp;
    [SerializeField] LampComponent startLamp;
    [SerializeField] LampComponent insertLamp;

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void TurnOnInsertLamp() => insertLamp.TurnOn();
    public void TurnOffInsertlamp() =>startLamp.TurnOff();

    public void TurnOnStartLamp() => startLamp.TurnOn();
    public void TurnOffStartlamp() => insertLamp.TurnOff();

    public void TurnOnWaitLamp() => waitLamp.TurnOn();
    public void TurnOffWaitLamp() => waitLamp.TurnOff();

    public void TurnOnReplayLamp() => replayLamp.TurnOn();
    public void TurnOffReplayLamp() => replayLamp.TurnOff();
}

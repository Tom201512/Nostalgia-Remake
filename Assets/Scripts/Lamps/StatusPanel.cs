using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Lamps
{
    // ステータスランプ部分
    public class StatusPanel : MonoBehaviour
    {
        const float flashBetweenTime = 0.25f;   // 点滅時の待機秒数

        [SerializeField] LampComponent replayLamp;
        [SerializeField] LampComponent waitLamp;
        [SerializeField] LampComponent startLamp;
        [SerializeField] LampComponent insertLamp;

        private bool isInsertTurnOn;    // INSERTが点灯しているか
        private bool isStartTurnOn;     // STARTが点灯しているか
        private bool hasAnimation;      // アニメーション中か

        void Awake()
        {
            isInsertTurnOn = false;
            isStartTurnOn = false;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        // INSERT点灯
        public void TurnOnInsertLamp()
        {
            isInsertTurnOn = true;
            if (!hasAnimation)
            {
                StartCoroutine(nameof(FlashInsertAndStart));
            }
        }

        // START点灯
        public void TurnOnStartLamp()
        {
            isStartTurnOn = true;
            if (!hasAnimation)
            {
                StartCoroutine(nameof(FlashInsertAndStart));
            }
        }

        // INSERT, START消灯
        public void TurnOffInsertAndStart()
        {
            isInsertTurnOn = false;
            isStartTurnOn = false;
            hasAnimation = false;
            StopCoroutine(nameof(FlashInsertAndStart));

            startLamp.TurnOff();
            insertLamp.TurnOff();
        }

        // WAIT点灯
        public void TurnOnWaitLamp() => waitLamp.TurnOn();
        // WAIT消灯
        public void TurnOffWaitLamp() => waitLamp.TurnOff();

        // REPLAY点灯
        public void TurnOnReplayLamp() => replayLamp.TurnOn();
        // REPLAY消灯
        public void TurnOffReplayLamp() => replayLamp.TurnOff();

        // INSERTとSTARTを点滅させる時の処理
        private IEnumerator FlashInsertAndStart()
        {
            hasAnimation = true;
            while (isInsertTurnOn || isStartTurnOn)
            {
                if (isInsertTurnOn)
                {
                    insertLamp.TurnOn();
                }
                if (isStartTurnOn)
                {
                    startLamp.TurnOff();
                }
                yield return new WaitForSeconds(flashBetweenTime);

                if (isInsertTurnOn)
                {
                    insertLamp.TurnOff();
                }
                if (isStartTurnOn)
                {
                    startLamp.TurnOn();
                }
                yield return new WaitForSeconds(flashBetweenTime);
            }
        }
    }
}

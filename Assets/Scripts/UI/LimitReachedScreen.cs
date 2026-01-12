using ReelSpinGame_Option.MenuContent;
using System;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_System
{
    // プレイ回転数到達時の画面
    public class LimitReachedScreen : MonoBehaviour
    {
        private CanvasGroup canvasGroup;            // フェードイン、アウト用

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // 画面表示&初期化
        public void OpenScreen()
        {
            StartCoroutine(nameof(FadeInBehavior));
        }

        // フェードイン
        IEnumerator FadeInBehavior()
        {
            canvasGroup.alpha = 0;
            float fadeSpeed = Time.deltaTime / OptionScreenFade.FadeTime;

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha = Math.Clamp(canvasGroup.alpha + fadeSpeed, 0f, 1f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}


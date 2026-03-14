using System.Collections;
using UnityEngine;

// ウェイト管理
namespace ReelSpinGame_System
{
    public class WaitManager : MonoBehaviour
    {
        public const float WaitTime = 4.1f;     // ウェイトに必要な秒数

        public bool HasWait { get; private set; }       // ウェイトが有効か
        public bool HasWaitCut { get; private set; }    // ウェイトを無効にしているか

        private void Awake()
        {
            HasWait = false;
            HasWaitCut = false;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        // ウェイトカットの設定
        public void SetWaitCutSetting(bool hasWaitCut) => HasWaitCut = hasWaitCut;

        // ウェイトをかける
        public void SetWaitTimer()
        {
            // ウェイトカット、または実行中のウェイトがなければ実行
            if (!HasWaitCut && !HasWait)
            {
                HasWait = true;
                StartCoroutine(nameof(WaitBehaviour));
            }
        }

        // ウェイトタイマーを解除
        public void DisableWaitTimer()
        {
            StopAllCoroutines();
            HasWait = false;
        }

        private IEnumerator WaitBehaviour()
        {
            yield return new WaitForSeconds(WaitTime);
            HasWait = false;
        }
    }
}
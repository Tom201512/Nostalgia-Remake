using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // スベリコマ数表示
    public class DelayDisplayer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI delayText;     // テキスト

        public int CurrentDelay { get; set; }           // 現在のスベリコマ

        void Awake()
        {
            CurrentDelay = -1;
        }

        public void ResetDelay() => CurrentDelay = -1;
        public void SetDelay(int delay)
        {
            Debug.Log("Delay:" + delay);
            CurrentDelay = delay;
            UpdateScreen();
        }

        void UpdateScreen()
        {
            if (CurrentDelay == -1)
            {
                delayText.text = "-";
            }
            else
            {
                delayText.text = CurrentDelay.ToString();
            }
        }
    }
}

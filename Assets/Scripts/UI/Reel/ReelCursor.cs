using ReelSpinGame_Reels.Spin;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    // リールカーソル
    public class ReelCursor : MonoBehaviour
    {
        [SerializeField] int showAmounts;       // 表示個数

        Vector2 symbolSize;                     // 図柄の大きさ
        RectTransform[] cursors;                // カーソル
        RectTransform rectTransform;            // 本体のRectTransform

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            cursors = GetComponentsInChildren<RectTransform>();
            symbolSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y / ReelSpinModel.MaxReelArray);

            // カーソルの大きさを変更
            Vector2 cursorSize = new Vector2(symbolSize.x, symbolSize.y * showAmounts);
            Debug.Log("CursorSize" +  cursorSize);
            foreach(RectTransform cursor in cursors)
            {
                cursor.sizeDelta = cursorSize;
            }

            // 全体の大きさを変更
            float y = symbolSize.y * (ReelSpinModel.MaxReelArray + showAmounts);
            Vector2 rectSize = new Vector2(rectTransform.sizeDelta.x, y);
            Debug.Log("RectSize" + rectSize);
            rectTransform.sizeDelta = rectSize;
        }

        // 位置を設定
        public void SetPosition(int reelIndex, float tweenOffset)
        {
            float x = rectTransform.anchoredPosition.x;
            float y;

            if(reelIndex < 0)
            {
                y = 0;
            }
            else
            {
                y = symbolSize.y * (reelIndex + showAmounts) + symbolSize.y * tweenOffset;
            }

            Vector2 cursorPos = new Vector2(x, y);
            rectTransform.anchoredPosition = cursorPos;
        }
    }
}

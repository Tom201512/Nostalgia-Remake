using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Bonus.BonusSystemData;

namespace ReelSpinGame_UI.Bonus
{
    // 当選ボーナス表示
    public class BonusTypeDisplay : MonoBehaviour
    {
        // const

        // var
        // 当選色ごとのBIG表示画像
        [SerializeField] List<Sprite> bigDisplayImages;
        // REG表示画像
        [SerializeField] Sprite regDisplayImages;
        // 画像部分
        Image image;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        // 図柄の表示
        public void ShowBonusDisplay(BigColor bigColor)
        {
            // BIG当選時の色がNone以外であればBIG図柄を表示
            if ((int)bigColor > (int)BigColor.None)
            {
                if ((int)bigColor - 1 < bigDisplayImages.Count)
                {
                    image.sprite = bigDisplayImages[(int)bigColor - 1];
                }
                else
                {
                    Debug.LogError("Cannot find a sprite");
                }
            }
            // それ以外はREGを表示
            else
            {
                image.sprite = regDisplayImages;
            }
        }
    }
}

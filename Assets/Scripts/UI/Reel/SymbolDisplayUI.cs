using ReelSpinGame_Reels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_UI.Reel
{
    public class SymbolDisplayUI : MonoBehaviour
    {
        // 表示部分
        private Image symbolImage;

        // リール位置識別ID
        [SerializeField] private ReelPosID posID;

        private void Awake()
        {
            symbolImage = GetComponent<Image>();
        }

        // 図柄変更
        public void ChangeSymbol(Sprite symbolSprite) => symbolImage.sprite = symbolSprite;

        // 位置IDを返す
        public ReelPosID GetPosID() => posID;
    }
}

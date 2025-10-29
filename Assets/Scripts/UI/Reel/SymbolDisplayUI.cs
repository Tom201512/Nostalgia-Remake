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
        // �\������
        private Image symbolImage;

        // ���[���ʒu����ID
        [SerializeField] private ReelPosID posID;

        private void Awake()
        {
            symbolImage = GetComponent<Image>();
        }

        // �}���ύX
        public void ChangeSymbol(Sprite symbolSprite) => symbolImage.sprite = symbolSprite;

        // �ʒuID��Ԃ�
        public ReelPosID GetPosID() => posID;
    }
}

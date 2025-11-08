using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Graph
{
    public class GraphComponent : MonoBehaviour
    {
        // グラフ用のコンポーネント

        // const
        // グラフ始点X座標
        const int GraphStartPosX = 0;
        // グラフ始点Y座標
        const int GraphStartPosY = 0;

        // var
        // グラフを描く対象のオブジェクト
        [SerializeField] private Image graphLines;

        // グラフのTexture2D
        private Texture2D graphTexture;

        private void Awake()
        {

        }

        private void Start()
        {
            
        }
    }
}
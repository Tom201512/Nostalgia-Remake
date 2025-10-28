using ReelSpinGame_Reels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // リール結果表示

        // var
        // リール図柄
        [SerializeField] Sprite[] symbolImages;
        // 各リール
        [SerializeField] List<ReelObject> reels;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
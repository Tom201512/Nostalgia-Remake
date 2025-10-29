using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Reel
{
    public class ReelDisplayUI : MonoBehaviour
    {
        // リール結果表示

        // var
        // リールディスプレイ
        [SerializeField] List<ReelDisplayer> reelDisplayers;

        // Start is called before the first frame update
        void Start()
        {
            foreach(ReelDisplayer display in  reelDisplayers)
            {
                Debug.Log("Display at" + 19);
                display.DisplayReel(19);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReelSpinGame_UI.Graph
{
    public class GraphComponent : MonoBehaviour
    {
        // グラフ用のコンポーネント

        // const
        // グラフX座標最大値
        const float GraphMaxPosX = 1250.0f;
        // グラフY座標最大値
        const float GraphMaxPosY = 600.0f;

        // 描画時のX座標、Y座標のオフセット設定
        const float XPosDivideValue = GraphMaxPosX / 1000;
        const float YPosDivideValue = GraphMaxPosY / 1000;

        // var
        // グラフを描く対象のオブジェクト
        [SerializeField] private GraphDrawer graphDrawer;
        // 差枚数カウンタテキスト(+)
        [SerializeField] private TextMeshProUGUI maxDiffText;
        // 差枚数カウンタテキスト(-)
        [SerializeField] private TextMeshProUGUI minDiffText;

        private void Awake()
        {

        }

        private void Start()
        {
            graphDrawer.gameObject.SetActive(false);
        }
        
        // func
        // グラフの描画を開始する
        public void StartDrawGraph(int[] medalSlumpGraph)
        {
            graphDrawer.gameObject.SetActive(true);
            graphDrawer.ResetDraw();

            // ゲーム数、最高と最低差枚数よりグラフを作成する。
            // 1000Gごとに計測する区切りを変更していく。
            // 差枚数の表記は1000枚超えるごとに変化する。

            // 最高、最低差枚を求める
            int highest = 0;
            int lowest = 0;

            foreach(int i in medalSlumpGraph)
            {
                if(i > highest)
                {
                    highest = i;
                }
                if(i < lowest)
                {
                    lowest = i;
                }
            }

            // 最高差枚が1000, または最低差枚数が-1000を超えていた場合は1000桁区切りで近い数値に表記を合わせる
            if(highest > 1000 || lowest > -1000)
            {
                maxDiffText.text = "+" + (highest / 1000);
                minDiffText.text = "-" + (lowest / 1000);
            }
            // それ以外は最高差枚を1000枚、最低差枚を-1000枚に設定
            else
            {
                maxDiffText.text = "+" + 1000;
                minDiffText.text = "-" + 1000;
            }

            // グラフ描画を開始する

            // 区切る時の数値を求める
            int divideValue = medalSlumpGraph.Length / 1000 + 1;

            foreach(int i in medalSlumpGraph)
            {

            }
        }
    }
}
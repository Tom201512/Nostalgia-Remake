using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ReelSpinGame_UI.Graph
{
    public class GraphDrawer : MonoBehaviour
    {
        // グラフを描画するスクリプト

        // const

        // var
        // 線描画コンポーネント
        private LineRenderer lineRenderer;

        // 座標位置
        private List<Vector3> positions;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            positions.Add(new Vector3(0, 0));
        }

        private void Start()
        {
            ResetDraw();
        }

        // func

        // 描画のリセット
        public void ResetDraw()
        {
            positions.Clear();
            positions.Add(new Vector3(0, 0));
            lineRenderer.SetPositions(positions.ToArray());
        }

        // 指定座標へ描画する
        public void DrawAtPosition(float x, float y)
        {
            positions.Add(new Vector3(x, y));
            lineRenderer.SetPositions(positions.ToArray());
        }
    }
}

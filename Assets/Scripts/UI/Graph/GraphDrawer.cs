using System.Collections.Generic;
using UnityEngine;

namespace ReelSpinGame_UI.Graph
{
    // グラフを描画するスクリプト
    public class GraphDrawer : MonoBehaviour
    {
        [SerializeField] float simplifyTolerance = 5f;  // グラフの見やすさ
        private LineRenderer lineRenderer;              // 線描画コンポーネント
        private List<Vector3> positions;                // 座標位置

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            positions = new List<Vector3>();
        }

        // 描画のリセット
        public void ResetDraw()
        {
            positions.Clear();
            lineRenderer.positionCount = 0;
            lineRenderer.SetPositions(positions.ToArray());
        }

        // 座標の追加
        public void AddPosition(float x, float y)
        {
            lineRenderer.positionCount += 1;
            positions.Add(new Vector3(x, y));
        }

        // 描画を行う
        public void StartDraw()
        {
            Vector3[] results = positions.ToArray();
            lineRenderer.SetPositions(results);
            // グラフを見やすくする
            lineRenderer.Simplify(simplifyTolerance);
        }
    }
}

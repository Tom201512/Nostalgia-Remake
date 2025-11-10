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
            positions = new List<Vector3>();
        }

        // func

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
            foreach(Vector3 position in positions)
            {
                Debug.Log("Position X:" + position.x + " Y:" + position.y);
            }

            Vector3[] results = positions.ToArray();
            lineRenderer.SetPositions(results);
        }
    }
}

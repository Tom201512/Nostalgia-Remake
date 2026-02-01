using ReelSpinGame_System;
using ReelSpinGame_UI.Graph;
using UnityEngine;

namespace ReelSpinGame_Option.MenuContent
{
    // 差枚数のスランプグラフを表示するUI
    public class SlumpGraphDataUI : MonoBehaviour
    {
        [SerializeField] private GraphComponent graphComponent;
        public void UpdateData(PlayerDatabase player)
        {
            graphComponent.StartDrawGraph(player);
        }
    }
}

using ReelSpinGame_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReelSpinGame_UI.Graph;

namespace ReelSpinGame_Option.MenuContent
{
    public class SlumpGraphDataUI : MonoBehaviour
    {
        // 差枚数のスランプグラフを表示するUI

        // var
        [SerializeField] private GraphComponent graphComponent;

        // func
        public void UpdateData(PlayerDatabase player) => graphComponent.StartDrawGraph(player);
    }
}

using ReelSpinGame_System;
using System;
using TMPro;
using UnityEngine;

namespace ReelSpinGame_UI.Graph
{
    // グラフ用のコンポーネント
    public class GraphComponent : MonoBehaviour
    {

        const float GraphMaxPosX = 2400.0f;                     // グラフX座標最大値
        const float GraphMaxPosY = 1200.0f;                     // グラフY座標最大値
        const float GraphMiddlePosY = GraphMaxPosY / 2;         // Y座標中央値
        const float GraphXPosOffset = GraphMaxPosX / 1000;      // X座標分割値
        const float GraphYPosOffset = GraphMiddlePosY / 1000;   // Y座標分割値


        [SerializeField] private Camera graphCamera;                // グラフ用のカメラ
        [SerializeField] private GraphDrawer graphDrawer;           // グラフを描く対象のオブジェクト
        [SerializeField] private TextMeshProUGUI maxDiffText;       // 差枚数カウンタテキスト(+)
        [SerializeField] private TextMeshProUGUI minDiffText;       // 差枚数カウンタテキスト(-)
        [SerializeField] private TextMeshProUGUI gameCountText;     // ゲーム数表示テキスト

        // グラフの描画を開始する
        public void StartDrawGraph(PlayerDatabase playerData)
        {
            graphCamera.gameObject.SetActive(true);
            graphDrawer.gameObject.SetActive(true);
            graphDrawer.ResetDraw();

            // 最高、最低差枚の表記設定
            int highest = 0;
            int lowest = 0;

            foreach (int i in playerData.PlayerMedalData.MedalSlumpGraph)
            {
                if (i > highest)
                {
                    highest = i;
                }
                if (i < lowest)
                {
                    lowest = i;
                }
            }

            // 最高、最低差枚数の表示


            int differenceOffset = 1;
            // 最低でも-1000枚~1000枚から表示する。超えている場合は高い方の数値を基準に表示
            if (highest > 1000 || lowest < -1000)
            {
                if (highest >= lowest)
                {
                    differenceOffset = highest / 1000 + 1;
                }
                else
                {
                    differenceOffset = Math.Abs(lowest) / 1000 + 1;
                }
            }

            maxDiffText.text = "+" + differenceOffset * 1000;
            minDiffText.text = "-" + differenceOffset * 1000;

            // グラフ描画
            // 区切る時の数値を求める
            int divideValueX = playerData.PlayerMedalData.MedalSlumpGraph.Count / 1000 + 1;
            int count = 0;
            int index = 0;

            // 差枚数ごとに座標を求めて描画する
            while (true)
            {
                float posX = 0.0f;
                float posY = 0.0f;

                posX = GraphXPosOffset * (count + 1);
                posY = GraphMiddlePosY + GraphYPosOffset * ((float)playerData.PlayerMedalData.MedalSlumpGraph[index] / differenceOffset);

                graphDrawer.AddPosition(posX, posY);

                // スランプグラフのデータ数分読み込んだ場合は終了させる(桁あふれした場合)
                if (index == playerData.PlayerMedalData.MedalSlumpGraph.Count - 1)
                {
                    break;
                }

                count += 1;
                index += divideValueX;

                // スランプグラフのデータ数分読み込んだ場合は終了させる(桁あふれしなかった場合)
                if (index == playerData.PlayerMedalData.MedalSlumpGraph.Count)
                {
                    break;
                }
                // 桁あふれする場合は次回最後のデータを表示したら終了
                else if (index > playerData.PlayerMedalData.MedalSlumpGraph.Count)
                {
                    index = playerData.PlayerMedalData.MedalSlumpGraph.Count - 1;
                }
            }

            graphDrawer.StartDraw();

            // ゲーム数を表示する(1000G区切り。9万Gを超える場合は 99999Gと表記)
            int gameCountValue = (playerData.TotalGames / 1000 + 1) * 1000;

            if (gameCountValue > 90000)
            {
                gameCountValue = 99999;
            }
            gameCountText.text = gameCountValue + "G";
        }
    }
}
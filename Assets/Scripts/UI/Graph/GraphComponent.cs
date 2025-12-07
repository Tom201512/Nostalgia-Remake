using ReelSpinGame_System;
using System;
using System.Collections.Generic;
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
        const float GraphMaxPosX = 2400.0f;
        // グラフY座標最大値
        const float GraphMaxPosY = 1200.0f;
        // Y座標中央値
        const float GraphMiddlePosY = GraphMaxPosY / 2;
        // X座標分割値
        const float GraphXPosOffset = GraphMaxPosX / 1000;
        // Y座標分割値
        const float GraphYPosOffset = GraphMiddlePosY / 1000;

        // var
        // グラフ用のカメラ
        [SerializeField] private Camera graphCamera;
        // グラフを描く対象のオブジェクト
        [SerializeField] private GraphDrawer graphDrawer;
        // 差枚数カウンタテキスト(+)
        [SerializeField] private TextMeshProUGUI maxDiffText;
        // 差枚数カウンタテキスト(-)
        [SerializeField] private TextMeshProUGUI minDiffText;
        // ゲーム数表示テキスト
        [SerializeField] private TextMeshProUGUI gameCountText;

        private void Awake()
        {

        }

        private void Start()
        {

        }
        
        // func
        // グラフの描画を開始する
        public void StartDrawGraph(PlayerDatabase playerData)
        {
            graphCamera.gameObject.SetActive(true);
            graphDrawer.gameObject.SetActive(true);
            graphDrawer.ResetDraw();

            // 最高、最低差枚の表記設定
            int highest = 0;
            int lowest = 0;

            foreach(int i in playerData.PlayerMedalData.MedalSlumpGraph)
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

            // 最高、最低差枚数の表示

            // 1000枚ごとの区切りをつける
            int differenceOffset = 1;
            // 最低でも-1000枚~1000枚から表示する。超えている場合は高い方の数値を基準に表示
            if(highest > 1000 || lowest < -1000)
            {
                if(highest >= lowest)
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
            // X
            int divideValueX = playerData.PlayerMedalData.MedalSlumpGraph.Count / 1000 + 1;
            Debug.Log("DivideX:" + divideValueX);

            // 現在のデータ数
            int count = 0;
            // 現在読み込む配列位置
            int index = 0;

            // 差枚数ごとに座標を求めて描画する
            Debug.Log("Load count:" + playerData.PlayerMedalData.MedalSlumpGraph.Count);
            while(true)
            {
                float posX = 0.0f;
                float posY = 0.0f;

                posX = GraphXPosOffset * (count + 1);
                posY = GraphMiddlePosY + GraphYPosOffset * ((float)playerData.PlayerMedalData.MedalSlumpGraph[index] / differenceOffset);

                Debug.Log("DrawGraph at:" + posX + "," + posY);
                graphDrawer.AddPosition(posX, posY);

                // スランプグラフのデータ数分読み込んだ場合は終了させる(桁あふれした場合)
                if(index == playerData.PlayerMedalData.MedalSlumpGraph.Count - 1)
                {
                    Debug.Log("Finished");
                    break;
                }

                count += 1;
                index += divideValueX;

                Debug.Log("Count:" + count);
                Debug.Log("Index:" + index);

                // スランプグラフのデータ数分読み込んだ場合は終了させる(桁あふれしなかった場合)
                if (index == playerData.PlayerMedalData.MedalSlumpGraph.Count)
                {
                    Debug.Log("Finished");
                    break;
                }
                // 桁あふれする場合は次回最後のデータを表示したら終了
                else if (index > playerData.PlayerMedalData.MedalSlumpGraph.Count)
                {
                    Debug.Log("Overflow");
                    index = playerData.PlayerMedalData.MedalSlumpGraph.Count - 1;
                }
            }

            graphDrawer.StartDraw();

            // ゲーム数を表示する(1000G区切り。9万Gを超える場合は 99999Gと表記)
            int gameCountValue = (playerData.TotalGames / 1000 + 1) * 1000;
            
            if(gameCountValue > 90000)
            {
                gameCountValue = 99999;
            }
            gameCountText.text = gameCountValue + "G" ;
        }
    } 
}
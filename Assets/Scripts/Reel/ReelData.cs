using ReelSpinGame_Reels.Array;
using System.Collections;
using UnityEngine;

namespace ReelSpinGame_Reels
{
    public class ReelData
    {
        // リールのデータ *MonoBehaviourを持つ

        // 目指す目標

        // 1:リールの回転
        // 2:図柄の実装
        // 3:リールの停止
        // 4:スベリ実装
        // 5:テーブル機能搭載

        // 各リールデータが持つもの

        // リールの現在位置(下段), スベリ数, 停止するか
        // そのリールの配列
        // リール制御テーブル
        // 制御条件テーブル

        // const 

        // var

        // リールの状態

        private bool isStopping;
        private ReelArray.ReelSymbols[] array;


        public ReelData(int test, ReelArray.ReelSymbols[] arrayData) 
        {
            isStopping = false;
            array = arrayData;
            Debug.Log("ReelGenerated:" +  test);

            for(int i = 0; i < arrayData.Length; i++)
            {
                Debug.Log("No." + i + " Symbol:" + arrayData[i]);
            }
        }


        // func

        
    }
}

using UnityEngine;

namespace ReelSpinGame_Scriptable
{
    // リール配列データ
    public class ReelArrayData : ScriptableObject
    {
        // リール配列
        [SerializeField] private int[] array;

        public int[] Array { get => array; }

        // 配列設定
        public void SetArray(int[] array) => this.array = array;
    }
}
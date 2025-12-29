using UnityEngine;

namespace ReelSpinGame_Datas
{
    // リール配列データ
    public class ReelArrayData : ScriptableObject
    {
        // リール配列
        [SerializeField] private byte[] array;

        public byte[] Array { get => array; }

        // 配列設定
        public void SetArray(byte[] array) => this.array = array;
    }
}

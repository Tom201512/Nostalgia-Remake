using UnityEngine;

namespace ReelSpinGame_Datas
{
    // リール配列データ
    public class ReelArrayData : ScriptableObject
    {
        // var
        // リール配列
        [SerializeField] private byte[] array;

        public byte[] Array { get { return array; } }

        // func
        public void SetArray(byte[] array) => this.array = array;
    }
}

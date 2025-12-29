using System.Collections.Generic;
using System.IO;

namespace ReelSpinGame_Save
{
    // セーブマネージャーのインターフェース
    public interface ISaveManage
    {
        public List<int> GenerateDataBuffer(); // データバッファを作成
        public bool LoadDataBuffer(Stream baseStream, BinaryReader br); //データバッファの読み込み
    }
}
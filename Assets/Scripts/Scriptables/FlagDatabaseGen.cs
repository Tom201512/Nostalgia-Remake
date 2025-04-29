using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class FlagDatabaseGen : EditorWindow
    {
        public static FlagDataSets MakeFlagTableSets(StreamReader flagTableFile)
        {
            return new FlagDataSets(flagTableFile);
        }
    }
#endif
}


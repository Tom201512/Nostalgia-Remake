using ReelSpinGame_Datass;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class FlagDatabaseGen : EditorWindow
    {
        // const

        // var
        // �t�@�C����
        private string fileName;

        // �t�@�C���w��
        private TextAsset normalTableAfile;
        private TextAsset normalTableBfile;
        private TextAsset bigTablefile;
        private float jacNonePossibility;

        // func
        // ���[���z��̍쐬
        [MenuItem("ScriptableGen/FlagData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open FlagData Generator");
            FlagDatabaseGen window = GetWindow<FlagDatabaseGen>();
            window.titleContent = new GUIContent("FlagData Generator");
        }

        private void Awake()
        {
            fileName = "FlagDatabase.asset";
            normalTableAfile = null;
            normalTableBfile = null;
            bigTablefile = null;
            jacNonePossibility = 1.0f;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("�t���O�f�[�^�x�[�X�쐬");
            normalTableAfile = (TextAsset)EditorGUILayout.ObjectField("NormalTableA:", normalTableAfile, typeof(TextAsset), true);
            normalTableBfile = (TextAsset)EditorGUILayout.ObjectField("NormalTableB:", normalTableBfile, typeof(TextAsset), true);
            bigTablefile = (TextAsset)EditorGUILayout.ObjectField("BigTable:", bigTablefile, typeof(TextAsset), true);
            jacNonePossibility = EditorGUILayout.FloatField("Jac None Possibility(n > 0)", jacNonePossibility);

            if (GUILayout.Button("�t���O�f�[�^�x�[�X�쐬"))
            {
                Debug.Log("Pressed");

                if(jacNonePossibility > 0)
                {
                    MakeFlagData(normalTableAfile, normalTableBfile, bigTablefile, jacNonePossibility);
                }
                else
                {
                    Debug.LogError("JAC none possibility is must be higher that 0");
                }
            }
        }

        private void MakeFlagData(TextAsset normalTableAfile, TextAsset normalTableBfile, TextAsset bigTablefile, float jacNonePossibility)
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/FlagDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            FlagDatabase flagDatabase = CreateInstance<FlagDatabase>();

            // �t���O�e�[�u���쐬
            // �ʏ펞A�t���O�e�[�u���쐬
            flagDatabase.SetNormalATable(MakeFlagTableSets(normalTableAfile));
            // �ʏ펞B�t���O�e�[�u���쐬
            flagDatabase.SetNormalBTable(MakeFlagTableSets(normalTableBfile));
            // �����Q�[�����t���O�e�[�u���쐬
            flagDatabase.SetBIGTable(MakeFlagTableSets(bigTablefile));

            // JAC���n�Y��
            flagDatabase.SetJACNonePoss(jacNonePossibility);

            // �ۑ�����
            AssetDatabase.CreateAsset(flagDatabase, Path.Combine(path, fileName));
            Debug.Log("Flag Database is generated");
        }

        private FlagDataSets MakeFlagTableSets(TextAsset flagTableFile)
        {
            StringReader buffer = new StringReader(flagTableFile.text);

            return new FlagDataSets(buffer);
        }
    }
#endif
}


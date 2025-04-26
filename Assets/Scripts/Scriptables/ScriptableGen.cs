using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // �X�N���v�^�u�����쐬����
#if UNITY_EDITOR
    public class ScriptableGen : EditorWindow
    {
        // const
        // �f�t�H���g�Ŏg���t�@�C���p�X
        private const string DataPath = "Assets/DataFile";

        // ���[���̃t�@�C��
        private const string LeftPath = "ReelL";
        private const string MiddlePath = "ReelM";
        private const string RightPath = "ReelR";

        // var
        // �e���[�������ۂɑI�񂾃{�^���ԍ�
        private int reelSelection;

        // func
        // ���[���z��̍쐬
        [MenuItem("ScriptableGen/Scriptable Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open ScriptableGen Generator");
            ScriptableGen window = GetWindow<ScriptableGen>();
            window.titleContent = new GUIContent("Scriptable Generator");
        }

        private void Awake()
        {
            reelSelection = -1;
        }

        private void OnGUI()
        {
            GUILayout.Label("�X�N���v�^�u���I�u�W�F�N�g�쐬\n");
            // fileName = EditorGUILayout.TextField("File Name", fileName);
            //arrayFile = (TextAsset)EditorGUILayout.ObjectField("Array:", arrayFile, typeof(TextAsset), true);
            //conditionsFile = (TextAsset)EditorGUILayout.ObjectField("Conditions:", conditionsFile, typeof(TextAsset), true);
            //delaysData = (TextAsset)EditorGUILayout.ObjectField("Tables:", delaysData, typeof(TextAsset), true);
            GUILayout.Label("���[���f�[�^�쐬\n");

            if (GUILayout.Button("�S���[���f�[�^�쐬"))
            {
                Debug.Log("Pressed");
                //MakeReelData(fileName, arrayFile, conditionsFile, delaysData);
                MakeReelDataAll();
            }

            GUILayout.Label("\n�e���[���̍쐬\n");

            reelSelection = GUILayout.Toolbar(reelSelection, new[] { "��", "��", "�E" });

            if (reelSelection == (int)ReelManager.ReelID.ReelLeft)
            {
                reelSelection = -1;
                MakeReelData(LeftPath);
            }

            if (reelSelection == (int)ReelManager.ReelID.ReelMiddle)
            {
                reelSelection = -1;
                MakeReelData(MiddlePath);
            }

            if (reelSelection == (int)ReelManager.ReelID.ReelRight)
            {
                reelSelection = -1;
                MakeReelData(RightPath);
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { LeftPath, MiddlePath, RightPath };

            // ���炩���ߐݒ肵���f�B���N�g������S���[���̃f�[�^��ǂݍ���
            for (int i = 0; i < ReelManager.ReelAmounts; i++)
            {
                MakeReelData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        private void MakeReelData(string filePath)
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/ReelDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }
            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();
            // �z��쐬
            reelDatabase.SetArray(MakeReelArray(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Array.csv"))));
            // �����e�[�u���쐬
            reelDatabase.SetConditions(MakeReelConditions(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Condition.csv"))));
            // �f�B���C�e�[�u���쐬
            reelDatabase.SetTables(MakeTableDatas(new StreamReader(Path.Combine(DataPath, filePath, filePath + "Table.csv"))));

            var fileName = filePath + ".asset";
            // �ۑ�����
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }

        private byte[] MakeReelArray(StreamReader arrayFile)
        {
            // �z��ǂݍ���
            string[] values = arrayFile.ReadLine().Split(',');
            // �z��ɕϊ�
            byte[] result = Array.ConvertAll(values, byte.Parse);

            foreach (byte value in result)
            {
                Debug.Log(value + "Symbol:" + ReelData.ReturnSymbol(value));
            }

            for (int i = 0; i < ReelData.MaxReelArray; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReelData.ReturnSymbol(result[i]));
            }

            return result;
        }

        private List<ReelConditionsData> MakeReelConditions(StreamReader conditionsFile)
        {
            List<ReelConditionsData> finalResult = new List<ReelConditionsData>();

            // �S�Ă̍s��ǂݍ���
            while (conditionsFile.Peek() != -1)
            {
                finalResult.Add(new ReelConditionsData(conditionsFile));
            }

            foreach (ReelConditionsData condition in finalResult)
            {
                // �f�o�b�O�p
                string ConditionDebug = "";

                for (int i = 0; i < 5; i++)
                {
                    ConditionDebug += condition.GetConditionData(i).ToString() + ",";
                }

                Debug.Log("Condition:" + condition.MainConditions + "Details:" + ConditionDebug + "FirstReel:" + condition.FirstReelPosition + "ReelTableNum" + condition.ReelTableNumber);
            }

            return finalResult;
        }

        private List<ReelTableData> MakeTableDatas(StreamReader tablesFile)
        {
            List<ReelTableData> finalResult = new List<ReelTableData>();

            // �S�Ă̍s��ǂݍ���
            while (tablesFile.Peek() != -1)
            {
                finalResult.Add(new ReelTableData(tablesFile));
            }

            return finalResult;
        }
    }
#endif
}

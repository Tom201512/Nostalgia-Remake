using System.IO;
using UnityEditor;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

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

        // �t���O�̃t�@�C��
        private const string FlagPath = "LotsTable";

        // �����o���֘A�̃t�@�C��
        private const string PayoutPath = "Payouts";

        // JAC�͂���f�t�H���g�l
        private const float JacNoneDefault = 256f;

        // var
        // �e���[�������ۂɑI�񂾃{�^���ԍ�
        private int reelSelection;

        // JAC�Q�[�����̂͂���m��
        private float jacNoneProb;

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
            jacNoneProb = JacNoneDefault;
        }

        private void OnGUI()
        {
            GUILayout.Label("�X�N���v�^�u���I�u�W�F�N�g�쐬\n");
            GUILayout.Label("���[���f�[�^�쐬\n");

            if (GUILayout.Button("�S���[���f�[�^�쐬"))
            {
                Debug.Log("Pressed");
                //MakeReelData(fileName, arrayFile, conditionsFile, delaysData);
                MakeReelDataAll();
            }

            GUILayout.Label("\n�e���[���̍쐬\n");

            reelSelection = GUILayout.Toolbar(reelSelection, new[] { "��", "��", "�E" });

            if (reelSelection == (int)ReelID.ReelLeft)
            {
                reelSelection = -1;
                MakeReelData(LeftPath);
            }

            if (reelSelection == (int)ReelID.ReelMiddle)
            {
                reelSelection = -1;
                MakeReelData(MiddlePath);
            }

            if (reelSelection == (int)ReelID.ReelRight)
            {
                reelSelection = -1;
                MakeReelData(RightPath);
            }

            GUILayout.Label("\n�t���O�f�[�^�̍쐬\n");
            jacNoneProb = EditorGUILayout.FloatField("JAC���͂���m��(float)", jacNoneProb);

            if (GUILayout.Button("�t���O�f�[�^�쐬"))
            {
                Debug.Log("Pressed");
                MakeFlagData();
            }

            GUILayout.Label("\n�����o���f�[�^�x�[�X�쐬\n");

            if (GUILayout.Button("�����o���f�[�^�x�[�X�쐬"))
            {
                Debug.Log("Pressed");
                MakePayoutData();
            }
        }

        private void MakeReelDataAll()
        {
            string[] pathOrder = { LeftPath, MiddlePath, RightPath };

            // ���炩���ߐݒ肵���f�B���N�g������S���[���̃f�[�^��ǂݍ���
            for (int i = 0; i < ReelAmounts; i++)
            {
                MakeReelData(pathOrder[i]);
            }

            Debug.Log("All ReelData is generated");
        }

        private void MakeReelData(string filePath)
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/ReelDatas";
            // �t�@�C�����w��
            string fileName = filePath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();

            // �t�@�C���ǂݍ���

            // �z��쐬
            using StreamReader array = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Array.csv"));
            reelDatabase.SetArray(ReelDatabaseGen.MakeReelArray(array));

            // �����e�[�u���쐬
            using StreamReader condition = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Condition.csv"));
            reelDatabase.SetConditions(ReelDatabaseGen.MakeReelConditions(condition));

            // �f�B���C�e�[�u���쐬
            using StreamReader table = new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Table.csv"));
            reelDatabase.SetTables(ReelDatabaseGen.MakeTableDatas(table));

            // �ۑ�����
            GenerateFile(path, fileName, reelDatabase);
        }


        private void MakeFlagData()
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/FlagDatas";
            // �t�@�C�����w��
            string fileName = "FlagDatabase";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            FlagDatabase flagDatabase = CreateInstance<FlagDatabase>();

            // �t���O�e�[�u���쐬
            // �ʏ펞A�t���O�e�[�u���쐬
            using StreamReader normalA = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalA.csv"));
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(normalA));

            // �ʏ펞B�t���O�e�[�u���쐬
            using StreamReader normalB = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalB.csv"));
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(normalB));

            // �����Q�[�����t���O�e�[�u���쐬
            using StreamReader bigTable = new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableBig.csv"));
            flagDatabase.SetBIGTable(FlagDatabaseGen.MakeFlagTableSets(bigTable));

            // JAC���n�Y��
            flagDatabase.SetJACNonePoss(jacNoneProb);

            // �ۑ�����
            GenerateFile(path, fileName, flagDatabase);
        }


        private void MakePayoutData()
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/PayoutDatas";
            // �t�@�C�����w��
            string fileName = "PayoutDatabase";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            PayoutDatabase payoutDatabase = CreateInstance<PayoutDatabase>();

            // �����o�����C���쐬
            using StreamReader payoutLine = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - PayoutLineData.csv"));
            payoutDatabase.SetPayoutLines(PayoutDatabaseGen.MakePayoutLineDatas(payoutLine));

            // �����o���g�ݍ��킹�\�쐬
            // �ʏ펞
            using StreamReader normalPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - NormalPayout.csv"));
            payoutDatabase.SetNormalPayout(PayoutDatabaseGen.MakeResultDatas(normalPayout));

            // �����Q�[����
            using StreamReader bigPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - BigPayout.csv"));
            payoutDatabase.SetBigPayout(PayoutDatabaseGen.MakeResultDatas(bigPayout));

            // JAC�Q�[����
            using StreamReader jacPayout = new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - JacPayout.csv"));
            payoutDatabase.SetJacPayout(PayoutDatabaseGen.MakeResultDatas(jacPayout));

            // �ۑ�����
            GenerateFile(path, fileName, payoutDatabase);
        }

        private void GenerateFile(string path, string fileName, ScriptableObject scriptableObject)
        {
            // �ۑ�����
            // �t�@�C��������ꍇ
            if (File.Exists(Path.Combine(path, fileName) + ".asset"))
            {
                // �f�B���N�g���̍쐬
                string temporaryPath = "Assets/Temp";

                Directory.CreateDirectory(temporaryPath);
                Debug.Log("Temporary Directory is created");

                // ���������p�̉��t�@�C�����쐬
                AssetDatabase.CreateAsset(scriptableObject, Path.Combine(temporaryPath, fileName) + ".asset");
                // ���������t�@�C���ɒu��������
                FileUtil.ReplaceFile(Path.Combine(temporaryPath, fileName) + ".asset", Path.Combine(path, fileName) + ".asset");

                // ���t�@�C�����폜
                AssetDatabase.DeleteAsset(temporaryPath);
                Debug.Log(fileName + " is replaced");
            }
            else
            {
                AssetDatabase.CreateAsset(scriptableObject, Path.Combine(path, fileName) + ".asset");
                Debug.Log(fileName + " is generated");
            }

            AssetDatabase.Refresh();
        }
    }
#endif
}

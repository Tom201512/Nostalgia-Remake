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
            reelDatabase.SetArray(ReelDatabaseGen.MakeReelArray
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Array.csv"))));
            // �����e�[�u���쐬
            reelDatabase.SetConditions(ReelDatabaseGen.MakeReelConditions
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Condition.csv"))));
            // �f�B���C�e�[�u���쐬
            reelDatabase.SetTables(ReelDatabaseGen.MakeTableDatas
                (new StreamReader(Path.Combine(DataPath, filePath, "Nostalgia_Reel - " + filePath + "Table.csv"))));

            var fileName = filePath + ".asset";
            // �ۑ�����

            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }


        private void MakeFlagData()
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
            flagDatabase.SetNormalATable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalA.csv"))));

            // �ʏ펞B�t���O�e�[�u���쐬
            flagDatabase.SetNormalBTable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableNormalB.csv"))));

            // �����Q�[�����t���O�e�[�u���쐬
            flagDatabase.SetBIGTable(FlagDatabaseGen.MakeFlagTableSets(
                new StreamReader(Path.Combine(DataPath, FlagPath, "Nostalgia_Flag - FlagTableBig.csv"))));
            // JAC���n�Y��
            flagDatabase.SetJACNonePoss(jacNoneProb);

            // �ۑ�����
            AssetDatabase.CreateAsset(flagDatabase, Path.Combine(path, "FlagDatabase.asset"));
            Debug.Log("Flag Database is generated");
        }


        private void MakePayoutData()
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/PayoutDatas";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            PayoutDatabase payoutDatabase = CreateInstance<PayoutDatabase>();

            // �����o�����C���쐬
            payoutDatabase.SetPayoutLines(PayoutDatabaseGen.MakePayoutLineDatas(
                new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - PayoutLineData.csv"))));
            // �����o���g�ݍ��킹�\�쐬
            // �ʏ펞
            payoutDatabase.SetNormalPayout(PayoutDatabaseGen.MakeResultDatas(
                new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - NormalPayout.csv"))));
            // �����Q�[����
            payoutDatabase.SetBigPayout(PayoutDatabaseGen.MakeResultDatas(
                new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - BigPayout.csv"))));
            // JAC�Q�[����
            payoutDatabase.SetJacPayout(PayoutDatabaseGen.MakeResultDatas(
                new StreamReader(Path.Combine(DataPath, PayoutPath, "Nostalgia_Payout - JacPayout.csv"))));
            // �ۑ�����
            AssetDatabase.CreateAsset(payoutDatabase, Path.Combine(path, "PayoutDatabase.asset"));
            Debug.Log("Payout Database is generated");
        }
    }
#endif
}

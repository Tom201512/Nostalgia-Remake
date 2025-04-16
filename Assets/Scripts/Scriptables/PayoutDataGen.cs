using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
#if UNITY_EDITOR
    public class PayoutDatabaseGen : EditorWindow
    {
        // const

        // var
        // �t�@�C����
        private string fileName;

        // �t�@�C���w��
        private TextAsset payoutLineFile;
        private TextAsset normalPayoutFile;
        private TextAsset bigPayoutFile;
        private TextAsset jacPayoutFile;

        // func
        // ���[���z��̍쐬
        [MenuItem("ScriptableGen/PayoutData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open PayoutData Generator");
            PayoutDatabaseGen window = GetWindow<PayoutDatabaseGen>();
            window.titleContent = new GUIContent("PayoutData Generator");
        }

        private void Awake()
        {
            fileName = "PayoutDatabase.asset";
            payoutLineFile = null;
            normalPayoutFile = null;
            bigPayoutFile = null;
            jacPayoutFile = null;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("�����o���f�[�^�x�[�X�쐬");
            payoutLineFile = (TextAsset)EditorGUILayout.ObjectField("PayoutLines:", payoutLineFile, typeof(TextAsset), true);
            normalPayoutFile = (TextAsset)EditorGUILayout.ObjectField("NormalPayouts:", normalPayoutFile, typeof(TextAsset), true);
            bigPayoutFile = (TextAsset)EditorGUILayout.ObjectField("BigPayouts:", bigPayoutFile, typeof(TextAsset), true);
            jacPayoutFile = (TextAsset)EditorGUILayout.ObjectField("JacPayouts:", jacPayoutFile, typeof(TextAsset), true);

            if (GUILayout.Button("�����o���f�[�^�x�[�X�쐬"))
            {
                Debug.Log("Pressed");
                MakePayoutData(payoutLineFile, normalPayoutFile, bigPayoutFile, jacPayoutFile);
            }
        }

        private void MakePayoutData(TextAsset payoutLineFile, TextAsset normalPayoutFile, TextAsset bigPayoutFile, TextAsset jacPayoutFile)
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
            payoutDatabase.SetPayoutLines(MakePayoutLineDatas(payoutLineFile));
            // �����o���g�ݍ��킹�\�쐬
            // �ʏ펞
            payoutDatabase.SetNormalPayout(MakeResultDatas(normalPayoutFile));
            // �����Q�[����
            payoutDatabase.SetBigPayout(MakeResultDatas(bigPayoutFile));
            // JAC�Q�[����
            payoutDatabase.SetJacPayout(MakeResultDatas(jacPayoutFile));

            // �ۑ�����
            AssetDatabase.CreateAsset(payoutDatabase, Path.Combine(path, fileName));
            Debug.Log("Payout Database is generated");
        }

        private List<PayoutLineData> MakePayoutLineDatas(TextAsset payoutLineFile)
        {
            List<PayoutLineData> finalResult = new List<PayoutLineData>();
            StringReader buffer = new StringReader(payoutLineFile.text);

            Debug.Log(buffer.ToString());

            // �S�Ă̍s��ǂݍ���
            while (buffer.Peek() != -1)
            {
                finalResult.Add(new PayoutLineData(buffer));
            }
            return finalResult;
        }

        private List<PayoutResultData> MakeResultDatas(TextAsset payoutResultFile)
        {
            List<PayoutResultData> finalResult = new List<PayoutResultData>();
            StringReader buffer = new StringReader(payoutResultFile.text);

            // �S�Ă̍s��ǂݍ���
            while (buffer.Peek() != -1)
            {
                finalResult.Add(new PayoutResultData(buffer));
            }
            return finalResult;
        }
    }
#endif
}

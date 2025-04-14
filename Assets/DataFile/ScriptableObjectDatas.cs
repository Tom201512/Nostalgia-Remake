using ReelSpinGame_Reels;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    [Serializable]
    public class ReelConditionsData
    {
        // const
        // ������ǂݍ��ރo�C�g��
        public const int ConditionMaxRead = 5;
        // ����~���[����~�ʒu��ǂݍ��ރo�C�g��
        public const int FirstReelPosMaxRead = ConditionMaxRead + 10;
        // �e�[�u��ID�̓ǂݍ��ރo�C�g��
        public const int ReelTableIDMaxRead = FirstReelPosMaxRead + 1;
        // ������ǂݍ��ލۂɂ��炷�r�b�g��
        public const int ConditionBitOffset = 4;

        // enum
        // �����̃V���A���C�Y
        public enum ConditionID { Flag, FirstPush, Bonus, Bet, Random }

        // var
        // �e�[�u���g�p������int�^�f�[�^�Ŋi�[���A1�o�C�g(8bit)���ƂɃf�[�^�𕪂���
        // �t���OID, ����~, �{�[�i�X, �x�b�g����, �����_������̏��œǂݍ���
        [SerializeField] int mainConditions;
        [SerializeField] int firstReelPosition;
        [SerializeField] byte reelTableNumber;

        public int MainConditions { get; private set; }

        // ����~�������[���̈ʒu(��������ƂɃe�[�u���̕ύX������)
        public int FirstReelPosition { get; private set; }

        // �g�p����e�[�u���ԍ�
        public byte ReelTableNumber { get; private set; }

        // �R���X�g���N�^
        public ReelConditionsData(StringReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // �ǂݍ��݊J�n(�v�f�ԍ������ƂɃf�[�^��ǂݍ���
            foreach (string value in values)
            {
                // ���C������(16�i���œǂݍ���int�^�ň��k)
                if (indexNum < ConditionMaxRead)
                {
                    int offset = (int)Math.Pow(16, indexNum);
                    MainConditions += Convert.ToInt32(value) * offset;
                }

                // ��ꃊ�[����~
                else if (indexNum < FirstReelPosMaxRead)
                {
                    FirstReelPosition += ConvertToArrayBit(Convert.ToInt32(value));
                }

                // �e�[�u��ID�ǂݍ���
                else if (indexNum < ReelTableIDMaxRead)
                {
                    ReelTableNumber = Convert.ToByte(value);
                }

                // �Ō�̈�s�͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum++;
            }

            // �f�o�b�O�p
            string ConditionDebug = "";

            for (int i = 0; i < 5; i++)
            {
                ConditionDebug += GetConditionData(i).ToString() + ",";
            }
            Debug.Log("Condition:" + MainConditions + "Details:" + ConditionDebug + "FirstReel:" + FirstReelPosition + "ReelTableNum" + ReelTableNumber);
        }

        // func
        // �e������int�ɂ���
        public static int ConvertConditionData(int flagID, int firstPush, int bonus, int bet, int random)
        {
            // 16�i���̃f�[�^�֕ύX
            int conditions = 0;
            // �z��ɂ���
            int[] conditionArray = { flagID, firstPush, bonus, bet, random };

            for (int i = 0; i < (int)ConditionID.Random; i++)
            {
                conditions |= conditionArray[i] << ConditionBitOffset * i;
            }
            return conditions;
        }

        // �e�����̐��l��Ԃ�
        public int GetConditionData(int conditionID) => ((MainConditions >> ConditionBitOffset * conditionID) & 0xF);

        // �f�[�^���r�b�g�ɂ���
        private int ConvertToArrayBit(int data)
        {
            //Do not convert if data is 0
            if (data == 0) { return 0; }
            return (int)Math.Pow(2, data);
        }
    }

    // ���[������e�[�u��
    [Serializable]
    public class ReelTableData
    {
        // ���[������e�[�u��(�f�B���C���i�[����)
        public byte[] TableData { get; private set; } = new byte[ReelData.MaxReelArray];

        public ReelTableData(StringReader LoadedData)
        {
            string[] values = LoadedData.ReadLine().Split(',');
            int indexNum = 0;

            // �f�o�b�O�p
            string debugBuffer = "";

            // �ǂݍ��݊J�n
            foreach (string value in values)
            {
                // ���[���f�[�^��ǂݍ���
                if (indexNum < ReelData.MaxReelArray)
                {
                    TableData[indexNum] = Convert.ToByte(value);
                    debugBuffer += TableData[indexNum];
                }

                // �Ō�̈�s�͓ǂ܂Ȃ�(�e�[�u����)
                else
                {
                    break;
                }
                indexNum++;
            }

            Debug.Log("Array:" + debugBuffer);
        }
    }

    public class ScriptableGenerator : EditorWindow
    {
        // const

        // var

        // �t�@�C���w��
        private TextAsset arrayData;
        private TextAsset conditionsData;
        private TextAsset delayData;

        // func
        // ���[���z��̍쐬
        [MenuItem("ScriptableGen/ReelData Generator")]
        private static void OpenWindow()
        {
            Debug.Log("Open ReelData Generator");
            ScriptableGenerator window = GetWindow<ScriptableGenerator>();
            window.titleContent = new GUIContent("ReelData Generator");
        }

        private void Awake()
        {
            arrayData = null;
            conditionsData = null;
            delayData = null;
            Debug.Log("Awake done");
        }

        private void OnGUI()
        {
            GUILayout.Label("���[���z��t�@�C���A�����e�[�u���A�f�B���C�e�[�u��");
            arrayData = (TextAsset)EditorGUILayout.ObjectField("Array:", arrayData, typeof(TextAsset), true);
            conditionsData = (TextAsset)EditorGUILayout.ObjectField("Conditions:", conditionsData, typeof(TextAsset), true);
            delayData = (TextAsset)EditorGUILayout.ObjectField("Tables:", delayData, typeof(TextAsset), true);

            if (GUILayout.Button("���[���f�[�^�쐬"))
            {
                Debug.Log("Pressed");
                MakeReelData(arrayData);
            }
        }

        private void MakeReelData(TextAsset arrayFile)
        {
            // �f�B���N�g���̍쐬
            string path = "Assets/ReelDatas";

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // �X�N���v�^�u���I�u�W�F�N�g�쐬
            ReelDatabase reelDatabase = CreateInstance<ReelDatabase>();

            // �z��쐬
            reelDatabase.Array = MakeReelArray(reelDatabase, arrayFile);

            var fileName = arrayFile.name + ".asset";

            foreach (byte value in reelDatabase.Array)
            {
                Debug.Log(value + "Symbol:" + ReelData.ReturnSymbol(value));
            }

            for (int i = 0; i < ReelData.MaxReelArray; i++)
            {
                Debug.Log("No." + i + " Symbol:" + ReelData.ReturnSymbol(reelDatabase.Array[i]));
            }

            // �ۑ�����
            AssetDatabase.CreateAsset(reelDatabase, Path.Combine(path, fileName));
            Debug.Log("ReelData is generated");
        }

        private byte[] MakeReelArray(ReelDatabase reelDatabase, TextAsset arrayFile)
        {
            StringReader buffer = new StringReader(arrayFile.text);
            // �z��ǂݍ���
            string[] values = buffer.ReadLine().Split(',');
            // �z��ɕϊ�
            return Array.ConvertAll(values, byte.Parse);
        }
    }
}


using ReelSpinGame_Sound.BGM;
using ReelSpinGame_Sound.SE;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReelSpinGame_Datas
{
    // サウンドパックを作成する
#if UNITY_EDITOR
    public class SoundDataGen : EditorWindow
    {
        // var

        // 作成中のサウンドパック
        private SePack sePack;
        private BgmPack bgmPack;

        // サウンドパックの名前
        private string soundPackName;

        // スクロール数値
        private Vector2 scrollPos;

        // func
        [MenuItem("ScriptableGen/SoundPackGenerator")]
        private static void OpenWindow()
        {
            Debug.Log("Open SoundPack Generator");
            SoundDataGen window = GetWindow<SoundDataGen>();
            window.titleContent = new GUIContent("SoundPack Generator");
        }

        private void OnEnable()
        {
            soundPackName = "";
            sePack = CreateInstance<SePack>();
            bgmPack = CreateInstance<BgmPack>();
            scrollPos = new Vector2(0, 0);
        }

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("サウンドパック作成\n");

                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    Editor.CreateEditor(sePack).OnInspectorGUI();
                    Editor.CreateEditor(bgmPack).OnInspectorGUI();
                }

                soundPackName = EditorGUILayout.TextField("サウンドパック名", soundPackName);

                if (GUILayout.Button("サウンドパックファイル作成"))
                {
                    if(NullCheck())
                    {
                        MakeSoundPack();
                    }
                }
            }
        }

        private bool NullCheck()
        {
            if (!sePack.NullCheck())
            {
                Debug.LogError("All se files are not selected");
                return false;
            }

            if (!bgmPack.NullCheck())
            {
                Debug.LogError("All bgm files are not selected");
                return false;
            }

            if (soundPackName == null)
            {
                Debug.LogError("SoundPackName is invalid");
                return false;
            }

            Debug.Log("Null Check passed");
            return true;
        }

        private void MakeSoundPack()
        {
            // ディレクトリの作成
            string path = Path.Combine("Assets/SoundPack", soundPackName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            SoundPack soundPack = CreateInstance<SoundPack>();

            // SE, BGMの生成
            soundPack.SetSE(sePack);
            soundPack.SetBGM(bgmPack);

            // 保存処理
            GenerateFile(path, soundPackName, soundPack);
            GenerateFile(path, "SE", sePack);
            GenerateFile(path, "BGM", bgmPack);
        }

        private void GenerateFile(string path, string fileName, ScriptableObject scriptableObject)
        {
            // 保存処理
            // ファイルがある場合
            if (File.Exists(Path.Combine(path, fileName) + ".asset"))
            {
                // ディレクトリの作成
                string temporaryPath = "Assets/Temp";

                Directory.CreateDirectory(temporaryPath);
                Debug.Log("Temporary Directory is created");

                // 書き換え用の仮ファイルを作成
                AssetDatabase.CreateAsset(scriptableObject, Path.Combine(temporaryPath, fileName) + ".asset");
                // 元あったファイルに置き換える
                FileUtil.ReplaceFile(Path.Combine(temporaryPath, fileName) + ".asset", Path.Combine(path, fileName) + ".asset");

                // 仮ファイルを削除
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

    // SEパックの作成
    [CustomEditor(typeof(SePack))]
    public class SePackEditor : Editor
    {
        private SerializedProperty bet;
        private SerializedProperty wait;
        private SerializedProperty start;
        private SerializedProperty spStart;
        private SerializedProperty stop;
        private SerializedProperty redRiichiSound;
        private SerializedProperty blueRiichiSound;
        private SerializedProperty bb7RiichiSound;
        private SerializedProperty replay;
        private SerializedProperty normalPayout;
        private SerializedProperty maxPayout;
        private SerializedProperty jacPayout;
        private SerializedProperty redStart;
        private SerializedProperty redEnd;
        private SerializedProperty blueStart;
        private SerializedProperty blueEnd;
        private SerializedProperty blackStart;
        private SerializedProperty blackEnd;
        private SerializedProperty regStart;

        public void Awake()
        {
            bet = serializedObject.FindProperty("bet");
            wait = serializedObject.FindProperty("wait");
            start = serializedObject.FindProperty("start");
            spStart = serializedObject.FindProperty("spStart");
            stop = serializedObject.FindProperty("stop");
            redRiichiSound = serializedObject.FindProperty("redRiichiSound");
            blueRiichiSound = serializedObject.FindProperty("blueRiichiSound");
            bb7RiichiSound = serializedObject.FindProperty("bb7RiichiSound");
            replay = serializedObject.FindProperty("replay");
            normalPayout = serializedObject.FindProperty("normalPayout");
            maxPayout = serializedObject.FindProperty("maxPayout");
            jacPayout = serializedObject.FindProperty("jacPayout");
            redStart = serializedObject.FindProperty("redStart");
            redEnd = serializedObject.FindProperty("redEnd");
            blueStart = serializedObject.FindProperty("blueStart");
            blueEnd = serializedObject.FindProperty("blueEnd");
            blackStart = serializedObject.FindProperty("blackStart");
            blackEnd = serializedObject.FindProperty("blackEnd");
            regStart = serializedObject.FindProperty("regStart");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("SE");
            EditorGUILayout.PropertyField(bet);
            EditorGUILayout.PropertyField(wait);
            EditorGUILayout.PropertyField(start);
            EditorGUILayout.PropertyField(spStart);
            EditorGUILayout.PropertyField(stop);
            EditorGUILayout.PropertyField(redRiichiSound);
            EditorGUILayout.PropertyField(blueRiichiSound);
            EditorGUILayout.PropertyField(bb7RiichiSound);
            EditorGUILayout.PropertyField(replay);
            EditorGUILayout.PropertyField(normalPayout);
            EditorGUILayout.PropertyField(maxPayout);
            EditorGUILayout.PropertyField(jacPayout);
            EditorGUILayout.PropertyField(redStart);
            EditorGUILayout.PropertyField(redEnd);
            EditorGUILayout.PropertyField(blueStart);
            EditorGUILayout.PropertyField(blueEnd);
            EditorGUILayout.PropertyField(blackStart);
            EditorGUILayout.PropertyField(blackEnd);
            EditorGUILayout.PropertyField(regStart);
            serializedObject.ApplyModifiedProperties();
        }
    }

    // BGMパックの作成
    [CustomEditor(typeof(BgmPack))]
    public class BgmPackEditor : Editor
    {
        private SerializedProperty redBGM;
        private SerializedProperty redJAC;
        private SerializedProperty blueBGM;
        private SerializedProperty blueJAC;
        private SerializedProperty blackBGM;
        private SerializedProperty blackJAC;
        private SerializedProperty regJAC;

        public void Awake()
        {
            redBGM = serializedObject.FindProperty("redBGM");
            redJAC = serializedObject.FindProperty("redJAC");
            blueBGM = serializedObject.FindProperty("blueBGM");
            blueJAC = serializedObject.FindProperty("blueJAC");
            blackBGM = serializedObject.FindProperty("blackBGM");
            blackJAC = serializedObject.FindProperty("blackJAC");
            regJAC = serializedObject.FindProperty("regJAC");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("BGM");
            EditorGUILayout.PropertyField(redBGM);
            EditorGUILayout.PropertyField(redJAC);
            EditorGUILayout.PropertyField(blueBGM);
            EditorGUILayout.PropertyField(blueJAC);
            EditorGUILayout.PropertyField(blackBGM);
            EditorGUILayout.PropertyField(blackJAC);
            EditorGUILayout.PropertyField(regJAC);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

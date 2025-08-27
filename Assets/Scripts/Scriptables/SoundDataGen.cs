using ReelSpinGame_Sound;
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
        private SEPack se;
        private BGMPack bgm;

        // サウンドパックの名前
        private string sePackName;

        // スクロール数値
        private Vector2 scrollValue;

        // func
        [MenuItem("ScriptableGen/SoundPackGenerator")]
        private static void OpenWindow()
        {
            Debug.Log("Open SoundPack Generator");
            SoundDataGen window = GetWindow<SoundDataGen>();
            window.titleContent = new GUIContent("SoundPack Generator");
        }

        private void Awake()
        {
            sePackName = "";
            se = new SEPack();
            bgm = new BGMPack();
        }

        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollValue))
            {
                scrollValue = scrollView.scrollPosition;
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label("サウンドパック作成\n");
                    GUILayout.Label("効果音\n");

                    se.Bet = (SeFile)EditorGUILayout.ObjectField("ベット:", se.Bet, typeof(SeFile), true);
                    se.Wait = (SeFile)EditorGUILayout.ObjectField("ウェイト:", se.Wait, typeof(SeFile), true);
                    se.Start = (SeFile)EditorGUILayout.ObjectField("スタート:", se.Start, typeof(SeFile), true);
                    se.SpStart = (SeFile)EditorGUILayout.ObjectField("告知音スタート:", se.SpStart, typeof(SeFile), true);
                    se.Stop = (SeFile)EditorGUILayout.ObjectField("停止:", se.Stop, typeof(SeFile), true);
                    se.RedRiichiSound = (SeFile)EditorGUILayout.ObjectField("赤7リーチ:", se.RedRiichiSound, typeof(SeFile), true);
                    se.BlueRiichiSound = (SeFile)EditorGUILayout.ObjectField("青7リーチ:", se.BlueRiichiSound, typeof(SeFile), true);
                    se.BB7RiichiSound = (SeFile)EditorGUILayout.ObjectField("BB7リーチ:", se.BB7RiichiSound, typeof(SeFile), true);
                    se.Replay = (SeFile)EditorGUILayout.ObjectField("リプレイ:", se.Replay, typeof(SeFile), true);
                    se.NormalPayout = (SeFile)EditorGUILayout.ObjectField("通常払い出し:", se.NormalPayout, typeof(SeFile), true);
                    se.MaxPayout = (SeFile)EditorGUILayout.ObjectField("15枚払い出し:", se.MaxPayout, typeof(SeFile), true);
                    se.JacPayout = (SeFile)EditorGUILayout.ObjectField("JAC払い出し:", se.JacPayout, typeof(SeFile), true);

                    GUILayout.Label("\n音楽\n");

                    // 赤7
                    GUILayout.Label("\n赤7BIG\n");

                    se.RedStart = (SeFile)EditorGUILayout.ObjectField("赤7開始:", se.RedStart, typeof(SeFile), true);
                    bgm.RedBGM = (BgmFile)EditorGUILayout.ObjectField("赤7BGM:", bgm.RedBGM, typeof(BgmFile), true);
                    bgm.RedJAC = (BgmFile)EditorGUILayout.ObjectField("赤7JACBGM:", bgm.RedJAC, typeof(BgmFile), true);
                    se.RedEnd = (SeFile)EditorGUILayout.ObjectField("赤7終了:", se.RedEnd, typeof(SeFile), true);

                    // 青7
                    GUILayout.Label("\n青7BIG\n");

                    se.BlueStart = (SeFile)EditorGUILayout.ObjectField("青7開始:", se.BlueStart, typeof(SeFile), true);
                    bgm.BlueBGM = (BgmFile)EditorGUILayout.ObjectField("青7BGM:", bgm.BlueBGM, typeof(BgmFile), true);
                    bgm.BlueJAC = (BgmFile)EditorGUILayout.ObjectField("青7JACBGM:", bgm.BlueJAC, typeof(BgmFile), true);
                    se.BlueEnd = (SeFile)EditorGUILayout.ObjectField("青7終了:", se.BlueEnd, typeof(SeFile), true);

                    // BB7
                    GUILayout.Label("\nBB7BIG\n");

                    se.BlackStart = (SeFile)EditorGUILayout.ObjectField("BB7開始:", se.BlackStart, typeof(SeFile), true);
                    bgm.BlackBGM = (BgmFile)EditorGUILayout.ObjectField("BB7BGM:", bgm.BlackBGM, typeof(BgmFile), true);
                    bgm.BlackJAC = (BgmFile)EditorGUILayout.ObjectField("BB7JACBGM:", bgm.BlackJAC, typeof(BgmFile), true);
                    se.BlackEnd = (SeFile)EditorGUILayout.ObjectField("BB7終了:", se.BlackEnd, typeof(SeFile), true);

                    // ボーナスゲーム

                    GUILayout.Label("\nREG\n");

                    se.RegStart = (SeFile)EditorGUILayout.ObjectField("REG開始:", se.RegStart, typeof(SeFile), true);
                    bgm.RegJAC = (BgmFile)EditorGUILayout.ObjectField("REGJACBGM:", bgm.RegJAC, typeof(BgmFile), true);
                    GUILayout.Label("\nファイル\n");

                    sePackName = EditorGUILayout.TextField("サウンドパック名", sePackName);

                    if (GUILayout.Button("サウンドパックファイル作成"))
                    {
                        if(NullCheck())
                        {
                            MakeSoundPack();
                        }
                    }
                }
            }
        }

        private bool NullCheck()
        {
            if(!se.NullCheck())
            {
                Debug.LogError("All se files are not selected");
                return false;
            }

            if(!bgm.NullCheck())
            {
                Debug.LogError("All bgm files are not selected");
                return false;
            }

            Debug.Log("Null Check passed");
            return true;
        }

        private void MakeSoundPack()
        {
            // ディレクトリの作成
            string path = "Assets/SoundPack";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("Directory is created");
            }

            // スクリプタブルオブジェクト作成
            SoundDatabase seDatabase = CreateInstance<SoundDatabase>();

            // サウンド割り当て
            seDatabase.SetSoundEffectPack(se);
            seDatabase.SetMusicPack(bgm);

            // 保存処理
            GenerateFile(path, sePackName, seDatabase);
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
#endif
}

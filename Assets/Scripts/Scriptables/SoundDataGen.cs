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
        private SoundEffectPack sound;
        private BGMPack music;

        // サウンドパックの名前
        private string soundPackName;

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
            soundPackName = "";
            sound = new SoundEffectPack();
            music = new BGMPack();
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

                    sound.Bet = (AudioClip)EditorGUILayout.ObjectField("ベット:", sound.Bet, typeof(AudioClip), true);
                    sound.Wait = (AudioClip)EditorGUILayout.ObjectField("ウェイト:", sound.Wait, typeof(AudioClip), true);
                    sound.Start = (AudioClip)EditorGUILayout.ObjectField("スタート:", sound.Start, typeof(AudioClip), true);
                    sound.SpStart = (AudioClip)EditorGUILayout.ObjectField("告知音スタート:", sound.SpStart, typeof(AudioClip), true);
                    sound.Stop = (AudioClip)EditorGUILayout.ObjectField("停止:", sound.Stop, typeof(AudioClip), true);
                    sound.RedRiichiSound = (AudioClip)EditorGUILayout.ObjectField("赤7リーチ:", sound.RedRiichiSound, typeof(AudioClip), true);
                    sound.BlueRiichiSound = (AudioClip)EditorGUILayout.ObjectField("青7リーチ:", sound.BlueRiichiSound, typeof(AudioClip), true);
                    sound.BB7RiichiSound = (AudioClip)EditorGUILayout.ObjectField("BB7リーチ:", sound.BB7RiichiSound, typeof(AudioClip), true);
                    sound.Replay = (AudioClip)EditorGUILayout.ObjectField("リプレイ:", sound.Replay, typeof(AudioClip), true);
                    sound.NormalPayout = (AudioClip)EditorGUILayout.ObjectField("通常払い出し:", sound.NormalPayout, typeof(AudioClip), true);
                    sound.MaxPayout = (AudioClip)EditorGUILayout.ObjectField("15枚払い出し:", sound.MaxPayout, typeof(AudioClip), true);
                    sound.JacPayout = (AudioClip)EditorGUILayout.ObjectField("JAC払い出し:", sound.JacPayout, typeof(AudioClip), true);

                    GUILayout.Label("\n音楽\n");

                    // 赤7
                    GUILayout.Label("\n赤7BIG\n");

                    music.RedStart = (AudioClip)EditorGUILayout.ObjectField("赤7開始:", music.RedStart, typeof(AudioClip), true);
                    music.RedBGM = (AudioClip)EditorGUILayout.ObjectField("赤7BGM:", music.RedBGM, typeof(AudioClip), true);
                    music.RedJAC = (AudioClip)EditorGUILayout.ObjectField("赤7JACBGM:", music.RedJAC, typeof(AudioClip), true);
                    music.RedEnd = (AudioClip)EditorGUILayout.ObjectField("赤7終了:", music.RedEnd, typeof(AudioClip), true);

                    // 青7
                    GUILayout.Label("\n青7BIG\n");

                    music.BlueStart = (AudioClip)EditorGUILayout.ObjectField("青7開始:", music.BlueStart, typeof(AudioClip), true);
                    music.BlueBGM = (AudioClip)EditorGUILayout.ObjectField("青7BGM:", music.BlueBGM, typeof(AudioClip), true);
                    music.BlueJAC = (AudioClip)EditorGUILayout.ObjectField("青7JACBGM:", music.BlueJAC, typeof(AudioClip), true);
                    music.BlueEnd = (AudioClip)EditorGUILayout.ObjectField("青7終了:", music.BlueEnd, typeof(AudioClip), true);

                    // BB7
                    GUILayout.Label("\nBB7BIG\n");

                    music.BlackStart = (AudioClip)EditorGUILayout.ObjectField("BB7開始:", music.BlackStart, typeof(AudioClip), true);
                    music.BlackBGM = (AudioClip)EditorGUILayout.ObjectField("BB7BGM:", music.BlackBGM, typeof(AudioClip), true);
                    music.BlackJAC = (AudioClip)EditorGUILayout.ObjectField("BB7JACBGM:", music.BlackJAC, typeof(AudioClip), true);
                    music.BlackEnd = (AudioClip)EditorGUILayout.ObjectField("BB7終了:", music.BlackEnd, typeof(AudioClip), true);

                    // ボーナスゲーム

                    GUILayout.Label("\nREG\n");

                    music.RegStart = (AudioClip)EditorGUILayout.ObjectField("REG開始:", music.RegStart, typeof(AudioClip), true);
                    music.RegJAC = (AudioClip)EditorGUILayout.ObjectField("REGJACBGM:", music.RegJAC, typeof(AudioClip), true);
                    GUILayout.Label("\nファイル\n");

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
        }

        private bool NullCheck()
        {
            if(!SoundNullCheck())
            {
                Debug.LogError("All sound files are not selected");
                return false;
            }

            if(!BGMNullCheck())
            {
                Debug.LogError("All music files are not selected");
                return false;
            }

            Debug.Log("Null Check passed");
            return true;
        }

        private bool SoundNullCheck()
        {
            if(sound.Bet == null) { return false; }
            if(sound.Wait == null) { return false; }
            if(sound.Start == null) { return false; }
            if(sound.SpStart == null) { return false; }
            if(sound.Stop == null) { return false; }
            if(sound.RedRiichiSound == null) { return false; }
            if(sound.BlueRiichiSound == null) { return false; }
            if(sound.BB7RiichiSound == null) { return false; }
            if(sound.Replay == null) { return false; }
            if(sound.NormalPayout == null) { return false; }
            if(sound.MaxPayout == null) { return false; }
            if(sound.JacPayout == null) { return false; }

            return true;
        }

        private bool BGMNullCheck()
        {
            if (music.RedStart == null) { return false; }
            if (music.RedBGM == null) { return false; }
            if (music.RedJAC == null) { return false; }
            if (music.RedEnd == null) { return false; }
            if (music.BlueStart == null) { return false; }
            if (music.BlueBGM == null) { return false; }
            if (music.BlueJAC == null) { return false; }
            if (music.BlueEnd == null) { return false; }
            if (music.BlackStart == null) { return false; }
            if (music.BlackBGM == null) { return false; }
            if (music.BlackJAC == null) { return false; }
            if (music.BlackEnd == null) { return false; }
            if (music.RegStart == null) { return false; }
            if (music.RegJAC == null) { return false; }

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
            SoundDatabase soundDatabase = CreateInstance<SoundDatabase>();

            // サウンド割り当て
            soundDatabase.SetSoundEffectPack(sound);
            soundDatabase.SetMusicPack(music);

            // 保存処理
            GenerateFile(path, soundPackName, soundDatabase);
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

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;


namespace EModules
{


    [InitializeOnLoad]
    public static class AutoSaveHandler
    {
        //public static string relativeSavePath = "Assets/AutoSave/";
        public static string autoSaveFileName
        {
            get
            {
                if (!System.IO.Directory.Exists(Application.dataPath + "/" + AutoSaveFolder))
                {
                    System.IO.Directory.CreateDirectory(Application.dataPath + "/" + AutoSaveFolder);
                    AssetDatabase.Refresh();
                }
                //if (!AssetDatabase.IsValidFolder("Assets/" + AutoSaveFolder)) AssetDatabase.CreateFolder("Assets", AutoSaveFolder);
                var files = System.IO.Directory.GetFiles(Application.dataPath + "/" + AutoSaveFolder).Select(f => f.Replace('\\', '/')).Where(f =>
                f.EndsWith(".unity") && f.Substring(f.LastIndexOf('/') + 1).StartsWith("AutoSave")).ToArray();
                if (files == null || files.Length == 0) return "AutoSave_00";

                var times = files.Select(System.IO.File.GetCreationTime).ToList();
                var max = times.Max();
                var ind = times.IndexOf(max);
                var count = 0;
                files = files.Select(n => n.Remove(n.LastIndexOf('.'))).ToArray();
                if (int.TryParse(files[ind].Substring(files[ind].Length - 2), out count))
                {
                    count = (count + 1) % savefilescount;
                    return "AutoSave_" + count.ToString("D2");
                }
                return "AutoSave_00";
            }
        }
        public static float secondsBetweenSaves = 300;
        public static bool showDebug = false;

        static AutoSaveHandler()
        {
            EditorApplication.update -= UpdateCS;
            EditorApplication.update += UpdateCS;

            //  lastSave = (float)EditorApplication.timeSinceStartup;
            resetSet();
        }

        static void resetSet()
        {

            if (!EditorPrefs.HasKey("enablesave")) EditorPrefs.SetInt("enablesave", 1);
            enablesave = EditorPrefs.GetInt("enablesave") == 1;


            if (EditorPrefs.HasKey("auto1"))
            {
                savefilescount = EditorPrefs.GetInt("auto1");
                secondsBetweenSaves = EditorPrefs.GetInt("auto2") * 60;
                showDebug = EditorPrefs.GetBool("auto3");
            }
        }

        public static bool enablesave = false;
        public static int savefilescount = 10;
        public static float lastSave
        {
            get { return EditorPrefs.GetFloat("nextsave"); }
            set { EditorPrefs.SetFloat("nextsave", value); }

        }
        // public static int timeLeft = 0;
        static double? launchTime;
        public static void UpdateCS()
        {
            if (!enablesave) return;
            if (Application.isPlaying)
            {
                if (launchTime == null) launchTime = EditorApplication.timeSinceStartup;
                return;
            }
            // MonoBehaviour.print(launchTime);
            if (launchTime != null)
            {
                lastSave += (float)(EditorApplication.timeSinceStartup - launchTime.Value);
                launchTime = null;
            }

            if (Mathf.Abs(lastSave - (float)EditorApplication.timeSinceStartup) >= secondsBetweenSaves * 2)
            {
                lastSave = (float)EditorApplication.timeSinceStartup;
                resetSet();
            }
            // timeLeft = (int)(nextsave - EditorApplication.sce);
            // MonoBehaviour.print(lastSave - EditorApplication.timeSinceStartup);
            if (Mathf.Abs(lastSave - (float)EditorApplication.timeSinceStartup) >= secondsBetweenSaves)
            {
                SaveScene();
                EditorApplication.update -= UpdateCS;
                EditorApplication.update += UpdateCS;
            }
        }
        public static void SaveScene()
        {
            if (!System.IO.Directory.Exists(Application.dataPath + "/" + AutoSaveFolder))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/" + AutoSaveFolder);
                AssetDatabase.Refresh();
            }

            var relativeSavePath = "Assets/" + AutoSaveFolder + "/";
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), relativeSavePath + autoSaveFileName + ".unity", true);
            lastSave = (float)EditorApplication.timeSinceStartup;

            if (showDebug)
                Debug.Log("Auto-Save Current Scene: " + relativeSavePath + autoSaveFileName + ".unity");
        }


        static string AutoSaveFolder
        {
            get { return string.IsNullOrEmpty(EditorPrefs.GetString("Auto-Save Location")) ? "AutoSave" : EditorPrefs.GetString("Auto-Save Location"); }
            set
            {
                EditorPrefs.SetString("Auto-Save Location", value);
            }
        }

        [PreferenceItem("Auto-Save")]

        public static void PreferencesGUI()
        {

            EditorGUILayout.LabelField("Assets/" + AutoSaveFolder + " - Auto-Save Location");
            enablesave = EditorGUILayout.Toggle("Automatically Save Current Scene", enablesave);
            GUI.enabled = enablesave;
            savefilescount = Mathf.Clamp(EditorGUILayout.IntField("Maximum Files Version", savefilescount), 1, 99);
            secondsBetweenSaves = Mathf.Clamp(EditorGUILayout.IntField("Save Every (Minutes)", (int)(secondsBetweenSaves / 60)), 1, 60) * 60;
            showDebug = EditorGUILayout.Toggle("Log", showDebug);
            if (GUI.changed)
            {
                EditorPrefs.SetInt("enablesave", enablesave ? 1 : 0);
                EditorPrefs.SetInt("auto1", savefilescount);
                EditorPrefs.SetInt("auto2", (int)(secondsBetweenSaves / 60));
                EditorPrefs.SetBool("auto3", showDebug);
                lastSave = (float)EditorApplication.timeSinceStartup;
                resetSet();
            }
            GUI.enabled = true;
        }

    }
}
#endif
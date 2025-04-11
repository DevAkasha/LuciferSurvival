using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Ironcow.Convenience
{
    [ExecuteInEditMode]
    public class EditorHotkeyManager : Editor
    {
        private static EditorHotkeyManager _instance;
        public static EditorHotkeyManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EditorHotkeyManager();
                return _instance;
            }
        }

        [SerializeField] public UnityEngine.Object templeteFolder;

        [MenuItem("Ironcow/Tool/Screen Shot #&d")]
        static void ScreenShot()
        {
            var path = Application.dataPath + "/screenshot/";
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);
            DirectoryInfo info = new DirectoryInfo(path);
            var idx = info.GetFiles().Length / 2;
            ScreenCapture.CaptureScreenshot(path + "shot" + idx + ".png");
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        [MenuItem("Ironcow/PlayerPrefs/Auto Login Clear")]
        static void SetAutoLoginClear()
        {
            PlayerPrefs.DeleteKey("USER_ID_AUTO");
            PlayerPrefs.DeleteKey("editorNickname");
            PlayerPrefs.DeleteKey("JWT");
        }

        [MenuItem("Ironcow/PlayerPrefs/Tutorial Clear")]
        static void SetTutorialClear()
        {
            PlayerPrefs.DeleteKey("TUTORIAL_CLEAR_DATA");
        }

        [MenuItem("Ironcow/PlayerPrefs/Patch List Clear")]
        static void SetPatchListClear()
        {
            AssetDatabase.DeleteAsset("Assets/StreamingAssets~/meta/PatchList.json");
        }
        private static MethodInfo createScriptMethod = typeof(ProjectWindowUtil).GetMethod("CreateScriptAsset", BindingFlags.Static | BindingFlags.NonPublic);

        [MenuItem("Assets/Create/IroncowScript/UIBase Script", false, -500)]
        static void CreateUIBaseScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UIBaseTemplete.cs.txt";
            var dest = "UI.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/UIListBase Script", false, -500)]
        static void CreateUIListBaseScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UIListBaseTemplete.cs.txt";
            var dest = "UI.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/UIPopupScript", false, -500)]
        static void CreateUIPopupScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UIPopupTemplete.cs.txt";
            var dest = "Popup.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/UICanvasScript", false, -500)]
        static void CreateUICanvasScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UICanvasTemplete.cs.txt";
            var dest = "Canvas.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/UIListItemScript", false, -500)]
        static void CreateUIListItemScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UIListItemTemplete.cs.txt";
            var dest = "Item.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }


        [MenuItem("Assets/Create/IroncowScript/NetworkScript", false, -500)]
        static void CreateRequestScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "NetworkTemplete.cs.txt";
            var dest = "NetworkScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }


        [MenuItem("Assets/Create/IroncowScript/WorldBaseScript", false, -500)]
        static void CreateWorldBaseScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "WorldBaseTemplete.cs.txt";
            var dest = "WorldBaseScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/ManagerScript", false, -500)]
        static void CreateManagerScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "ManagerTemplete.cs.txt";
            var dest = "Manager.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }


        [MenuItem("Assets/Create/IroncowScript/MonoBehaviourScript", false, -500)]
        static void CreateMonoBehaviourScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "MonoBehaviourTemplete.cs.txt";
            var dest = "NewScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/UnitScript", false, -500)]
        static void CreateUnitScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "UnitTemplete.cs.txt";
            var dest = "UnitScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/BaseData", false, -500)]
        static void CreateBaseDataScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "BaseDataTemplete.cs.txt";
            var dest = "DataSO.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/OriginClass", false, -500)]
        static void CreateOriginClassScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "OriginClassTemplete.cs.txt";
            var dest = "NewScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/BlankClass", false, -500)]
        static void CreateBlankClassScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "BlankTemplete.cs.txt";
            var dest = "NewScript.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/MonoSingletonClass", false, -500)]
        static void CreateMonoSingletonClassScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "MonoSingletonTemplete.cs.txt";
            var dest = "Singleton.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }

        [MenuItem("Assets/Create/IroncowScript/SoSingletonClass", false, -500)]
        static void CreateSoSingletonClassScript()
        {
            string templetePath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(instance.templeteFolder) + "/";
            var template = templetePath + "SoSingletonTemplete.cs.txt";
            var dest = "Singleton.cs";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
#if UNITY_EDITOR
#if USE_ADDRESSABLE
using UnityEditor.AddressableAssets.Settings;
#endif
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEditor.PackageManager.UI;
using UnityEditor.PackageManager;
#endif

namespace Ironcow
{
	[CustomEditor(typeof(FrameworkController))]
	public class FrameworkControllerEditor : Editor
	{
		static bool DrawToolbar(ref int selected, string[] texts)
		{
			bool dirty = false;

			int value = GUILayout.Toolbar(selected, texts, EditorStyles.toolbarButton);
			if (value != selected)
			{
				selected = value;
				dirty = true;
			}

			return dirty;
		}

		private FrameworkController controller = null;
		private List<string> toolbarTexts = null;
		private int selected = 0;

		private void OnEnable()
		{
			this.controller = base.target as FrameworkController;

			this.toolbarTexts = new List<string>();
			this.toolbarTexts.Add("Property");
            this.controller.isLocale = IsSymbolAlreadyDefined("USE_LOCALE");
            this.controller.isAutoCaching = IsSymbolAlreadyDefined("USE_AUTO_CACHING");
            this.controller.isScriptableObjectData = IsSymbolAlreadyDefined("USE_SO_DATA");
            this.controller.isAddressableAsync = IsSymbolAlreadyDefined("USE_ADDRESSABLE");
            this.controller.isObjectPool = IsSymbolAlreadyDefined("USE_OBJECT_POOL");
            this.controller.isCloudCode = IsSymbolAlreadyDefined("USE_CLOUD_CODE");
            this.controller.isFSM = IsSymbolAlreadyDefined("USE_FSM");
        }

		public override void OnInspectorGUI()
		{
			if (this.controller == null)
				return;

            GUILayoutOption option = GUILayout.Width(400);
			if (Type.GetType("Ironcow.LocalizeTool.LocalizeTool") != null)
			{
				EditorGUILayout.LabelField("Locale", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				{
					this.controller.isLocale = EditorGUILayout.ToggleLeft("Use Locale", this.controller.isLocale, option);
					if (this.controller.isLocale)
					{
						AddDefineSymbol("USE_LOCALE");
					}
					else
					{
						RemoveDefineSymbol("USE_LOCALE");
					}
				}
				EditorGUI.indentLevel--;
				GUILayout.Space(10f);
			}

			if (Type.GetType("Ironcow.Convenience.PrefabAutoCache") != null)
            {
                EditorGUILayout.LabelField("Auto Caching", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isAutoCaching = EditorGUILayout.ToggleLeft("Use Auto Caching", this.controller.isAutoCaching);
                    if (this.controller.isAutoCaching)
                    {
                        AddDefineSymbol("USE_AUTO_CACHING");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_AUTO_CACHING");
                    }
                }
				EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }

            if (Type.GetType("Ironcow.Data.DataTool") != null)
            {
                EditorGUILayout.LabelField("SO Data", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isScriptableObjectData = EditorGUILayout.ToggleLeft("Use ScriptableObject Data", this.controller.isScriptableObjectData);
                    if (this.controller.isScriptableObjectData)
                    {
                        AddDefineSymbol("USE_SO_DATA");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_SO_DATA");
                    }
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }

            if (Type.GetType("Ironcow.ObjectPool.ObjectPoolTool") != null)
            {
                EditorGUILayout.LabelField("Object Pool", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isObjectPool = EditorGUILayout.ToggleLeft("Use Object Pool", this.controller.isObjectPool);
                    if (this.controller.isObjectPool)
                    {
                        AddDefineSymbol("USE_OBJECT_POOL");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_OBJECT_POOL");
                    }
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }
            
            if (Type.GetType("Ironcow.FSM.FSMEditor") != null)
            {
                EditorGUILayout.LabelField("FSM", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isFSM = EditorGUILayout.ToggleLeft("Use FSM", this.controller.isFSM);
                    if (this.controller.isFSM)
                    {
                        AddDefineSymbol("USE_FSM");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_FSM");
                    }
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }
#if UNITY_EDITOR
            var info = GetPackageInfo("com.unity.addressables");
            
            if (Type.GetType("Ironcow.Addressable.AddressableUtils") != null && info != null)
            {
                EditorGUILayout.LabelField("Addressable", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isAddressableAsync = EditorGUILayout.ToggleLeft("Use Addressable", this.controller.isAddressableAsync);
                    if (this.controller.isAddressableAsync)
                    {
                        AddDefineSymbol("USE_ADDRESSABLE");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_ADDRESSABLE");
                    }
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }

            info = GetPackageInfo("com.unity.services.cloudcode");

            if(info != null)
            {
                EditorGUILayout.LabelField("CloudCode", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    this.controller.isCloudCode = EditorGUILayout.ToggleLeft("Use CloudCode", this.controller.isCloudCode);
                    if (this.controller.isCloudCode)
                    {
                        AddDefineSymbol("USE_CLOUD_CODE");
                    }
                    else
                    {
                        RemoveDefineSymbol("USE_CLOUD_CODE");
                    }
                }
                EditorGUI.indentLevel--;
                GUILayout.Space(10f);
            }
#endif
        }

        public struct DefineSymbolData
        {
            public BuildTargetGroup buildTargetGroup; // 현재 빌드 타겟 그룹
            public string fullSymbolString;           // 현재 빌드 타겟 그룹에서 정의된 심볼 문자열 전체
            public Regex symbolRegex;

            public DefineSymbolData(string symbol)
            {
                buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                fullSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                symbolRegex = new Regex(@"\b" + symbol + @"\b(;|$)");
            }
        }

        /// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
        public static bool IsSymbolAlreadyDefined(string symbol)
        {
            DefineSymbolData dsd = new DefineSymbolData(symbol);

            return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
        }

        /// <summary> 심볼이 이미 정의되어 있는지 검사 </summary>
        public static bool IsSymbolAlreadyDefined(string symbol, out DefineSymbolData dsd)
        {
            dsd = new DefineSymbolData(symbol);

            return dsd.symbolRegex.IsMatch(dsd.fullSymbolString);
        }

        /// <summary> 특정 디파인 심볼 추가 </summary>
        public static void AddDefineSymbol(string symbol)
        {
            // 기존에 존재하지 않으면 끝에 추가
            if (!IsSymbolAlreadyDefined(symbol, out var dsd))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, $"{dsd.fullSymbolString};{symbol}");
            }
        }

        /// <summary> 특정 디파인 심볼 제거 </summary>
        public static void RemoveDefineSymbol(string symbol)
        {
            // 기존에 존재하면 제거
            if (IsSymbolAlreadyDefined(symbol, out var dsd))
            {
                string strResult = dsd.symbolRegex.Replace(dsd.fullSymbolString, "");

                PlayerSettings.SetScriptingDefineSymbolsForGroup(dsd.buildTargetGroup, strResult);
            }
        }
#if UNITY_EDITOR
        private PackageInfo GetPackageInfo(string packageName)
        {
            try
            {
                return UnityEditor.AssetDatabase.FindAssets("package")
                    .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                        .Where(x => UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                    .Select(PackageInfo.FindForAssetPath)
                        .Where(x => x != null)
                    .First(x => x.name == packageName);
            }
            catch { return null; }
        }
#endif
        private void AddPackage(string packageName)
        {
            
        }
    }
}
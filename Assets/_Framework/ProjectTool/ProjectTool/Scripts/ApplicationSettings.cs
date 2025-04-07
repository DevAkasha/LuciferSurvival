using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ironcow.ProjectTool
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class ApplicationSettings : SOSingleton<ApplicationSettings>
	{
#if UNITY_EDITOR
		[MenuItem("Ironcow/Data/Application Settings")]
		private static void Edit()
		{
			Selection.activeObject = Instance;
		}
#endif

		[HideInInspector]
		public List<BuildSettings> BuildSettingsList = new List<BuildSettings>();

		[HideInInspector]
		public bool LogEnabled = true;

		[HideInInspector]
		public BuildSettings CurrentBuildSettings = null;

		[HideInInspector]
		public string FacebookAppName = "";

		[HideInInspector]
		public string FacebookAppId = "";

		[HideInInspector]
		public bool Development = false;

		[HideInInspector]
		public bool TestMode = false;

		[HideInInspector]
		public bool FullBuild = false;
	}
}
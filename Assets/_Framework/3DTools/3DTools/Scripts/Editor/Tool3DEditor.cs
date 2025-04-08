
using UnityEditor;
using UnityEngine;


namespace Ironcow.Tool3D
{
    internal class Tool3DEditor : Editor
    {
        public static void CreateManagerInstance()
        {
            if (GameObject.Find("SpawnManager")) return;
            var obj = new GameObject("SpawnManager");
            obj.AddComponent<SpawnManager>();
        }

    }
}
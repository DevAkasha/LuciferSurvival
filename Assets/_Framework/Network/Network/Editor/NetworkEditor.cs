using UnityEngine;
using UnityEditor;
using System.IO;
using Ironcow.Data;
using Ironcow.Tool3D;
using Ironcow.Network;

namespace Ironcow.Network
{
	public class NetworkEditor : Editor
    {
        public static void CreateManagerInstance()
        {
            if (GameObject.Find("NetworkManager")) return;
            var obj = new GameObject("NetworkManager");
            obj.AddComponent<NetworkManager>();
        }
    }
}
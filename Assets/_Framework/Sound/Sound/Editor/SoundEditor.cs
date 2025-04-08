using UnityEditor;
using UnityEngine;


namespace Ironcow.Sound
{
    internal class SoundEditor : Editor
    {
        public static void CreateManagerInstance()
        {
            if (GameObject.Find("AudioManager")) return;
            var obj = new GameObject("AudioManager");
            obj.AddComponent<AudioManager>();
        }

    }
}
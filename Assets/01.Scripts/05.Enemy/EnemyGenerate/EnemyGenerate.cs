using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyGenerate : MonoBehaviour
{
    [SerializeField] EnemyListSO enemyList;
    [SerializeField] Camera mainCamera;

    public float SpawnRange = 1f;
    public int Wave = 1;

    public void EnemySet()
    {
        int listLimit;

        if (Wave > enemyList.SpawnEnemyList.Count)
        {
            listLimit = enemyList.SpawnEnemyList.Count;
        }
        else
        {
            listLimit = Wave;
        }

        int randomEnemy = Random.Range(0, listLimit);
        Instantiate(enemyList.SpawnEnemyList[randomEnemy]);
    }

    //Vector3 SpawnArea()
    //{
    //    if (mainCamera == null) return Vector3.zero;

    //    Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
    //}
    
}
[CustomEditor(typeof(EnemyGenerate))]
public class EnemySpawn : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("적 생성"))
            {
                ((EnemyGenerate)target).EnemySet();
            }
        }
    }
}
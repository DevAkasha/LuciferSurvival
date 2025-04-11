using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGenerate : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyList;

    public int Wave = 1;

    public void EnemySet()
    {
        int listLimit;

        if(Wave > enemyList.Count)
        {
            listLimit = enemyList.Count;
        }
        else
        {
            listLimit = Wave;
        }

        int randomEnemy = Random.Range(0, listLimit);
        Instantiate(enemyList[randomEnemy]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    private BulletEntity entitiy;

    // Start is called before the first frame update
    void Start()
    {
        //�Ѿ˹߻� �׽�Ʈ������ ����
    }

    // Update is called once per frame
    void Update()
    {
        entitiy.Move();
    }
}

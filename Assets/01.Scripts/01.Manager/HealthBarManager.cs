using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class HealthBarManager : Singleton<HealthBarManager>
{
    protected override bool IsPersistent => false;

    [SerializeField] private Canvas healthbarCanvas;
    [SerializeField] private HealthBarView prefab;
    private readonly Queue<HealthBarView> pool = new();

    public void Attach(AngelController angel)
    {
        var view = Get();
        view.Init(angel);
        view.gameObject.SetActive(true);
    }

    public void Attach(BossController angel)
    {
        var view = Get();
        view.Init(angel);
        view.gameObject.SetActive(true);
    }

    public void Detach(HealthBarView view)
    {
        Return(view);
    }

    private HealthBarView Get()
    {
        if (pool.Count > 0) return pool.Dequeue();
        return Instantiate(prefab, healthbarCanvas.transform);
    }

    private void Return(HealthBarView view)
    {
        view.gameObject.SetActive(false);
        pool.Enqueue(view);
    }
}
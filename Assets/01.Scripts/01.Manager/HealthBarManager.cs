using System.Collections.Generic;
using UnityEngine;



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
    }

    public void Attach(BossController boss)
    {
        var view = Get();
        view.Init(boss);
    }

    public void Detach(HealthBarView view)
    {
        pool.Enqueue(view);
    }

    private HealthBarView Get()
    {
        if (pool.Count > 0) return pool.Dequeue();
        return Instantiate(prefab, healthbarCanvas.transform);
    }
}
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.Events;
using System.Threading.Tasks;

public class Initializer : MonoSingleton<Initializer>
{
    [SerializeField] private List<IManagerInit> managerInits = new List<IManagerInit>();

#if USE_AUTO_CACHING && UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
#else
        void OnValidate()
        {
#endif
        var objects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach(var manager in objects)
        {
            var mng = manager.GetComponent<IManagerInit>();
            if (!managerInits.Contains(mng))
            {
                managerInits.Add(mng);
            }
        }
        managerInits.RemoveAll(obj => obj == null);
    }

#if UNITASK
    public async Task Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        foreach (var manager in managerInits)
        {
            manager.Init(progressTextCallback, progressValueCallback);
            await UniTask.WaitUntil(() => manager.isInit);
        }
    }
#endif
}
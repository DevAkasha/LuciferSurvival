using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DataManagerBase<T, U> : GSpreadReader<T> where T : DataManagerBase<T, U> where U : class
{
    [SerializeField] public U userInfo;

    private Dictionary<string, BaseDataSO> dataDics = new Dictionary<string, BaseDataSO>();

    public override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        AddDataDics(Resources.LoadAll<BaseDataSO>("GameDatas").ToList());
        IsInit = true;
    }

    public D GetData<D>(string rcode) where D : BaseDataSO
    {
        return (D)dataDics[rcode];
    }

    public D GetCloneData<D>(string rcode) where D : BaseDataSO
    {
        return (D)dataDics[rcode].clone;
    }

    public override void AddDataDics<D>(List<D> datas)
    {
        dataDics.AddRange(datas);
    }

    public List<D> GetDatas<D>() where D : BaseDataSO
    {
        List<D> datas = new List<D>();
        foreach (var key in dataDics.Keys)
        {
            if (dataDics[key].GetType().Equals(typeof(T)))
            {
                datas.Add((D)dataDics[key]);
            }
        }
        return datas;
    }
}

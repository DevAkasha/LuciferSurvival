using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.Data
{
    public class DataManagerBase<T, U> : GSpreadReader<T> where T : DataManagerBase<T, U> where U : class
    {
        [SerializeField] public U userInfo;

        private Dictionary<string, BaseDataSO> dataDics = new Dictionary<string, BaseDataSO>();

        public override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
        {
#if USE_ADDRESSABLE
            AddDataDics(ResourceManager.instance.LoadDataAssets<BaseDataSO>());
#else
#if USE_SO_DATA
            AddDataDics(Resources.LoadAll<BaseDataSO>("GameDatas").ToList());
#endif
#endif
            isInit = true;
        }

        public T GetData<T>(string rcode) where T : BaseDataSO
        {
            return (T)dataDics[rcode];
        }

        public T GetCloneData<T>(string rcode) where T : BaseDataSO
        {
            return (T)dataDics[rcode].clone;
        }

        public override void AddDataDics<T>(List<T> datas)
        {
#if USE_SO_DATA
            dataDics.AddRange(datas);
#endif
        }

        public List<T> GetDatas<T>() where T : BaseDataSO
        {
            List<T> datas = new List<T>();
            foreach(var key in dataDics.Keys)
            {
                if (dataDics[key].GetType().Equals(typeof(T)))
                {
                    datas.Add((T)dataDics[key]);
                }
            }
            return datas;
        }
    }
}
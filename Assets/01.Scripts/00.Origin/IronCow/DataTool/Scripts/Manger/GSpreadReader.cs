using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEditor;


public static class GSpreadExtensions
{
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
    {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone)
            tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }

}

[System.Serializable]
public class SheetInfo
{
    public string className;
    public string sheetId;
    public string key;
    public bool isUpdate;
    public List<Dictionary<string, string>> datas;
}

public abstract class GSpreadReader<V> : ManagerBase<V> where V : GSpreadReader<V> 
{
    [Serializable]
    public class GameData<T> where T : BaseDataSO
    {
        public List<T> data = new List<T>();

        public void SetData()
        {
            data = gSpread.ImportData<T>();
            gSpread.AddDataDics(data);
        }
    }

    public static GSpreadReader<V> gSpread;
    [SerializeField] private bool isAutoLoading = false;
    [SerializeField] private string url;
    [SerializeField] private List<SheetInfo> sheets;
    public List<SheetInfo> Sheets => sheets;

    protected override void Awake()
    {
        base.Awake();
        gSpread = this;
        if (isAutoLoading) Init();
    }

    public async override void Init(UnityAction<string> progressTextCallback = null, UnityAction<float> progressValueCallback = null)
    {
        foreach (var sheet in sheets)
        {
            var url = $"{this.url}export?format=tsv&gid={sheet.sheetId}";
            var req = UnityWebRequest.Get(url);
            var op = req.SendWebRequest();
            //UILoading.instance.SetProgress(op, $"{sheet.className} 데이터 로딩중");
            await op;
            var res = req.downloadHandler.text;
            Debug.Log(res);
            sheet.datas = TsvToDic(res);
        }
        ImportDatas();
    }

    List<Dictionary<string, string>> TsvToDic(string data)
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
        var rows = data.Split('\n');
        var keys = rows[0].Trim().Split('\t');
        for (int i = 1; i < rows.Length; i++)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var columns = rows[i].Trim().Split('\t');
            for (int j = 0; j < columns.Length; j++)
            {
                dic.Add(keys[j], columns[j]);
            }
            list.Add(dic);
        }
        return list;
    }

    protected void ImportDatas()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            field.FieldType.GetMethod("SetData")?.Invoke(field.GetValue(this), null);
        }
        IsInit = true;
    }

    public List<T> ImportData<T>()
    {
        var sheet = sheets.Find(obj => obj.className == typeof(T).Name);
        if (sheet != null)
        {
            return GetDatas<T>(sheet.datas);
        }
        return null;
    }

    public List<T> GetDatas<T>(List<Dictionary<string, string>> datas)
    {
        List<T> list = new List<T>();
        foreach (var data in datas)
        {
            list.Add(DicToClass<T>(data));
        }
        return list;
    }



    public T DicToClass<T>(Dictionary<string, string> data)
    {
        var dt = Activator.CreateInstance(typeof(T));
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var keys = new List<string>(data.Keys);
        foreach (var field in fields)
        {
            try
            {
                var idx = keys.FindIndex(obj => obj == field.Name);
                if (idx >= 0)
                {
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(dt, int.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(dt, float.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(dt, bool.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(double))
                    {
                        field.SetValue(dt, double.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(dt, data[keys[idx]]);
                    }
                    else if (field.FieldType == typeof(List<int>))
                    {
                        var datas = data[keys[idx]].Split('|');
                        List<int> list = new List<int>();
                        foreach (var str in datas)
                        {
                            list.Add(int.Parse(str));
                        }
                        field.SetValue(dt, list);
                    }
                    else if (field.FieldType == typeof(int[]))
                    {
                        var datas = data[keys[idx]].Split('|');
                        List<int> list = new List<int>();
                        foreach (var str in datas)
                        {
                            list.Add(int.Parse(str));
                        }
                        field.SetValue(dt, list.ToArray());
                    }
                    else if (field.FieldType == typeof(List<string>))
                    {
                        field.SetValue(dt, data[keys[idx]].Split('|').ToList());
                    }
                    else if (field.FieldType == typeof(string[]))
                    {
                        field.SetValue(dt, data[keys[idx]].Split('|'));
                    }
                    else if (field.FieldType == typeof(Vector3))
                    {
                        field.SetValue(dt, data[keys[idx]].ToVector3());
                    }
                    else
                    {
                        field.SetValue(dt, Enum.Parse(field.FieldType, data[keys[idx]]));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Convert Failed : {ex}");
            }
        }
        return (T)dt;
    }

    public abstract void AddDataDics<T>(List<T> datas) where T : BaseDataSO;

    public void SuccessInit()
    {
        Init();
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class ExtensionMethods
{
    public static string FileName(this string path)
    {
        return path.Split('/').Last().Split('.')[0];
    }

    public static T Last<T>(this T[] array)
    {
        var list = array.ToList<T>();
        return list.Last();
    }

    public static List<T> ToList<T>(this T[] array)
    {
        return new List<T>(array);
    }
#if USE_SO_DATA
    public static void AddRange<T>(this Dictionary<string, BaseDataSO> dic, List<T> datas) where T : BaseDataSO
    {
        foreach (var data in datas)
        {
            if (!dic.ContainsKey(data.rcode)) if (!dic.ContainsKey(data.rcode))
                    dic.Add(data.rcode, data);
        }
    }
#endif
}
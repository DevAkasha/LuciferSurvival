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
    public static Sprite ToSprite(this Texture2D tex)
    {
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
    }

    public static Sprite ToSprite(this Texture tex)
    {
        return ToSprite((Texture2D)tex);
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
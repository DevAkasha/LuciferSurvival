using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ExtensionMethods
{
    public static List<T> ToList<T>(this T[] array)
    {
        return new List<T>(array);
    }

}
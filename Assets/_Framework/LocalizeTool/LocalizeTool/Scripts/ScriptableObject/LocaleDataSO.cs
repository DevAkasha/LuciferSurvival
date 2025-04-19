using Ironcow;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Ironcow.LocalizeTool
{
    [System.Serializable]
    public class LocaleData
    {
        public string Key;
        public string Korean;
        public string English;
    }

    public class LocaleDataSO : SOSingleton<LocaleDataSO>
    {
        public static string languageValue { get => Application.systemLanguage.ToString(); }

        [SerializeField] public List<LocaleData> localeData = new List<LocaleData>();

        private Dictionary<string, string> localeDic = new Dictionary<string, string>();
        public Dictionary<string, string> LocaleDic
        {
            get
            {
                if (localeDic == null) localeDic = new Dictionary<string, string>();
                if (localeDic.Count != localeData.Count)
                {
                    InitLocaleDic();
                }

                return localeDic;
            }
        }

        public void InitLocaleDic()
        {
            localeData.ForEach(obj =>
            {
                var fields = obj.GetType().GetFields().ToList();
                if (!localeDic.ContainsKey(obj.Key))
                    localeDic.Add(obj.Key, (string)fields.Find(obj => obj.Name == Application.systemLanguage.ToString()).GetValue(obj));
                else
                    localeDic[obj.Key] = (string)fields.Find(obj => obj.Name == Application.systemLanguage.ToString()).GetValue(obj);
                //localeDic.Add(obj.Key, (string)fields.Find(obj => obj.Name == "English").GetValue(obj));
            });
        }

        public static string GetString(string key, params object[] param)
        {
            if (Instance.localeDic.Count == 0) Instance.InitLocaleDic();
            if (Instance.localeDic.ContainsKey(key))
            {
                if (param.Length > 0)
                    return string.Format(Instance.localeDic[key], param);
                else
                    return Instance.localeDic[key];
            }
            else
                return key;
        }
    }

    public class Locale
    {
        public static string GetString(string key, params object[] param)
        {
            return LocaleDataSO.GetString(key.Replace("/n", "\n"), param);
        }
    }
}
using Ironcow;
#if USE_LOCALE
using Ironcow.LocalizeTool;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Ironcow.UI
{
    [System.Serializable]
    public class UIOptions
    {
        public bool isActiveOnLoad = true;
        public bool isDestroyOnHide = true;
        public bool isMultiple = false;
    }

#if USE_LOCALE
[System.Serializable]
public class LocaleText
{
    public string key;
    public TMP_Text text;

    public LocaleText(string key, TMP_Text text)
    {
        this.key = key;
        this.text = text;
    }
}
#endif
#if USE_AUTO_CACHING
public abstract class UIBase : MonoAutoCaching
#else
public abstract class UIBase : MonoBehaviour
#endif
#if USE_LOCALE
, ILocale
#endif
    {
        public eUIPosition uiPosition;
        public UIOptions uiOptions;
        public UnityAction<object[]> opened;
        public UnityAction<object[]> closed;
        public RectTransform rectTransform { get => transform as RectTransform; }

        protected virtual void Awake()
        {
            opened = Opened;
            closed = Closed;
#if USE_LOCALE
            SetLocale();
        }

        public void SetLocale()
        { 
            foreach(var text in texts)
            {
                text.text.text = LocaleDataSO.Instance.LocaleDic[text.key];
            }
        }
    
        public List<LocaleText> texts;
        public void SetLocaleTexts()
        {
            texts.Clear();
            var tmpTexts = GetComponentsInChildren<TMPro.TMP_Text>(true).ToList();
            tmpTexts.ForEach(text =>
            {
                var localeData = LocaleDataSO.Instance.localeData.Find(obj => obj.Korean == text.text);
                if(localeData != null)
                {
                    texts.Add(new LocaleText(localeData.Key, text));
                }
            });
#endif
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public abstract void HideDirect();

        public abstract void Opened(object[] param);

        public virtual void Closed(object[] param) { }
    }
}
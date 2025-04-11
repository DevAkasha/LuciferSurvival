using Ironcow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow.Resource;
using UnityEngine.UI;
using System;

namespace Ironcow.UI
{
#if USE_AUTO_CACHING
    public class CanvasBase<T> : MonoSingleton<T>, CanvasOption where T : CanvasBase<T>
#else
    public class CanvasBase : MonoBehaviour, CanvasOption
#endif
    {
        [SerializeField] protected List<Transform> parents;
        [SerializeField] private bool isCreateSafeArea;
        private CanvasScaler scaler;

#if USE_AUTO_CACHING
        protected override void Awake()
        {
            base.Awake();
#else
        void Awake()
        {
#endif
            if (parents.Count > 0) UIManager.SetParents(parents);
            for (int i = 0; i < parents.Count; i++)
            {
                var rectTransform = parents[i] as RectTransform;
                var safeArea = Screen.safeArea;
                var minAnchor = safeArea.position;
                var maxAnchor = minAnchor + safeArea.size;

                minAnchor.x /= Screen.width;
                minAnchor.y /= Screen.height;
                maxAnchor.x /= Screen.width;
                maxAnchor.y /= Screen.height;

                rectTransform.anchorMin = minAnchor;
                rectTransform.anchorMax = maxAnchor;
            }
            if (isCreateSafeArea)
            {
                var ui = Instantiate(ResourceManager.instance.LoadAsset<GameObject>("SafeArea", ResourceType.Prefabs), transform);
            }
        }

#if USE_AUTO_CACHING && UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
#else
        void OnValidate()
        {
#endif
            SetScaler();
            SetParent();
        }

        private void SetScaler()
        {
            if (scaler == null)
            {
                scaler = GetComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        public void SetParent()
        {
            if (parents.Count == 0)
            {
                foreach (var name in Enum.GetNames(typeof(eUIPosition)))
                {
                    var parent = transform.Find(name);
                    if (parent != null && !parents.Contains(parent)) parents.Add(parent);
                }
            }
        }

    }


}
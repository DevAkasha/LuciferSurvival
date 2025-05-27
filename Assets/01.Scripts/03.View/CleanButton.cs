// Copyright (C) 2016 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ricimi
{
    /// <summary>
    /// Fundamental button class used throughout the demo.
    /// </summary>
    public class CleanButton : Button
    {
        private CleanButtonConfig config;
        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            config = GetComponent<CleanButtonConfig>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            UpdateVisualState();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                UpdateVisualState();
            }
        }

        private void UpdateVisualState()
        {
            if (canvasGroup == null) return;

            canvasGroup.alpha = interactable ? 1.0f : 0.4f; // 비활성화 시 투명도 낮춤
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerEnter(eventData);
            StopAllCoroutines();
            StartCoroutine(Utils.FadeOut(canvasGroup, config.onHoverAlpha, config.fadeTime));
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerExit(eventData);
            StopAllCoroutines();
            StartCoroutine(Utils.FadeIn(canvasGroup, 1.0f, config.fadeTime));
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerDown(eventData);
            canvasGroup.alpha = config.onClickAlpha;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable) return;
            base.OnPointerUp(eventData);
            canvasGroup.alpha = 1.0f;
        }
    }
}
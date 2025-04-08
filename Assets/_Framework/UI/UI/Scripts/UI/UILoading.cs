using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;

namespace Ironcow.UI
{
#if USE_ADDRESSABLE
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    public class UILoading : MonoSingleton<UILoading>
    {
        [SerializeField] private Image bg;
        [SerializeField] private Image progressFill;
        [SerializeField] private TMPro.TMP_Text progressText;
        [SerializeField] private TMPro.TMP_Text progressDesc;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public static void Show(Sprite bg = null)
        {
            instance.SetBG(bg);
            instance.gameObject.SetActive(true);
        }

        public void SetBG(Sprite bg = null)
        {
            if (bg != null)
                this.bg.sprite = bg;
        }

        public static void Hide()
        {
            instance.gameObject.SetActive(false);
        }

        public void SetProgress(float progress, string desc = "")
        {
            this.progressDesc.text = desc;
            progressFill.fillAmount = progress;
        }

        public void SetProgress(AsyncOperation op, string desc = "")
        {
            this.progressDesc.text = desc;
            StartCoroutine(Progress(op));
        }

        public IEnumerator Progress(AsyncOperation op)
        {
            while (op.isDone)
            {
                progressFill.fillAmount = op.progress;
                yield return new WaitForEndOfFrame();
            }
            progressFill.fillAmount = 1;
        }

#if USE_ADDRESSABLE
    public void SetProgress(AsyncOperationHandle op, string desc = "")
    {
        this.progressDesc.text = desc;
        StartCoroutine(Progress(op));
    }

    public IEnumerator Progress(AsyncOperationHandle op)
    {
        while (op.IsDone)
        {
                progressFill.fillAmount = op.GetDownloadStatus().Percent;
            yield return new WaitForEndOfFrame();
        }
            progressFill.fillAmount = 1;
    }
#endif
    }
}
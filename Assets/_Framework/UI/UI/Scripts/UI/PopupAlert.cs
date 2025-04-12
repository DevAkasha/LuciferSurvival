using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Ironcow;
using System;
using Ironcow.LocalizeTool;
using Ironcow.UI;

/// <summary>
/// �Ű����� ���� : ����, �޼�������, OKbtn����, Cancelbtn����, OKAction, CancleAction, Image, 1 or 2(�̹����ֻ��or���ϴ�)
/// </summary>
public class PopupAlert : UIBase
{
    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text descText;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private GameObject goCancel;

    [SerializeField]
    private RectTransform rtConfirm;

    [SerializeField]
    private TMP_Text okButtonText;

    [SerializeField]
    private TMP_Text cancelButtonText;

    [SerializeField]
    private Image image;

    private string text { set => descText.text = value; }
    private string title { set => titleText.text = value; }
    private string oktext { set => okButtonText.text = value; }
    private string canceltext { set => cancelButtonText.text = value; }

    private UnityAction okCallback;
    private UnityAction<string> okInputCallback;
    private UnityAction cancelCallback;

    // �Ű����� ���� : ����, �޼�������, OKbtn, Cancelbtn, OKAction, CancleAction, Image, 1 or 2(�̹����ֻ��or���ϴ�)
    public override void Opened(object[] param)
    {
        // �̹� �����ִ°� �Ȱ��� alert�� ��� ��ŵ.
        if (UIManager.IsOpened<PopupAlert>())
        {
            if (descText.text == (string)param[0] && titleText.text == (string)param[1])
            {
                print("�̹� �����ִ� alertâ");
                return;
            }
        }

        float btnHeight = goCancel.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        float height = 0; //rectTransform.sizeDelta.y; // descText.rectTransform.sizeDelta.y

        var title = (string)param[1];
        var desc = (string)param[0];
        var oktext = (string)param[2];
        var canceltext = (string)param[3];
        try
        {
            okCallback = (UnityAction)param[4];
        }
        catch (Exception ex)
        {
            okInputCallback = (UnityAction<string>)param[4];
            inputField.text = "";
            inputField.gameObject.SetActive(true);
        }
        cancelCallback = (UnityAction)param[5];
#if USE_LOCALE
        this.oktext = string.IsNullOrEmpty(oktext) ? LocaleDataSO.GetString("popupButtonOk") : oktext;
        this.canceltext = string.IsNullOrEmpty(canceltext) ? LocaleDataSO.GetString("popupButtonCancel") : canceltext;
#endif
        text = desc.Replace("/n", "\n");
#if USE_LOCALE
        this.title = string.IsNullOrEmpty(title) ? LocaleDataSO.GetString("popupTitle0") : title.Replace("/n", "\n");
#endif

        #region �˾�â ũ�� ����
        height += titleText.transform.parent.GetComponent<RectTransform>().sizeDelta.y;

        goCancel.SetActive(cancelCallback != null);
        if (param.Length > 6 && param[6] != null)
        {
            image.gameObject.SetActive(true);
            Texture2D tex = null;
            Sprite img = null;
            try
            {
                tex = (Texture2D)param[6];
            }
            catch (Exception e)
            {
                img = (Sprite)param[6];
            }
            if (tex != null)
            {
                img = tex.ToSprite();
            }
            image.sprite = img;
            var origin = image.rectTransform.sizeDelta;
            image.SetNativeSize();

            // �⺻ ������ �̹����� Ŭ ��
            if (image.rectTransform.sizeDelta.x >= origin.x || image.rectTransform.sizeDelta.y >= origin.y)
            {
                // ���� ���̴� ������Ű�� ������ŭ ���� ���̸� ���
                var size = image.rectTransform.sizeDelta.y / image.rectTransform.sizeDelta.x;
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, origin.x);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, origin.x * size);
            }
            height += Mathf.Max(100, image.rectTransform.sizeDelta.y);
        }
        else
        {
            image.gameObject.SetActive(false);
        }

        if (height > 1300)
        {
            print("ũ���ִ�ġ");
            height = 1300;
        }
        #endregion
    }

    public void OnClickOk()
    {
        if (okCallback != null)
        {
            okCallback.Invoke();
        }
        else
        {
            okInputCallback?.Invoke(inputField.text);
        }
        UIManager.HideAlert();
    }

    public void OnClickCancel()
    {
        if (cancelCallback != null)
        {
            cancelCallback.Invoke();
        }
        UIManager.HideAlert();
    }

    public override void HideDirect()
    {
        UIManager.HideAlert();
    }
}
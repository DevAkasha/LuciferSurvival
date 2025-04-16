using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircleRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Image image = GetComponent<Image>();
        RectTransform rectTransform = image.rectTransform;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out local);

        float radius = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / 2f;
        return local.magnitude <= radius;
    }
}
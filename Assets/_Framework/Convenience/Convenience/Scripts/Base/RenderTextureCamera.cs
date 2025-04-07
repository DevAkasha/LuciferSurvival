using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;

public class RenderTextureCamera : MonoSingleton<RenderTextureCamera>
{
    [SerializeField] private Camera renderCamera;

    private RenderTexture rt;
    private Transform trackingTarget;

    public void SetRenderTexture(RawImage view, Vector2Int size, bool isOrthographic = true)
    {
        rt = new RenderTexture(size.x, size.y, 0);
        renderCamera.targetTexture = rt;
        renderCamera.orthographic = isOrthographic;
        view.texture = rt;
    }

    public void SetTrackingTarget(Transform trackingTarget)
    {
        this.trackingTarget = trackingTarget;
    }

    private void LateUpdate()
    {
        if (trackingTarget != null)
        {
            transform.position = trackingTarget.position + new Vector3(0, 0, -10);
        }
    }
}
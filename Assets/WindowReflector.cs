using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowReflector : MonoBehaviour
{
    public bool enabled = false;
    public Camera reflectionCamera;
    public RenderTexture currentRenderTexture ;
    public Material glassMaterial;

    public MeshRenderer renderer1;
    public MeshRenderer renderer2;

    private Material newGlassMaterial;

    private void Start()
    {
        currentRenderTexture = new RenderTexture(512, 512, 16);
        if (enabled)
        {
            currentRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            reflectionCamera.targetTexture = currentRenderTexture;
        }
        else
        {
            reflectionCamera.enabled = false;
        }
        renderer1.material.SetTexture("BaseColorMap", currentRenderTexture);
        renderer2.material.SetTexture("BaseColorMap", currentRenderTexture);
        renderer1.material.mainTexture = currentRenderTexture;
        renderer2.material.mainTexture = currentRenderTexture;
    }
}

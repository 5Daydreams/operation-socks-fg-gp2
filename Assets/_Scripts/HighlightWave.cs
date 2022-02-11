using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class HighlightWave : MonoBehaviour
{
    [SerializeField] private MeshRenderer objectRenderer;
    [SerializeField] private Texture2D emissionTexture;
    [SerializeField] private float waveValue;
    private MaterialPropertyBlock mpb;
    
    void Update()
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        for (int x = 0; x < emissionTexture.width; x++)
        {
            for (int y = 0; y < emissionTexture.height; y++)
            {
                // float pixColor = Mathf.Clamp(waveValue, 0, 1);
                // emissionTexture.SetPixel(x,y, new Color(pixColor,pixColor,pixColor,pixColor));
            }
        }
        
        
        mpb.SetFloat("Emission", 1);
        mpb.SetTexture("_EmissionMap", emissionTexture);
        mpb.SetColor("_EmissionColor", Color.white);

        objectRenderer.SetPropertyBlock(mpb);
    }
}

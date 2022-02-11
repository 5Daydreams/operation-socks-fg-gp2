using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.ProBuilder.Shapes;

public class ConeViewDepthReader : MonoBehaviour
{
    public MeshRenderer ConeRenderer;
    [HideInInspector] public float SightAngle;
    [HideInInspector] public float MaxDistance;

    private const int BUFFER_SIZE = 256;
    private Quaternion m_Rotation;
    private float[] m_aDepthBuffer;
    private MaterialPropertyBlock mpb;

    void Start()
    {
        m_aDepthBuffer = new float[BUFFER_SIZE];
    }

    public void ApplyChanges()
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }
        
        mpb.SetFloat("_SightAngle", SightAngle);
        
        ConeRenderer.SetPropertyBlock(mpb);
        ConeRenderer.gameObject.transform.localScale = Vector3.one * (2* MaxDistance);
    }

    void FixedUpdate()
    {
        m_Rotation = this.transform.rotation;
        UpdateViewDepthBuffer();
    }

    void UpdateViewDepthBuffer()
    {
        float anglestep = SightAngle / BUFFER_SIZE;
        float viewangle = m_Rotation.eulerAngles.y;
        int bufferindex = 0;

        for (int i = 0; i < BUFFER_SIZE; i++)
        {
            float angle = anglestep * i + (viewangle - SightAngle / 2);


            Vector3 dest = GetVector(-angle * Mathf.PI / 180, MaxDistance);
            Ray r = new Ray(this.transform.position, dest);

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(r, out hit))
            {
                m_aDepthBuffer[bufferindex] = (hit.distance / MaxDistance);
            }
            else
            {
                m_aDepthBuffer[bufferindex] = -1;
            }

            bufferindex++;
        }

        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        mpb.SetFloatArray("_SightDepthBuffer", m_aDepthBuffer);

        ConeRenderer.SetPropertyBlock(mpb);
    }

    Vector3 GetVector(float angle, float dist)
    {
        float x = Mathf.Cos(angle) * dist;
        float z = Mathf.Sin(angle) * dist;
        return new Vector3(x, 0, z);
    }
}
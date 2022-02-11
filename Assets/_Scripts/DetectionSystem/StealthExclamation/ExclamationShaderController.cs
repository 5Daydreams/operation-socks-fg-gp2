using UnityEngine;

public class ExclamationShaderController : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    // [Range(0,1)] [SerializeField] private float debugValue;
    private float exclamationFill;
    private MaterialPropertyBlock propertyBlock;


#if  UNITY_EDITOR
    // This is just a debug method, and can be deleted prior to shipping
    // private void OnValidate()
    // {
    //     if (propertyBlock == null)
    //         propertyBlock = new MaterialPropertyBlock();
    //
    //     SetExclamationValue(debugValue);
    //     Update();
    // }
#endif

    private void Awake()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    public void SetExclamationValue(float value)
    {
        exclamationFill = value;
    }

    private void Update()
    {
        // With a property block, set the float value to the corresponding string-id for the property block  

        this.transform.rotation = Quaternion.identity;

        propertyBlock.SetFloat("_CircleFill", exclamationFill);

        meshRenderer.SetPropertyBlock(propertyBlock);
    }
}
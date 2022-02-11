Shader "Unlit/ExclamationVFX"
{
    Properties
    {
        _CircleTex ("Circle Texture", 2D) = "white" {}
        _StickTex ("Stick Texture", 2D) = "white" {}
        
        _CircleFill ("Circle fill", Range(0,1)) = 0
        _CircleZeroAlphaThreshold ("Circle Min Alpha Threshold", Range(0.0,0.5)) = 0.1
        _CircleMaxAlphaThreshold ("Circle Max Threshold", Range(0.0,0.5)) = 0.1
        _ExclamationThreshold ("Exclamation Popup Threshold", Range(0.5,1)) = 0.5
       
        _BaseTint ("Base Tint", Color) = (1,1,1,1)
        _FillTint ("Fill Tint", Color) = (1,1,0,1)
        _HighAlertColor ("High Alert Tint", Color) = (1,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Zwrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CircleTex;
            sampler2D _StickTex;
            float4 _CircleTex_ST;
            float4 _StickTex_ST;
            float4 _BaseTint;
            float4 _FillTint;
            float4 _HighAlertColor;
            float _CircleFill;
            float _MaxFillRadius;
            float _CircleZeroAlphaThreshold;
            float _CircleMaxAlphaThreshold;
            float _ExclamationThreshold;
            float _VerticalOffset;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _CircleTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fillUvCentered = i.uv.xy - float2(0.5,0.24); // Custom for the texture provided by Erik
                float distToCircleCenter = length(fillUvCentered);

                float maxRadius = 0.137 * _CircleFill; // also custom for the provided texture
                float LerpValue = step(maxRadius,distToCircleCenter);
                
                float exclamationAlpha = saturate((_CircleFill-_ExclamationThreshold)/(1.0f-_ExclamationThreshold));

                float initialAlphaLerp = saturate((_CircleFill-_CircleZeroAlphaThreshold)/(_CircleMaxAlphaThreshold-_CircleZeroAlphaThreshold));
                
                float circleAlpha = lerp(0,1,initialAlphaLerp);
                
                fixed4 colBase = tex2D(_CircleTex, i.uv) * _BaseTint; 

                float alertColorLerpValue = (1 + 5.0*exclamationAlpha ) * (1.0/6.0);
                
                float4 alertColor = lerp(_FillTint,_HighAlertColor,alertColorLerpValue);
                
                fixed4 colFill = tex2D(_CircleTex, i.uv) * alertColor;

                float4 fillResult = lerp(colFill,colBase,LerpValue) * circleAlpha;
                
                float4 stick = tex2D(_StickTex, i.uv) * exclamationAlpha;
                
                stick *= alertColor;
                
                return fillResult + stick;
            }
            ENDCG
        }
    }
}

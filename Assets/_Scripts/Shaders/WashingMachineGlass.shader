Shader "Unlit/WashingMachineGlass"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "black" {}
        _GlassRadius ("Glass Radius", Float) = 1.0
        _Color1 ("Color 1", Color) = (0.0,0.0,0.0,0.0)
        _Color2 ("Color 2", Color) = (1.0,1.0,1.0,1.0)
        _SwirlIntensity ("Swirl Intensity", Float) = 1.0
        _SwirlSpeed ("Swirl Speed", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
            
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _GlassRadius;
            float _SwirlIntensity;
            float _SwirlSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 centeredUV = i.uv - 0.5;
                float uvRadius = length(centeredUV);
                float uvPointAngle = atan2(centeredUV.y,centeredUV.x);
                float angularDisplacement =  - _SwirlIntensity * UNITY_TWO_PI * (uvRadius/_GlassRadius) + uvPointAngle;
                
                float2 spiraledUV = uvRadius * float2(cos(angularDisplacement+_Time.y * _SwirlSpeed), sin(angularDisplacement+_Time.y * _SwirlSpeed));
                
                fixed4 noise = tex2D(_NoiseTex, spiraledUV - 0.5);
                
                float lerpValue = step(uvRadius, _GlassRadius);

                float4 colorValue = lerp(_Color1,_Color2, noise); 
                
                return lerp(0, colorValue, lerpValue);
            }
            ENDCG
        }
    }
}
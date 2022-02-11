Shader "CustomShaders/ConeView"
{
    Properties
    {
        _Color("Outer Color",Color) = (1,1,1,1)
        _CenterColor("Center Color",Color) = (1,1,1,1)
        _SightAngle("SightAngle",Range(0,180)) = 0.5
        _RangeStep("Opacity",Range(0,1)) = 0.7
        _SourceWhiteness("Center Blend",Range(0,1)) = 1
        _AngleAlphaBoost("Angle Alpha Boost",Range(1,10)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite Off

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
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _CenterColor;
            float _SightAngle;
            float _RangeStep;
            float _SourceWhiteness;
            float _AngleAlphaBoost;

            uniform float _SightDepthBuffer[256];

            v2f vert(appdata IN)
            {
                v2f o;
                o.position = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.uv;
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                const float PI = 3.14159;

                IN.uv.x -= 0.5f;
                IN.uv.y -= 0.5f;
                half distcenter = 1 - sqrt(IN.uv.x * IN.uv.x + IN.uv.y * IN.uv.y) * 2;

                half2 fragmentDir = normalize(IN.uv.xy);

                half sightAngleRads = _SightAngle / 2 * PI / 180;
                half sightVal = cos(sightAngleRads);

                float viewDotPos = clamp(dot(float2(1, 0), fragmentDir), 0, 1);

                float4 col = lerp(_Color, _CenterColor, distcenter * _SourceWhiteness);

                // the functional code:
                // col.a *= _RangeStep * step(sightVal,viewDotPos) * step(1-distcenter,1) * lerp(sightVal,1,viewDotPos);

                // my smoothing test 
                // col.a *= _RangeStep * step(1-distcenter,1) * saturate(viewDotPos - sightVal);

                // my final smoothing test
                col.a *= _RangeStep * saturate(1 / distcenter) * saturate((viewDotPos - sightVal) * _AngleAlphaBoost);

                if (viewDotPos < sightVal)
                {
                    col.a *= _RangeStep;
                }
                else
                {
                    // --- Depth check
                    float fragmentAngle = atan2(fragmentDir.y, fragmentDir.x) + sightAngleRads;
                    float fragmentVal = 1.0f - (fragmentAngle) / (sightAngleRads * 2);
                    int index = fragmentVal * 256;

                    if (_SightDepthBuffer[index] > 0 && (1 - distcenter) > _SightDepthBuffer[index])
                        col = 0;
                }

                col.a *= _Color.a;

                // float pulse = 0.85 + 0.15f * sin(5 * _Time.z);
                //
                // col *= pulse;

                return col;
            }
            ENDCG
        }
    }

}
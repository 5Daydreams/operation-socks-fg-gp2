Shader "Custom/SpriteWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("BaseColor", Color) = (1,0,0,0)
        _WaveColor ("WaveColor", Color) = (1,1,1,0)
        _WaveValue ("Wave Value", float) = 0
        _WaveTrail ("Wave Trail", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
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

            float4 _BaseColor;
            float4 _WaveColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveValue;
            float _WaveTrail;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv.xy);
                float value = i.uv.x + i.uv.y;

                _WaveValue = frac(_Time.y)*5; 
                
                float waveTrail = smoothstep(_WaveValue - _WaveTrail, _WaveValue, value);
                float waveFront = step(value, _WaveValue);
                float wave = waveFront * waveTrail;
                
                float4 color = lerp(_BaseColor,_WaveColor, wave);
                
                return float4(color);
            }
            ENDCG
        }
    }
}
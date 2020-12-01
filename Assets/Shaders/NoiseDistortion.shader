Shader "Custom/NoiseDistortion"
{
    Properties
    {
        _NoiseIntensityMap ("NoiseIntensityMap", 2D) = "white" {}
        _NoiseSize ("NoiseSize", Range(0, 1)) = 0.1
        _NoiseSpeed ("NoiseSpeed", float) = 1
        _NoiseIntensity ("NoiseIntensity", float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        GrabPass { }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            // Random directional vector
            float2 randomGradient(int2 i)
            {
                float random = 2723 * sin(i.x * 35411 + i.y * 15422 + 7512) * cos(i.x * 12315 * i.y * 333571 + 1821);
                return float2(cos(random), sin(random));
            }

            // Dot product of the distance and gradient vectors
            float dotGridGradient(int2 i, float2 r)
            {
                float2 gradient = randomGradient(i);

                float2 d = float2(0, 0);
                d.x = r.x - i.x;
                d.y = r.y - i.y;

                return dot(gradient, d);
            }

            float perlin(float2 r)
            {
                int2 i0 = int2((int)r.x, (int)r.y);
                int2 i1 = int2(i0.x + 1, i0.y + 1);

                float sx = r.x - i0.x;
                float sy = r.y - i0.y;

                float n0, n1, ix0, ix1;

                n0 = dotGridGradient(i0, r);
                n1 = dotGridGradient(int2(i1.x, i0.y), r);
                ix0 = lerp(n0, n1, sx);

                n0 = dotGridGradient(int2(i0.x, i1.y), r);
                n1 = dotGridGradient(i1, r);
                ix1 = lerp(n0, n1, sx);

                return lerp(ix0, ix1, sy);
            }

            sampler2D _NoiseIntensityMap;
            float4 _NoiseIntensityMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseIntensityMap);
                o.grabUV = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            float _NoiseSize;
            float _NoiseSpeed;
            float _NoiseIntensity;

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = perlin(i.grabUV.xy / _NoiseSize + float2(0, _Time.y * _NoiseSpeed)) * _Time.x;
                float3 noiseIntensityTexture = tex2D(_NoiseIntensityMap, i.uv).rgb;
                float noiseIntensity = dot(noiseIntensityTexture, float3(1, 1, 1)) / 3;
                i.grabUV.xy += sin(float2(noise, noise)) * _NoiseIntensity * noiseIntensity * 0.1;

                return tex2Dproj(_GrabTexture, i.grabUV);
            }
            ENDCG
        }
    }
}

Shader "Custom/CircleWaveDistortion"
{
    Properties
    {
        [Header(Algorithms)]
        [Header(t is the distance from the inner radius)]
        [Header(ir equals Radius times InnerRadiusRatio is the inner radius)]
        [Header(rg equals Radius sub ir)]
        [Header(tn equals t div rg)]
        [Space]
        [Header(Polynomial)]
        [Header(texture offset equals tn to the Distortion plus tn to the Distortion2)]
        [Space]
        [Header(Exponential)]
        [Header(texture offset equals Distortion to the (tn times Distortion2))]
        [Enum(Polynomial, 0, Exponential, 1)] _Algorithm ("Algorithm", int) = 0
        [Space]
        [Space]
        _Distortion ("Distortion", float) = 2
        _Distortion2 ("Distortion2", float) = 1
        [Space]
        [Header(The center of the distortion in world space. Only uses first two values as x and y.)]
        _WorldCenter ("World Center", Vector) = (0, 0, 0, 0)
        [Header(The radius of the distortion in world space.)]
        _Radius ("Radius", float) = 1
        _InnerRadiusRatio ("Inner Radius Ratio", Range(0.5, 1)) = 0.9
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
                float4 vertex : POSITION0;
            };

            struct v2f
            {
                float4 clipPos : POSITION1;
                float4 worldPos : POSITION2;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipPos = UnityWorldToClipPos(o.worldPos);

                return o;
            }

            float _Radius;
            float _InnerRadiusRatio;
            float2 _WorldCenter;
            float _Distortion;
            float _Distortion2;
            int _Algorithm;

            float distFunc (float t)
            {
                if(_Algorithm == 0)
                {
                    if (t < 0.5)
                    {
                        return 0.25 * (pow(t * 2, _Distortion) + pow(t * 2, _Distortion2));
                    }
                    else
                    {
                        return 1 - 0.25 * (pow((1 - t) * 2, _Distortion) + pow((1 - t) * 2, _Distortion2));
                    }
                }
                if(_Algorithm == 1)
                {
                    if (t < 0.5)
                    {
                        return 0.5 * (pow(_Distortion, t * _Distortion2) - 1) / (pow(_Distortion, 0.5 * _Distortion2) - 1);
                    }
                    else
                    {
                        return 1 - 0.5 * (pow(_Distortion, (1 - t) * _Distortion2) - 1) / (pow(_Distortion, 0.5 * _Distortion2) - 1);
                    }
                }
                return t;
            }

            float innerRadius;
            float range;
            int cnt = 0;

            fixed4 frag (v2f i) : SV_Target
            {
                if(cnt == 0)
                {
                    cnt = 1;
                    innerRadius = _Radius * _InnerRadiusRatio;
                    range = _Radius - innerRadius;
                }

                float2 r = i.worldPos.xy - _WorldCenter;
                float len = length(r);
                float distFromCenter = abs(len - innerRadius);

                if(distFromCenter < range)
                {
                    r = r / len;
                    if(len < innerRadius)
                    {
                        r = r * (innerRadius - range * distFunc(distFromCenter / range));
                    }
                    else
                    {
                        r = r * (innerRadius + range * distFunc(distFromCenter / range));
                    }
                }

                float4 grabUV = ComputeGrabScreenPos(UnityWorldToClipPos(float3(_WorldCenter + r, i.worldPos.z)));

                return tex2Dproj(_GrabTexture, grabUV);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}

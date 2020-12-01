Shader "Custom/CircleLensDistortion"
{
    Properties
    {
        _Distortion ("Distortion", float) = 2
        _WorldCenter ("World Center", Vector) = (0, 0, 0, 0)
        _Radius ("Radius", float) = 1
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
            float2 _WorldCenter;
            float _Distortion;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 r = i.worldPos.xy - _WorldCenter;
                float len = length(r);

                if(len < _Radius)
                {
                    r = r / len;
                    r = r * _Radius * pow(len / _Radius, _Distortion);
                }

                float4 grabUV = ComputeGrabScreenPos(UnityWorldToClipPos(float3(_WorldCenter + r, i.worldPos.z)));

                return tex2Dproj(_GrabTexture, grabUV);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}

Shader "Custom/Grayscale"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            };

            struct v2f
            {
                fixed4 grabUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabUV = ComputeGrabScreenPos(o.vertex);
                return o;
            }
            
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 brgb = tex2Dproj(_GrabTexture, i.grabUV);
                return ((brgb.r + brgb.g + brgb.b) / 3) * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

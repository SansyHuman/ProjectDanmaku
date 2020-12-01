Shader "Custom/Contrast"
{
    Properties
    {
        [PowerSlider(5.0)] _Contrast ("Contrast", Range(0, 100)) = 1
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
                float4 grabUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabUV = ComputeGrabScreenPos(o.vertex);
                return o;
            }
            
            fixed _Contrast;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2Dproj(_GrabTexture, i.grabUV);
                color.rgb = ((color.rgb - 0.5) * max(_Contrast, 0)) + 0.5;

                return color;
            }
            ENDCG
        }
    }
}

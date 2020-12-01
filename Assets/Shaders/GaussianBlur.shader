Shader "Custom/GaussianBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0, 1)) = 0.05
        _Stdev ("Standard Deviation", Range(0, 0.1)) = 0.02
        [IntRange] _Samples ("Samples", Range(2, 100)) = 5
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

            #define PI 3.14159265359
            #define E 2.71828182846

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

            float _BlurSize;
            float _Stdev;
            float _Samples;

            fixed4 frag (v2f i) : SV_Target
            {
                if(_Stdev == 0)
                {
                    return tex2Dproj(_GrabTexture, i.grabUV);
                }

                float4 col = 0;
                float sum = 0;

                for(float x = 0; x < _Samples; x++)
                {
                    for(float y = 0; y < _Samples; y++)
                    {
                        float xOff = (x / (_Samples - 1) - 0.5) * _BlurSize;
                        float yOff = (y / (_Samples - 1) - 0.5) * _BlurSize;
                        float4 grabUV = i.grabUV + float4(xOff, yOff, 0, 0);

                        float sqrStdev = _Stdev * _Stdev;
                        float dist = xOff * xOff + yOff * yOff;

                        float gauss = pow(E, -dist / (2 * sqrStdev)) / (2 * PI * sqrStdev);

                        sum += gauss;
                        col += tex2Dproj(_GrabTexture, grabUV) * gauss;
                    }
                }

                col /= sum;
                return col;
            }
            ENDCG
        }
    }
}

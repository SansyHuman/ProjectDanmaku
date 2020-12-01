Shader "Custom/HSV"
{
    Properties
    {
        _HMul ("Hue Multiplier", Range(0, 10)) = 1
        _HOff ("Hue Offset", Range(-1, 1)) = 0
        _SMul ("Saturation Multiplier", Range(0, 10)) = 1
        _SOff ("Saturation Offset", Range(-1, 1)) = 0
        _VMul ("Value Multiplier", Range(0, 10)) = 1
        _VOff ("Value Offset", Range(-1, 1)) = 0
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

            fixed3 hueToRgb (fixed hue)
            {
                hue = frac(hue);
                return saturate(fixed3(abs(hue * 6 - 3) - 1, 2 - abs(hue * 6 - 2), 2 - abs(hue * 6 - 4)));
            }

            fixed3 hsvToRgb(fixed3 hsv)
            {
                fixed3 rgb = hueToRgb(hsv.x);
                rgb = lerp(1, rgb, hsv.y);
                rgb = rgb * hsv.z;
                return rgb;
            }

            fixed3 rgbToHsv(fixed3 rgb)
            {
                fixed maxC = max(rgb.r, max(rgb.g, rgb.b));
                fixed minC = min(rgb.r, min(rgb.g, rgb.b));
                fixed d = maxC - minC;
                fixed hue = 0;

                if(maxC == rgb.r)
                {
                    hue = (rgb.g - rgb.b) / d;
                }
                else if(maxC == rgb.g)
                {
                    hue = 2 + (rgb.b - rgb.r) / d;
                }
                else if(maxC == rgb.b)
                {
                    hue = 4 + (rgb.r - rgb.g) / d;
                }
                hue = frac(hue / 6);
                
                fixed sat = d / maxC;
                fixed val = maxC;

                return fixed3(hue, sat, val);
            }

            fixed _HMul;
            fixed _HOff;
            fixed _SMul;
            fixed _SOff;
            fixed _VMul;
            fixed _VOff;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2Dproj(_GrabTexture, i.grabUV);
                fixed3 hsv = rgbToHsv(col.rgb);

                hsv.x = hsv.x * _HMul + _HOff;
                hsv.y = hsv.y * _SMul + _SOff;
                hsv.z = hsv.z * _VMul + _VOff;
                hsv = saturate(hsv);

                return fixed4(hsvToRgb(hsv), col.a);
            }
            ENDCG
        }
    }
}

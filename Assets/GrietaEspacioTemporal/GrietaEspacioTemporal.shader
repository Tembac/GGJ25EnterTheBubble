Shader "Paco/GrietaEspacioTemporal"
{
    Properties
    {
        [NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
        _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0, 360)) = 0

        [PowerSlider(4)] _FresnelExponent ("Fresnel Exponent", Range(1, 16)) = 1
        _FresnelBias ("Fresnel Bias", Range(1, 4)) = 1

        [Toggle(USAR_DEBUG)] _UsarDebug("Ver Debug", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        // Cull Off
        // Cull Back
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
		    #pragma shader_feature USAR_DEBUG

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;
                float3 worldSpaceNormal : NORMAL0;
                float3 worldSpaceViewDir : NORMAL1;
            };

            samplerCUBE _Tex;
            half4 _Tex_HDR;
            half4 _Tint;
            half _Exposure;
            float _Rotation;
            float _FresnelExponent;
            float _FresnelBias;

            float3 RotateAroundYInDegrees (float3 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldSpaceNormal = UnityObjectToWorldNormal(v.normal);
                o.worldSpaceViewDir = WorldSpaceViewDir(v.vertex);
                o.uv = RotateAroundYInDegrees(-o.worldSpaceViewDir, _Rotation);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnel = (pow((dot(i.worldSpaceNormal, normalize(-i.worldSpaceViewDir)))*_FresnelBias,_FresnelExponent));

                //fresnel = max( step(0.001, -fresnel), fresnel);

                half4 tex = texCUBE (_Tex, i.uv);
                half3 c = DecodeHDR (tex, _Tex_HDR);
                c = c * _Tint * unity_ColorSpaceDouble.rgb;
                c *= _Exposure;

                // float val = step(0.1,i.vertex.z/i.vertex.w);
                // c = lerp(c,half3(1,0,0),val);
                // fresnel = lerp(fresnel,1,val);

                #if USAR_DEBUG
                float menosQueZero = step(-0.001,-fresnel);
                float masQueUno = step(1.0,fresnel);
                return half4(menosQueZero,masQueUno,fresnel, 1.0);
                #else
                return half4(c,fresnel);
                #endif
            }
            ENDCG
        }
    }
}

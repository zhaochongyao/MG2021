Shader "ZaoCha/CartoonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexCol("TexCol",color) = (1.0,1.0,1.0,1.0)
        _brightNum ("brightNum",float)= 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags 
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
			#include "Lighting.cginc"
			#pragma multi_compile_fwdbase_fullshadows

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                LIGHTING_COORDS(1, 2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TexCol;
            float _brightNum;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float shadow = LIGHT_ATTENUATION(i);
                float4 finalCol = col * shadow * _TexCol * _brightNum;
                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse" /////这一句很重要妈的为什么
}

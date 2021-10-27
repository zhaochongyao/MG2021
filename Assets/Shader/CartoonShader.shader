Shader "ZaoCha/CartoonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexCol("TexCol",color) = (1.0,1.0,1.0,1.0)
        _brightNum ("brightNum",float)= 2.0
        _Steps("ColorStep",float)= 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                LIGHTING_COORDS(1, 2)
                float3 normal : TEXCOORD3;
                //float3 viewDir : TEXCOORD4;
                float3 lightDir : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TexCol;
            float _brightNum;
            float _Steps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                //o.viewDir = ObjSpaceViewDir(v.vertex);
                o.lightDir = _WorldSpaceLightPos0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 nDirWS = normalize(i.normal);
			    float3 lDirWS = normalize(i.lightDir);
                float dist = length(i.lightDir);//具体光源距离
                float nDotl = max(0,dot(nDirWS, lDirWS));
                float halflamber = nDotl*0.5+0.5;
                float atten = 1/(dist);
                float toon = floor(halflamber * _Steps * atten) / _Steps;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float shadow = LIGHT_ATTENUATION(i);
                float4 finalCol = col * shadow * _TexCol * _brightNum * toon;
                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse" /////这一句很重要妈的为什么
}

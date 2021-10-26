Shader "LangChao/Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distance("RGBOffset",float) = 0.02
        _RGBSpeed("RGBSpeed",float) = 50
        _ScanInt("JitterOffset",float) = 0.02
        _ScanDis("JitterDis",float) = 0.3
        _u_scan("JitterInt",float) = 1
        _Time01("Time01",Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" "IsEmissive" = "true"}
        //Blend One OneMinusSrcAlpha
        Cull off
        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float _Distance;
            uniform float _RGBSpeed;
            uniform float _ScanInt;
            uniform float _ScanDis;
            uniform float _u_scan;
            uniform float _Time01;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            //随机函数
            float randomNoise(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            }
            fixed4 frag (v2f i) : SV_Target
            {

                //扫描射线
                float jitter = randomNoise(i.uv.y,_Time.y * _ScanDis * 100) * 2 - 1;
                float scan = clamp (1.0 - _u_scan * 1.2, 0.0, 1.0);
                jitter *= step (scan, abs (jitter)) ;
                float2 offset2 = frac(float2(i.uv.x + jitter,0.0));//y方向相同的点偏移是一致的
                float4 color1 = tex2D(_MainTex, i.uv + offset2 * _ScanInt);

                //色彩分离
                float2 offset = float2(sin(_Time.y * _RGBSpeed) * _Distance,0.0);
                //fixed4 col1 = tex2D(_MainTex, i.uv);
                //fixed4 col2 = tex2D(_MainTex, i.uv + offset);
                fixed4 color2 = tex2D(_MainTex, i.uv + offset2 * _ScanInt + offset);
                float4 finalCol = float4(color1.r,color1.g,color2.b,1.0);
                finalCol.a = 1-_Time01*0.5;
                return finalCol;
            }
            ENDCG
        }
    }
}

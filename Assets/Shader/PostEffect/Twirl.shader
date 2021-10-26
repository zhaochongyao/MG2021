Shader "Unlit/Twirl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RotSpeed("RotSpeed", Float) = 0
        _AlphaSpeed("AlphaSpeed", Float) = 0
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
            uniform float _RotSpeed;
            uniform float _AlphaSpeed;
            uniform float _Time01;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
               
                float2 center = float2(0.5,0.5);
                //计算距离
                float2 dt = uv - center;
                float len = sqrt(dot(dt, dt));


                //根据时间旋转角度变大
                float RotScal = _Time01 * _RotSpeed;
                //根据距离 计算出旋转角
                float theta = -len * RotScal;

                //旋转矩阵
                float2x2 rot =
                {
                    cos(theta), sin(theta),
                    -sin(theta) ,cos(theta)
                };
                dt = mul(rot, dt);
                uv = dt + center;
                
  
                fixed4 col = tex2D(_MainTex, uv);

                //根据时间淡出
                col.a = 1-RotScal*_AlphaSpeed;
                
                return col;
            }
            ENDCG
        }
    }
}

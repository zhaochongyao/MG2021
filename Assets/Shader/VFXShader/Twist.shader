Shader  "LangChao/Twist"
{
     Properties
     {
         //_MainTex ( "Texture" , 2D) =  "white"  {}
         _RotScal( "RotScal" , float ) = 0
         _Alpha("Alpha",float) =1
     }
     SubShader
     {
         Tags {  "RenderType" = "Opaque"  "Queue" = "Transparent"}
         LOD 100
         GrabPass
         {
            "_GarbTex"
         }
 
         Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag           
             #include "UnityCG.cginc"
 
             struct  appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
             };
 
             struct  v2f
             {
                 float2 uv : TEXCOORD0;
                 float4 vertex : SV_POSITION;
             };
 
             sampler2D _MainTex;
             sampler2D _GarbTex;
             float  _RotScal;
             float _Alpha;
             
             v2f vert (appdata v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv = v.uv;
                 return  o;
             }
             
             fixed4 frag (v2f i) : SV_Target
             {              
                 float2 uv = i.uv;
                 float2 center = float2(0.5,0.5);

                //uv 扭曲代码 
                if (_RotScal > 0)
                {
                    //计算距离
                    float2 dt = uv - center;
                    float len = sqrt(dot(dt, dt));

                    //根据距离 计算出旋转角
                    float theta = -len * _RotScal;

                    //旋转矩阵
                    float2x2 rot =
                    {
                        cos(theta), sin(theta),
                        -sin(theta) ,cos(theta)
                    };
                    dt = mul(rot, dt);
                    uv = dt + center;
                }
         
                fixed4 col = tex2D(_GarbTex, uv);
                return float4(col.rgb,_Alpha);
             }
             ENDCG
         }
     }
}
Shader "DopeElections/Cloud Layer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha One

        Pass
        {


            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewT : TEXCOORD1; //you don't need these semantics except for XBox360
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.viewT = normalize(ObjSpaceViewDir(v.vertex)); //ObjSpaceViewDir is similar, but localspace.
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 viewDir = i.viewT;
                float incoming = dot(viewDir, i.normal);
                float x = 1 - max(incoming, 0) * max(incoming, 0);
                clip(incoming - 0.1);
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(x, 0.5)) * _Brightness;
                return col;
            }
            ENDCG
        }
    }
}
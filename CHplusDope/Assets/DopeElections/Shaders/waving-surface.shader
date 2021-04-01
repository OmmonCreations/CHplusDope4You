// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WavingSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
        _Scale ("Scale", Float) = 1
        _Wave ("Wave", Float) = 1
        _Direction ("Direction", Vector) = (0,1,0)
        _Tess ("Tessellation", Range(1,32)) = 4
        _TimeScale ("Time Scale", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert tessellate:tessFixed addshadow fullforwardshadows nolightmap
        #pragma target 4.6
        struct Input
        {
            float2 uv_MainTex;
        };

        float _Tess;
        
        float4 tessFixed()
        {
            return _Tess;
        }

        float _Scale;
        float _Wave;
        float _Amount;
        float _TimeScale;
        float3 _Direction;

        void vert(inout appdata_full v)
        {
            float3 objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
            float x = sin(v.texcoord.x * _Scale + length(objectPos.xz) * _Wave + _Time.z * _TimeScale);
            //float wave = sin(sqrt(worldPos.x * worldPos.x + worldPos.y * worldPos.y));
            float wave = (x * 2 - 1) * _Amount * v.color.r;
            v.vertex.xyz += _Direction * wave;
        }

        sampler2D _MainTex;

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
Shader "Unlit/Diffusion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _DiffuseWrap ("Diffuse Wrap", Range(0, 1)) = 0.5
        _Alpha ("Transparency", Range(0, 1)) = 1.0 // 1 = Visible, 0 = Invisible
    }

    SubShader
    {
        // Change Tags for Transparency
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        
        // Standard Alpha Blending
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BaseColor;
                float _DiffuseWrap;
                float _Alpha;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                Light light = GetMainLight();
                float NdotL = dot(normalize(IN.normalWS), light.direction);

                // Diffusion logic
                float diffuse = NdotL * _DiffuseWrap + (1.0 - _DiffuseWrap);
                diffuse = max(0, diffuse * diffuse); 

                half4 texColor = tex2D(_MainTex, IN.uv);
                
                // Calculate final RGB
                half3 finalRGB = texColor.rgb * _BaseColor.rgb * (diffuse * light.color);
                
                // Combine texture alpha, color alpha, and our master slider
                half finalAlpha = texColor.a * _BaseColor.a * _Alpha;

                return half4(finalRGB, finalAlpha);
            }
            ENDHLSL
        }
    }
}
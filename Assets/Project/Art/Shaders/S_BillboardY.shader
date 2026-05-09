Shader "RedVeil/BillboardY"
{
    Properties
    {
        [MainTexture] _BaseMap ("Sprite", 2D) = "white" {}
        [MainColor] _BaseColor ("Tint", Color) = (1, 1, 1, 1)
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [Toggle(_FLIP_X)] _FlipX ("Flip Horizontally", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
            "RenderPipeline" = "UniversalPipeline"
            "DisableBatching" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZWrite On
            AlphaToMask Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _FLIP_X

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Cutoff;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 오브젝트의 월드 위치 (피벗)
                float3 worldPivot = TransformObjectToWorld(float3(0, 0, 0));

                // 카메라 위치
                float3 cameraPos = _WorldSpaceCameraPos;

                // Y축만 카메라를 향하도록 회전 방향 계산
                float3 toCamera = cameraPos - worldPivot;
                toCamera.y = 0; // Y축 고정
                toCamera = normalize(toCamera);

                // 빌보드의 right, up 벡터 구성
                float3 worldUp = float3(0, 1, 0);
                float3 worldRight = normalize(cross(worldUp, toCamera));

                // 오브젝트 로컬 공간의 정점을 빌보드 평면으로 매핑
                // 로컬 X → worldRight, 로컬 Y → worldUp
                float scaleX = length(unity_ObjectToWorld._m00_m10_m20);
                float scaleY = length(unity_ObjectToWorld._m01_m11_m21);

                float3 worldPos = worldPivot
                    + worldRight * IN.positionOS.x * scaleX
                    + worldUp * IN.positionOS.y * scaleY;

                OUT.positionHCS = TransformWorldToHClip(worldPos);

                // UV 처리 (수평 뒤집기 옵션)
                float2 uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                #ifdef _FLIP_X
                    uv.x = 1.0 - uv.x;
                #endif
                OUT.uv = uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                // Alpha cutoff (도트 게임용)
                clip(col.a - _Cutoff);

                return col;
            }
            ENDHLSL
        }

        // 그림자 캐스팅용 패스
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex shadowVert
            #pragma fragment shadowFrag
            #pragma shader_feature_local _FLIP_X

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Cutoff;
            CBUFFER_END

            Varyings shadowVert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPivot = TransformObjectToWorld(float3(0, 0, 0));
                float3 lightDir = _MainLightPosition.xyz;

                // 빛 방향 기준 빌보드 (그림자가 자연스럽게 나옴)
                float3 toLight = -lightDir;
                toLight.y = 0;
                toLight = normalize(toLight);

                float3 worldUp = float3(0, 1, 0);
                float3 worldRight = normalize(cross(worldUp, toLight));

                float scaleX = length(unity_ObjectToWorld._m00_m10_m20);
                float scaleY = length(unity_ObjectToWorld._m01_m11_m21);

                float3 worldPos = worldPivot
                    + worldRight * IN.positionOS.x * scaleX
                    + worldUp * IN.positionOS.y * scaleY;

                OUT.positionHCS = TransformWorldToHClip(worldPos);

                float2 uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                #ifdef _FLIP_X
                    uv.x = 1.0 - uv.x;
                #endif
                OUT.uv = uv;

                return OUT;
            }

            half4 shadowFrag(Varyings IN) : SV_Target
            {
                half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).a;
                clip(alpha - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
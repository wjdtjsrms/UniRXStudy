Shader "Jsg/MyLit"
{
    Properties
    {
		[Header(Surface options)]
		[MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
		_Smoothness("Smoothness", float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
			Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }
		
			HLSLPROGRAM 
			
			#define _SPECULAR_COLOR
			
			#if UNITY_VERSION >= 202120
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#endif
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
			
			#pragma vertex Vertex
            #pragma fragment Fragment
			
			#include "MyLitForwardLitPass.hlsl"
			ENDHLSL
        }
    }
}

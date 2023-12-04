Shader "Jsg/MyLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
			#pragma vertex Vertex
            #pragma fragment Fragment
			
			#include "MyLitForwardLitPass.hlsl"
			ENDHLSL
        }
    }
}

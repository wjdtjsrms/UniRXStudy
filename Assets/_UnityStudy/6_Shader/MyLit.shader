Shader "Jsg/MyLit"
{
    Properties
    {
		[Header(Surface options)]
		[MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
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

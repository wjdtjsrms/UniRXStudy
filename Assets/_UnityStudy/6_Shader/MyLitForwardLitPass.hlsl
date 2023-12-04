#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
	float2 uv : TEXCOORD0;
};

struct Interpolators
{
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
};

TEXTURE2D(_ColorMap);
SAMPLER(sampler_ColorMap);

CBUFFER_START(UnityPerMaterial)

	float4 _ColorTint;
	float4 _ColorMap_ST;
	
CBUFFER_END

Interpolators Vertex(Attributes input)
{
    Interpolators output;
    
	VertexPositionInputs posInput = GetVertexPositionInputs(input.positionOS);
	
	output.positionCS = posInput.positionCS;
	output.uv = TRANSFORM_TEX(input.uv, _ColorMap);

	return output;
}

float4 Fragment(Interpolators input) : SV_TARGET
{
	float2 uv = input.uv;
	float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
	
	return colorSample * _ColorTint;
}
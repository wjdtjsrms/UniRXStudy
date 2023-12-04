#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
};

struct Interpolators
{
	float4 positionCS : SV_POSITION;
};

Interpolators Vertex(Attributes input)
{
    Interpolators output;
    
	VertexPositionInputs posInput = GetVertexPositionInputs(input.positionOS);
	
	output.positionCS = posInput.positionCS;

	return output;
}

float4 Fragment(Interpolators input) : SV_TARGET
{
	return float4(1, 1, 1, 1);
}
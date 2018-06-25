//@author: vux
//@help: template for standard shaders
//@tags: template
//@credits:

Texture2D texture2d <string uiname="Texture";>;

SamplerState linearSampler : IMMUTABLE
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
};

cbuffer cbPerDraw : register( b0 )
{
	float4x4 tVP : LAYERVIEWPROJECTION;
	float4x4 tP : PROJECTION;
};

cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float4x4 tWVIT;
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
};

struct VS_IN
{
	float4 PosO : POSITION;
	float3 Normal : NORMAL;
};

struct vs2ps
{
	float4 PosWVP: SV_Position;
};

float2 TransformViewToProjection (float2 v) {
	return float2(v.x*tP[0][0], v.y*tP[1][1]);
}

float OutlineAmout;
vs2ps VS(VS_IN input)
{
	vs2ps output;
	output.PosWVP  = mul(input.PosO,mul(tW,tVP));
	
	//float3 norm = mul((float3x3)tWVIT, input.Normal);
	float3 norm = mul(input.Normal, (float3x3)tWVIT);
	float2 offset = TransformViewToProjection(norm.xy);
	
	output.PosWVP.xy += offset * output.PosWVP.z * OutlineAmout;
	return output;
}

float4 PS(vs2ps In): SV_Target
{
	float4 col = cAmb;
	return col;
}

technique10 Constant
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}

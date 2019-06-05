//@author: vux
//@help: standard constant shader
//@tags: color
//@credits:

struct vsInput
{
	float4 posObject : POSITION;
};

struct psInput
{
	float4 posScreen : SV_Position;
};

struct vsInputTextured
{
	float4 posObject : POSITION;
	float4 uv: TEXCOORD0;
};

struct psInputTextured
{
	float4 posScreen : SV_Position;
	float4 uv: TEXCOORD0;
	float4 uvOrig: TEXCOORD1;
};

Texture2D inputTexture <string uiname="Texture";>;

SamplerState linearSampler <string uiname="Sampler State";>
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
};

cbuffer cbPerDraw : register(b0)
{
	float4x4 tVP : LAYERVIEWPROJECTION;
};

cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float Alpha <float uimin=0.0; float uimax=1.0;> = 1;
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
	float4x4 tColor <string uiname="Color Transform";>;
};

cbuffer cbTextureData : register(b2)
{
	float4x4 tTex <string uiname="Texture Transform"; bool uvspace=true; >;
};

psInputTextured VS_Textured(vsInputTextured input)
{
	psInputTextured output;
	output.posScreen = mul(input.posObject,mul(tW,tVP));
	
	output.uvOrig = input.uv;
	output.uv = mul(input.uv, tTex);
	
	return output;
}

float2 LeftRight = {0, 0};
float2 LeftRightGamma = {1, 1};

float2 BottomTop = {0, 0};
float2 BottomTopGamma = {1, 1};

float4 SoftEdgeColor <bool color=true;>;
float4 PS_Textured(psInputTextured input): SV_Target
{
	float4 col = inputTexture.Sample(linearSampler,input.uv.xy) * cAmb;
	
	col = mul(col, tColor);
	
	//Left
	col.rgb = lerp(SoftEdgeColor, col.rgb, pow(smoothstep(0, LeftRight.x, input.uvOrig.x), LeftRightGamma.x));
	//Right
	col.rgb = lerp(SoftEdgeColor, col.rgb, pow(1-smoothstep(1-LeftRight.y, 1, input.uvOrig.x), LeftRightGamma.y));
	//Up
	col.rgb = lerp(SoftEdgeColor, col.rgb, pow(smoothstep(0, BottomTop.y, input.uvOrig.y), BottomTopGamma.y));
	//Down
	col.rgb = lerp(SoftEdgeColor, col.rgb, pow(1-smoothstep(1-BottomTop.x, 1, input.uvOrig.y), BottomTopGamma.x));
	

	
	col.a *= Alpha;

	return col;
}

technique11 Constant <string noTexCdFallback="ConstantNoTexture"; >
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS_Textured() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_Textured() ) );
	}
}

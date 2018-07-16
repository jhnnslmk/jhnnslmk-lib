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
	float4x4 tV : LAYERVIEW;
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
	//	output.posScreen = mul(input.posObject, mul(tW,tV));
	output.uv = mul(input.uv, tTex);
	return output;
}

float Time : TIME;
float Scale;
bool Direction;

float4 PS_Textured(psInputTextured input): SV_Target
{
	float2 uv = input.uv;
	uv = (Direction) ? uv.yx : uv.xy;
	float4 col = inputTexture.Sample(linearSampler,input.uv.xy) * cAmb;
	
	col.a *= smoothstep(0.0f, 0.5f, 1-abs(2*(uv.y-0.5f)));
	col.a *= ((1-uv.x * Scale + Time) % 1) > 0.5f;
	
	col.a *= Alpha;
	return col;
}

technique11 Constant
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS_Textured() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_Textured() ) );
	}
}

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
};

cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float4 Color <bool color=true;String uiname="Color";> = { 0.0f,0.0f,0.0f,1.0f };
	float4 BorderColor <bool color=true;String uiname="Border Color";> = { 1.0f,1.0f,1.0f,1.0f };
};

struct VS_IN
{
	float4 PosO : POSITION;
	float4 TexCd : TEXCOORD0;

};

struct vs2ps
{
    float4 PosWVP: SV_Position;
    float4 TexCd: TEXCOORD0;
};

vs2ps VS(VS_IN input)
{
    vs2ps output;
    output.PosWVP  = mul(input.PosO,mul(tW,tVP));
    output.TexCd = input.TexCd;
	
    return output;
}

float BorderWidth = 0.1f;
float2 Scale = 1.0f.xx;
float Alpha = 1;
float2 InvAspectRatio = {1, 1};

bool Border(float2 TexCd, float2 BorderWidth)
{
	float2 uv = (TexCd.xy-0.5f)*2;
	float2 s = Scale*InvAspectRatio;
	bool2 b = abs(uv*s) > s-BorderWidth;
	
	return max(b.x, b.y);
}

float4 PS_NoAspect(vs2ps In): SV_Target
{	
	float4 col = texture2d.Sample(linearSampler, In.TexCd.xy) * Color;
	col = lerp(col, BorderColor, Border(In.TexCd.xy, BorderWidth.xx));
	
	col.a *= Alpha;	
    return col;
}

float4 PS_Aspect(vs2ps In): SV_Target
{	
	float4 col = texture2d.Sample(linearSampler, In.TexCd.xy) * Color;
	col = lerp(col, BorderColor, Border(In.TexCd.xy, BorderWidth.xx));
	
	col.a *= Alpha;
    return col;
}

technique10 BorderNoAspect
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_NoAspect() ) );
	}
}

technique10 BorderAspect
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_Aspect() ) );
	}
}




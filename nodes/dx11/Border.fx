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
float Alpha = 1;
float4 PS(vs2ps In): SV_Target
{
	float2 scale = float2((tW._m11/tW._m00)*2, 1);
	bool2 b = 1-abs(2*In.TexCd-1) < BorderWidth*scale;
	
	float4 col = lerp(Color, BorderColor, max(b.x, b.y));
	
	col.a *= Alpha;
	
    return col;
}

technique10 Border
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}





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
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
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

float Alpha;
float2 minmax = float2(0.5f, 0.51f);
float4 PS(vs2ps In): SV_Target
{
//    float4 col = texture2d.Sample(linearSampler,In.TexCd.xy) * cAmb;
	float4 col = 1.0f.xxxx;
	col = cAmb;
	
	float2 center = 0.0f.xx;
	float2 uv = (In.TexCd * 2)-1;
	
	float w = distance(center, uv);
	
	
	col.a = smoothstep(minmax.x, minmax.y, w);
	col.a = (1-col.a) * (w > minmax.x);
	col.a *= Alpha;
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

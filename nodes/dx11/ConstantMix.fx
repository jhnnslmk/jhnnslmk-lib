//@author: readme
//@help: mixing images
//@tags: mix
//@credits: 

Texture2D tex1 <string uiname="Texture";>;
Texture2D tex2 <string uiname="Texture 2";>;

SamplerState linearSampler : IMMUTABLE
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
	float4 color <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
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

float Mix;
float FadeIn;
float Alpha = 1;
float4 BorderColor <bool color=true;>;
float4 PS(vs2ps In): SV_Target
{
    float4 col = tex1.Sample(linearSampler,In.TexCd.xy)*FadeIn;
	float4 col2 = tex2.Sample(linearSampler,In.TexCd.xy);
	col = lerp(col, color, (1-col2.r)*Mix);
	
	bool2 border = abs((In.TexCd.xy-0.5f)*2) > 0.9f;
	col = lerp(col, BorderColor, max(border.x, border.y)*BorderColor.a);
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




